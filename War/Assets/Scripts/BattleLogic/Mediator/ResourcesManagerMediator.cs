/// <summary>
/// ResourcesManager的中介
/// author : fanzhengyong
/// date  : 2017-03-01
/// 
/// </summary>
using UnityEngine;
using System.Collections;

public class ResourcesManagerMediator
{
    public static T GetNoGameObjectFromResourcesManager<T>(string path) where T : Object
    {
        T t = default(T);
        t   = Resources.Load<T>(path);
        return t;
    }
    
    public static GameObject GetGameObjectFromResourcesManager(string path)
    {
        GameObject go    = null;
        GameObject goRes = null;
        goRes = Resources.Load<GameObject>(path);
        if (goRes == null)
        {
            Debug.LogError("资源加载错误！ " + path);
            return go;
        }

        go = GameObject.Instantiate<GameObject>(goRes);

        return go;

    }

    public static GameObject GetGameObjectFromResourcesManager2(string path)
    {
        GameObject go = null;
        GameObject goRes = null;
        //Debug.Log(Application.dataPath);
        string _path = "Assets/" + path;
        goRes = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(_path);
        if (goRes == null)
        {
            Debug.LogError("资源加载错误！ " + _path);
            return go;
        }

        go = GameObject.Instantiate<GameObject>(goRes);

        return go;
    }


}
