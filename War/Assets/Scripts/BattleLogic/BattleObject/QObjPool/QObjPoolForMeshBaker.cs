/// <summary>
/// 生成对象，并使用MeshBaker合并对象
/// author : fanzhengyong
/// date  : 2017-02-22
/// 
/// 本质上是QObjPool的一种GameObject版本的实现
/// 使用方使用的还是QObjPool，所以也要遵循QObjPool的借和还的规则。不能在外面删，只能还回来！！！
/// </summary>
/// 
using UnityEngine;
using System.Collections;

public class QObjCreatorForMeshBaker : QObjCreator<GameObject>
{
    //被生成并克隆的对象，称为种子。春天把一个坦克埋进去，到秋天长出好多坦克:)
    private GameObject m_seed;
    private GameObject m_meshbakerGo;
    //private BattleObjManager.E_BATTLE_OBJECT_TYPE m_type;
    
    //一次生成的对象个数
    public int m_count;
    
    //初始化生成位置固定,是一个在场景中看不到的地方。
    private readonly Vector3 INIT_POS = new Vector3(0f, -10f, 0f);
    private readonly float   MAX_BOUND_SIDE = 1000f;

    private MB3_MeshBaker m_meshBaker;

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
    public QObjCreatorForMeshBaker(string[] paths, BattleObjManager.E_BATTLE_OBJECT_TYPE type, int count)
    {
        GameObject bakerRes = Resources.Load<GameObject>(paths[0]);
        if (bakerRes == null)
        {
            Debug.LogError("加载baker生成器资源出错" + paths[0]);
            return;
        }
        m_meshbakerGo = GameObject.Instantiate<GameObject>(bakerRes);

        MB3_TextureBaker textureBaker = m_meshbakerGo.GetComponent<MB3_TextureBaker>();
        m_meshBaker = m_meshbakerGo.GetComponentInChildren<MB3_MeshBaker>();

        InitBaker(paths, textureBaker, m_meshBaker);

        //m_type = type;
        GameObject seedRes = Resources.Load<GameObject>(paths[3]);
        m_seed = GameObject.Instantiate<GameObject>(seedRes);
        m_seed.transform.position = INIT_POS;

        m_count = count;
    }
    private QObjCreatorForMeshBaker() { }

    public override GameObject[] CreateObjects()
    {
        GameObject[] objects = new GameObject[m_count];
        for (int i = 0; i < m_count; i++)
        {
            GameObject go = GameObject.Instantiate<GameObject>(m_seed);
            go.transform.position = INIT_POS;
            objects[i] = go;
            //go.transform.SetParent(m_seed.transform, false);
        }

        //人为调整合并后smr的bound
        objects[0].transform.position = new Vector3(MAX_BOUND_SIDE, INIT_POS.y, INIT_POS.z);
        objects[1].transform.position = new Vector3(-MAX_BOUND_SIDE, INIT_POS.y, INIT_POS.z);
        objects[2].transform.position = new Vector3(INIT_POS.x, INIT_POS.y, MAX_BOUND_SIDE);
        objects[3].transform.position = new Vector3(INIT_POS.x, INIT_POS.y, -MAX_BOUND_SIDE);

        m_meshBaker.AddDeleteGameObjects(objects, null, true);
        m_meshBaker.Apply();

        return objects;
    }

    public override void HideObject(GameObject obj)
    {
        obj.transform.position = INIT_POS;
    }

    public override void RealseObject(GameObject obj)
    {

    }

    private bool InitBaker(string[] paths, MB3_TextureBaker textureBaker, MB3_MeshBaker meshBaker)
    {
        bool result = false;

        Material material = Resources.Load<Material>(paths[1]);
        if (material == null)
        {
            return result;
        }

        MB2_TextureBakeResults textureBakeResults =
            Resources.Load<MB2_TextureBakeResults>(paths[2]);
        if (textureBakeResults == null)
        {
            return result;
        }

        textureBaker.resultMaterial = material;
        textureBaker.textureBakeResults = textureBakeResults;
        meshBaker.textureBakeResults = textureBakeResults;

        result = true;
        return result;
    }
}

public class QObjCreatorFactoryForMeshBaker : QObjCreatorFactory<GameObject>
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
    public QObjCreatorFactoryForMeshBaker(string[] paths, BattleObjManager.E_BATTLE_OBJECT_TYPE type, int count)
    {
        m_paths = paths;
        m_type = type;
        m_count = count;
    }

    private QObjCreatorFactoryForMeshBaker() { }
    public override QObjCreator<GameObject> CreatCreator()
    {
        QObjCreatorForMeshBaker creator = new QObjCreatorForMeshBaker(
            m_paths, m_type, m_count);
        return creator;
    }
}
