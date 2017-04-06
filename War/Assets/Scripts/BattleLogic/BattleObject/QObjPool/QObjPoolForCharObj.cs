/// <summary>
/// 生成对象，并使用MeshBaker合并对象
/// author : fanzhengyong
/// date  : 2017-02-22
/// 
/// QObjPool是T类型的，所以也支持自定义类型，这是CharObj版本的实现
/// 使用方使用的任然是QObjPool，所以也要遵循QObjPool的借和还的规则。
/// 不能在外面删，只能还回来！！！
/// </summary>
/// 
using UnityEngine;
using System.Collections;

public class CharObjCreator : QObjCreator<CharObj>
{
    //被生成并克隆的对象，称为种子。春天把一个坦克埋进去，到秋天长出好多坦克:)
    private GameObject    m_seed          = null;
    private GameObject    m_meshbakerGo   = null;
    private MB3_MeshBaker m_meshBaker     = null;
    private BattleObjManager.E_BATTLE_OBJECT_TYPE m_type;
    
    //一次生成的对象个数
    public int m_count;
    
    private  static readonly float  MAX_BOUND_SIDE  = 100000f;

    private static readonly Vector3 SMR_BOUND_MAX_X =
        new Vector3(MAX_BOUND_SIDE, CharObj.INIT_POS.y, CharObj.INIT_POS.z);

    private static readonly Vector3 SMR_BOUND_MIN_X = 
        new Vector3(-MAX_BOUND_SIDE, CharObj.INIT_POS.y, CharObj.INIT_POS.z);

    private static readonly Vector3 SMR_BOUND_MAX_Z = 
        new Vector3(CharObj.INIT_POS.x, CharObj.INIT_POS.y, MAX_BOUND_SIDE);

    private static readonly Vector3 SMR_BOUND_MIN_Z = 
        new Vector3(CharObj.INIT_POS.x, CharObj.INIT_POS.y, -MAX_BOUND_SIDE);

    /// <summary>
    /// </summary>
    /// <param name="path">
    /// path[0] meshbaker生成器资源路径
    /// path[1] meshbaker材质资源路径
    /// path[2] meshbaker贴图资源路径
    /// path[3] meshbaker合并对象资源路径
    /// </param>
    /// <param name="type"></param>
    /// <param name="count"></param>
    public CharObjCreator(string[] paths, BattleObjManager.E_BATTLE_OBJECT_TYPE type, int count)
    {
        bool retCode        = false;

        m_type = type;
        m_meshbakerGo = ResourcesManagerMediator.
            GetGameObjectFromResourcesManager(paths[0]);
        if (m_meshbakerGo == null)
        {
            Debug.LogError("加载baker生成器资源出错" + paths[0]);
            return;
        }

        MB3_TextureBaker textureBaker = m_meshbakerGo.GetComponent<MB3_TextureBaker>();
        m_meshBaker = m_meshbakerGo.GetComponentInChildren<MB3_MeshBaker>();

        retCode = InitBaker(paths, textureBaker, m_meshBaker);
        if (!retCode)
        {
            Debug.LogError("MeshBaker初始化失败");
            return;
        }

        m_seed = ResourcesManagerMediator.GetGameObjectFromResourcesManager(paths[3]);
        if (m_seed == null)
        {
            Debug.LogError("加载种子资源失败 " + paths[3]);
            return;
        }
        m_seed.transform.position = CharObj.INIT_POS;

        m_count = count;
    }

    private CharObjCreator() { }

    public override CharObj[] CreateObjects()
    {
        GameObject[] goObjs   = new GameObject[m_count];
        CharObj[]    charObjs = new CharObj[m_count];
        for (int i = 0; i < m_count; i++)
        {
            GameObject go = GameObject.Instantiate<GameObject>(m_seed);
            
#if _WAR_TEST_
            MeshBakerManager.Instance.AddGo(go);
#endif
            go.transform.position = CharObj.INIT_POS;
            goObjs[i]             = go;
            CharObj charObj = new CharObj(goObjs[i], m_type);
            charObjs[i]           = charObj;
        }

        //人为调整合并后smr的bound
        charObjs[0].GameObject.transform.position = SMR_BOUND_MAX_X;
        charObjs[1].GameObject.transform.position = SMR_BOUND_MIN_X;
        charObjs[2].GameObject.transform.position = SMR_BOUND_MAX_Z;
        charObjs[3].GameObject.transform.position = SMR_BOUND_MIN_Z;

        //M_Build_Jeep_Seed模型不能合并，先跳过
        //Debug.Log(m_seed.name);
        if (m_seed.name == "M_Build_Jeep_Seed(Clone)")
        {
            return charObjs;
        }

        m_meshBaker.AddDeleteGameObjects(goObjs, null, true);
        m_meshBaker.Apply();

#if _WAR_TEST_
        MeshBakerManager.Instance.AddCombine(m_meshBaker, m_seed, m_meshbakerGo);
#endif
        return charObjs;
    }

    public override void HideObject(CharObj obj)
    {
        //停下来
        obj.Deactive();
        //关闭所有特效
        //放到初始位置
        obj.GameObject.transform.position = CharObj.INIT_POS;
    }

    public override void RealseObject(CharObj obj)
    {

    }

    public override void Realse()
    {

    }

    private bool InitBaker(string[] paths, MB3_TextureBaker textureBaker, MB3_MeshBaker meshBaker)
    {
        bool result = false;

        Material material = ResourcesManagerMediator.
            GetNoGameObjectFromResourcesManager<Material>(paths[1]);
        if (material == null)
        {
            Debug.LogError("加载合并材质资源失败" + paths[1]);
            return result;
        }

        MB2_TextureBakeResults textureBakeResults = ResourcesManagerMediator.
            GetNoGameObjectFromResourcesManager<MB2_TextureBakeResults>(paths[2]);
        if (textureBakeResults == null)
        {
            Debug.LogError("加载MB2_TextureBakeResults资源失败" + paths[2]);
            return result;
        }

        textureBaker.resultMaterial     = material;
        textureBaker.textureBakeResults = textureBakeResults;
        meshBaker.textureBakeResults    = textureBakeResults;
#if _WAR_TEST_
        MeshBakerManager.Instance.AddMaterial(material);
        MeshBakerManager.Instance.AddTexture(textureBakeResults);
#endif
        result = true;
        return result;
    }
}

public class CharObjCreatorFactory : QObjCreatorFactory<CharObj>
{
    private string[] m_paths;
    private BattleObjManager.E_BATTLE_OBJECT_TYPE m_type;
    private int m_count;

    /// <summary>
    /// </summary>
    /// <param name="path">
    /// path[0] meshbaker生成器资源路径
    /// path[1] meshbaker材质资源路径
    /// path[2] meshbaker贴图资源路径
    /// path[3] meshbaker合并对象资源路径
    /// </param>
    /// <param name="type"></param>
    /// <param name="count"></param>
    public CharObjCreatorFactory(string[] paths, BattleObjManager.E_BATTLE_OBJECT_TYPE type, int count)
    {
        m_paths = paths;
        m_type  = type;
        m_count = count;
    }

    private CharObjCreatorFactory() { }
    public override QObjCreator<CharObj> CreatCreator()
    {
        CharObjCreator creator = new CharObjCreator(
            m_paths, m_type, m_count);
        return creator;
    }
}
