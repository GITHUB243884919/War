/// <summary>
/// Asset.Path-AB.Path 映射表(assetlist.txt)
///     m_assetsPathMapBunlesPath
///     
/// manifest加载
/// 
/// Cache.AB 映射表(path-AssetBundle)
///     m_cacheBundles
/// 
/// Asset.ID-AB.Path 映射表
///     m_cacheAssetsIDMapBunlesPath
///     
/// GameObject.ID-Asset.ID 映射表
///     m_cacheGameObjectIDMapAssetID
///     
/// AB.Path-AB.Hash对应表(bundlelist.txt)
/// 
/// </summary>

using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;


#if _LOG_MEDIATOR_
using Debug = LogMediator;
#endif

namespace UF_Framework
{
public class ResourcesManager2 : MonoBehaviour
{
    private static ResourcesManager2 s_instance = null;
    private bool m_isInit = false;

    public static string STREAMING_ASSET_PATH = null;

    public static string STREAMING_ASSET_LIST_PATH = null;

    //Asset路径和Bundle路径映射
    private Dictionary<string, string> m_assetsPathMapBunlesPath
        = new Dictionary<string, string>();

    //Bundle的缓存
    private Dictionary<string, BundleElement> m_cacheBundles
        = new Dictionary<string, BundleElement>();

    //Asset.ID和Bundle.Path的映射
    private Dictionary<int, string> m_cacheAssetsIDMapBunlesPath
        = new Dictionary<int, string>();

    private Dictionary<int, GameObject> m_cacheGameObjectIDMapAssetID
        = new Dictionary<int, GameObject>();

    //全局Manifest
    private AssetBundleManifest m_manifest = null;

    public static ResourcesManager2 Instance
    {
        get
        {
            if (s_instance == null)
            {
                Debug.LogError("没有持有ResourcesManager的对象");

            }
            return s_instance;
        }
    }

    public void InitPath()
    {
        STREAMING_ASSET_PATH =
#if (UNITY_ANDROID) && (!UNITY_EDITOR)
        Application.dataPath + "!assets";
#else
        Application.streamingAssetsPath;
#endif

        STREAMING_ASSET_LIST_PATH =
            STREAMING_ASSET_PATH + "/bundleinfo/assetlist.bundle";

    }

    public void Init()
    {
        if (m_isInit)
        {
            return;
        }

        InitPath();
        LoadAssetsPathMapBunlesPath();
        LoadManifest();

        m_isInit = true;
    }

    /// <summary>
    /// 同步获取资源
    /// 依赖的所有AssetBundle的引用计数+1
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetPath"></param>
    /// <returns></returns>
    public T LoadAssetSync<T>(string assetPath) where T : Object
    {
        T t = default(T);
        bool retCode = false;

        string bundlePath = null;
        retCode = m_assetsPathMapBunlesPath.TryGetValue(assetPath, out bundlePath);
        if (!retCode)
        {
            Debug.LogError("没有找到对应的assetbundle" + assetPath);
            return t;
        }
        bundlePath = STREAMING_ASSET_PATH + "/" + bundlePath;
        t = LoadAssetSync<T>(bundlePath, assetPath);

        return t;
    }

    /// <summary>
    /// 释放资源
    /// 1.Asset = null
    /// 2.依赖的所有AssetBundle的引用计数-1
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    public void ReleaseAsset<T>(ref T t, bool isGameObject = false) where T : Object
    {
        bool retCode = false;

        string bundlePath = null;
        int assetID = t.GetInstanceID();
        retCode = m_cacheAssetsIDMapBunlesPath.TryGetValue(
            assetID, out bundlePath);
        if (!retCode)
        {
            Debug.Log("Asset的ID没有找到对应AssetBundle" + t.name + " " + assetID);
            return;
        }

        ReleaseAssetBundle(bundlePath);

        if (!isGameObject)
        {
            t = default(T);
        }
        
    }

    /// <summary>
    /// 取GameObject
    /// 记录GameObject.ID和Asset.ID的映射关系
    /// 是Asset对应的AssetBundle的引用计数+1
    /// 
    /// </summary>
    /// <param name="assetPath"></param>
    /// <returns></returns>
    public GameObject GetGameObject(string assetPath)
    {
        GameObject go = null;
        GameObject orginGo = null;
        GameObject insGo = null;
        bool retCode = false;
        //处理的引用计数
        orginGo = LoadAssetSync<GameObject>(assetPath);
        insGo = GameObject.Instantiate<GameObject>(orginGo);
        int gameObjectID = insGo.GetInstanceID();
        //int assetID = 0;
        retCode = m_cacheGameObjectIDMapAssetID.TryGetValue(gameObjectID, out orginGo);
        if (retCode)
        {
            Debug.LogError("GameObject ID 已存在");
            return go;
        }

        m_cacheGameObjectIDMapAssetID.Add(gameObjectID, orginGo);
        go = insGo;

        return go;
    }

