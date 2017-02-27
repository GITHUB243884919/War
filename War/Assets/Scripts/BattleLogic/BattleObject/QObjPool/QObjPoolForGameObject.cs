/// <summary>
/// QObjPool的通用GameObject版本实现，打算用来缓存角色的攻击特效之类。
/// author : fanzhengyong
/// date  : 2017-02-23
/// 
/// </summary>
/// 
using UnityEngine;
using System.Collections;

public class QObjCreatorForGameObject : QObjCreator<GameObject>
{
    public string m_path;

    public int m_count;

    public static readonly Vector3 INIT_POS = new Vector3(0f, -10f, 0f);

    private GameObject m_seed;

    private bool isInit = false;

    public QObjCreatorForGameObject(string path, int count)
    {
        GameObject seedRes = Resources.Load<GameObject>(path);
        if (seedRes == null)
        {
            Debug.LogWarning("读取种子资源出错 " + path);
            return;
        }
        m_seed = GameObject.Instantiate<GameObject>(seedRes);

        m_seed.transform.position = INIT_POS;

        m_path  = path;
        m_count = count;
        isInit  = true;
    }

    private QObjCreatorForGameObject() { }

    public override void Realse()
    {
        m_seed = null;
    }

    public override GameObject[] CreateObjects()
    {
        if (!isInit)
        {
            return null;
        }

        GameObject[] objs = new GameObject[m_count];
        for (int i = 0; i < m_count; i++)
        {
            GameObject go = GameObject.Instantiate<GameObject>(m_seed);
            go.transform.position = INIT_POS;
            objs[i] = go;
        }

        return objs;
    }

    public override void HideObject(GameObject obj)
    {
        obj.transform.position = INIT_POS;
    }

    public override void RealseObject(GameObject obj)
    {

    }

}