    /// <summary>
    /// 释放GameObject
    /// 1.找到对应的Asset(就是实例化之前的GameObject)
    /// 2.Asset对应的AssetBundle的引用计数-1
    /// </summary>
    /// <param name="go"></param>
    public void ReleaseGameObject(ref GameObject go)
    {
        bool retCode = false;

        int gameObjectID = go.GetInstanceID();
        GameObject orginGo = null;
        retCode = m_cacheGameObjectIDMapAssetID.TryGetValue(gameObjectID, out orginGo);
        if (!retCode)
        {
            Debug.LogError("GameObject ID 不存在");
            return;
        }

        ReleaseAsset(ref orginGo, true);

        GameObject.Destroy(go);
        go = null;
    }

    /// <summary>
    /// 克隆GameObject
    /// 1.orginGo是曾经被实例化后的go
    /// 2.找到orginGo对应的AssetBundle，引用计数+1
    /// 3.建立实例化后go的ID和Asset的对应关系
    /// </summary>
    /// <param name="orginGo"></param>
    /// <returns></returns>
    public GameObject CloneGameObject(GameObject orginGo)
    {
        bool retCode = false;
        GameObject go = null;
        GameObject insGo = null;
        insGo = GameObject.Instantiate<GameObject>(orginGo);
        int gameObjectID = orginGo.GetInstanceID();
        GameObject assetGo = null;
        retCode = m_cacheGameObjectIDMapAssetID.TryGetValue(gameObjectID, out assetGo);
        if (!retCode)
        {
            Debug.LogError("GameObject ID 不存在");
            return go;
        }
        int AssetID = assetGo.GetInstanceID();
        string path = null;
        retCode = m_cacheAssetsIDMapBunlesPath.TryGetValue(AssetID, out path);
        if (!retCode)
        {
            Debug.LogError("Asset ID 不存在");
            return go;
        }

        //AssetBundle引用计数+1
        LoadAssetBundleSync(path);
        //实例化后的GameObject的ID和Asset（实例化前的GameObject）建立映射关系
        m_cacheGameObjectIDMapAssetID.Add(
            insGo.GetInstanceID(), assetGo);
        go = insGo;
        return go;
    }

    bool LoadAssetsPathMapBunlesPath()
    {
        bool result = false;
        TextAsset assetListAsset = LoadAssetSync<TextAsset>(
            STREAMING_ASSET_LIST_PATH,
            STREAMING_ASSET_LIST_PATH,
            false);

        if (assetListAsset == null)
        {
            Debug.Log(STREAMING_ASSET_LIST_PATH + " 加载失败");
            return result;
        }
        Debug.Log(STREAMING_ASSET_LIST_PATH + " " + assetListAsset.GetInstanceID());
        using (StringReader sr = new StringReader(assetListAsset.text))
        {
            string line = null;
            while ((line = sr.ReadLine()) != null)
            {
                string[] split = line.Split(',');
                //Debug.Log(split[0] + " " + split[1]);
                m_assetsPathMapBunlesPath.Add(split[0], split[1]);
            }
        }

        result = true;
        return result;
    }

    bool LoadManifest()
    {
        bool result = false;

        string path = STREAMING_ASSET_PATH + "/StreamingAssets";

        m_manifest = LoadAssetSync<AssetBundleManifest>(path, "AssetBundleManifest", false);
        if (m_manifest == null)
        {
            Debug.Log("Manifest加载失败");
            return result;
        }
        Debug.Log("m_manifest " + (m_manifest != null));
        result = true;
        return result;
    }

    /// <summary>
    /// AssetBundle同步加载
    /// 1.检查cache中有没有
    /// 2.cache中有，从cache加载
    /// --2.1本bundle本身引用+1，本bundle所有依赖+1
    /// 3.cache中没有，从磁盘加载
    /// --3.1如果有依赖先加载依赖
    /// --3.2本bundle本身引用+1，本bundle所有依赖+1
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    AssetBundle LoadAssetBundleSync(string path, bool isProcessDependencies = true)
    {
        BundleElement bundleElment = null;
        AssetBundle bundle = null;

        bool retCode = false;
        retCode = m_cacheBundles.TryGetValue(path, out bundleElment);
        if (retCode)
        {
            bundle = bundleElment.Bundle;
            if (bundle == null)
            {
                Debug.LogError("Cache中获得的AssetBundle为空 " + path);
                return bundle;
            }

            //处理依赖
            if (isProcessDependencies)
            {
                bundleElment.AddRef();

                //递归依赖
                var dependencies = m_manifest.GetDirectDependencies(path);
                foreach (var dependency in dependencies)
                {
                    Debug.Log("bundle name: " + path + " dependent by :" + dependency);
                    LoadAssetBundleSync(dependency);
                }
            }

            return bundle;
        }

        bundle = AssetBundle.LoadFromFile(path);
        if (bundle != null)
        {
            bundleElment = new BundleElement(bundle);
            m_cacheBundles.Add(path, bundleElment);

            //处理依赖
            if (isProcessDependencies)
            {
                bundleElment.AddRef();

                //递归依赖
                Debug.Log("m_manifest " + (m_manifest != null) + " " + path);
                var dependencies = m_manifest.GetDirectDependencies(path);
                foreach (var dependency in dependencies)
                {
                    Debug.Log("bundle name: " + path + " dependent by :" + dependency);
                    LoadAssetBundleSync(dependency);
                }
            }

        }

        return bundle;
    }

    /// <summary>
    /// 释放AssetBundle
    /// 依赖的所有的AssetBundle的引用计数-1
    /// </summary>
    /// <param name="path"></param>
    /// <param name="isProcessDependencies"></param>
    public void ReleaseAssetBundle(string path, bool isProcessDependencies = true)
    {
        BundleElement bundleElment = null;
        //AssetBundle bundle = null;

        bool retCode = false;
        retCode = m_cacheBundles.TryGetValue(path, out bundleElment);
        if (!retCode)
        {
            return;
        }

        if (isProcessDependencies)
        {
            bundleElment.MinusDef();

            //递归依赖
            var dependencies = m_manifest.GetDirectDependencies(path);
            foreach (var dependency in dependencies)
            {
                Debug.Log("bundle name: " + path + " dependent by :" + dependency);
                ReleaseAssetBundle(dependency);
            }
        }

    }

    public T LoadAssetSync<T>(AssetBundle bundle, string assetName) where T : Object
    {
        T t = default(T);

        if (bundle != null)
        {
            t = bundle.LoadAsset<T>(assetName);
        }

        return t;
    }

    /// <summary>
    /// 同步加载资源
    /// 1.加载资源
    /// 2.把资源的id和bundlePath缓存起来
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bundlePath"></param>
    /// <param name="assetPath"></param>
    /// <param name="isProcessDependencies"></param>
    /// <returns></returns>
    T LoadAssetSync<T>(string bundlePath, string assetPath,
        bool isProcessDependencies = true) where T : Object
    {
        Debug.Log("LoadAssetSync " + assetPath);

        T t = default(T);
        bool retCode = false;

        AssetBundle bundle = LoadAssetBundleSync(bundlePath, isProcessDependencies);
        if (bundle == null)
        {
            Debug.LogError("AssetBundle加载失败 " + bundlePath);
            return t;
        }

        string assetName = Path.GetFileNameWithoutExtension(assetPath);
        t = LoadAssetSync<T>(bundle, assetName);
        if (isProcessDependencies
            && (t != default(T))
        )
        {
            Debug.Log("Cache Asset.ID " + assetPath + " " + t.GetInstanceID());
            int instanceID = t.GetInstanceID();
            string path = null;

            retCode = m_cacheAssetsIDMapBunlesPath.TryGetValue(instanceID, out path);
            if (!retCode)
            {
                //retCode == true是正常情况，因为同一个AssetBundle取出的相同的
                //Asset的InstanceID是相同的。
                m_cacheAssetsIDMapBunlesPath.Add(t.GetInstanceID(), bundlePath);
            }
        }

        return t;
    }

    //--------------------------------
    bool LoadAssetBundleAsync()
    {
        bool result = false;

        return result;
    }


    //--------------
    void Release()
    {
        s_instance = null;

        if (m_assetsPathMapBunlesPath != null)
        {
            m_assetsPathMapBunlesPath.Clear();
            m_assetsPathMapBunlesPath = null;
        }

        if (m_cacheBundles != null)
        {
            foreach (var e in m_cacheBundles.Values)
            {
                e.Release();
            }
            m_cacheBundles.Clear();
            m_cacheBundles = null;
        }

        if (m_cacheAssetsIDMapBunlesPath != null)
        {
            m_cacheAssetsIDMapBunlesPath.Clear();
            m_cacheAssetsIDMapBunlesPath = null;
        }

        m_isInit = false;
    }

    //---------------------
    public void Print_CacheAssetsIDMapBunlesPath()
    {
        Debug.Log("========CacheAssetsIDMapBunlesPath===");
        foreach (var pair in m_cacheAssetsIDMapBunlesPath)
        {
            Debug.Log(pair.Key + " " + pair.Value);
        }
        Debug.Log("=================================");
    }

    //----Unity----------------------------
    void Awake()
    {
        s_instance = this;
    }

    void OnDestroy()
    {
        Release();
    }
}

}

