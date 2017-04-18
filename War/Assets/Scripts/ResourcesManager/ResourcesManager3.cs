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
public class ResourcesManager3 : MonoBehaviour
{
    private static ResourcesManager3 s_instance = null;
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

    public static ResourcesManager3 Instance
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
    public T LoadAsset<T>(string assetPath) where T : Object
    {
        T t = default(T);

        string bundlePath = FindBundlePath(assetPath);
        if (string.IsNullOrEmpty(bundlePath))
        {
            Debug.LogError("没有找到对应的assetbundle" + assetPath);
            return t;
        }

        //Debug.Log(bundlePath);
        bundlePath = STREAMING_ASSET_PATH + "/" + bundlePath;
        t = LoadAsset<T>(assetPath, bundlePath);
        if (t != default(T))
        {
            bool retCode = false;
            int  AssetID = t.GetInstanceID();
            string bundlePathInCache = null;
            retCode = m_cacheAssetsIDMapBunlesPath.TryGetValue(
                AssetID, out bundlePathInCache);
            if (!retCode)
            {
                m_cacheAssetsIDMapBunlesPath.Add(AssetID, bundlePath);
            }
        }

        return t;
    }

    public void ReleaseAsset<T>(ref T t) where T : Object
    {
        bool retCode = false;
        int AssetID = t.GetInstanceID();
        string bundlePath = null;
        retCode = m_cacheAssetsIDMapBunlesPath.TryGetValue(
            AssetID, out bundlePath);
        if (!retCode)
        {
            Debug.LogError("缓存中没有找到AssetBundle　" + AssetID);
            return;
        }

        Debug.Log("ReleaseAsset " + bundlePath);
        ReleaseBundle(bundlePath);

        t = default(T);
    }

    string FindBundlePath(string assetPath)
    {
        string bundlePath = null;
        bool retCode = false;
        retCode = m_assetsPathMapBunlesPath.TryGetValue(assetPath, out bundlePath);
        if (!retCode)
        {
            Debug.LogError("没有找到对应的assetbundle" + assetPath);
            return bundlePath;
        }
        //bundlePath = STREAMING_ASSET_PATH + "/" + bundlePath;

        return bundlePath;
    }

    T LoadAsset<T>(string assetPath, string bundlePath, 
        bool isProcessRefCount = true) where T : Object
    {
        T t = default(T);

        AssetBundle bundle = null;

        bundle = LoadBundle(bundlePath, isProcessRefCount);

        if (bundle == null)
        {
            Debug.LogError("获取Assetbundle失败");
            return t;
        }

        t = bundle.LoadAsset<T>(Path.GetFileNameWithoutExtension(assetPath));
        return t;
    }

    AssetBundle LoadBundle(string bundlePath, bool isProcessRefCount = true)
    {
        AssetBundle bundle = null;
        BundleElement bundleElement = null;
        bundle = GetBundleFromCache(bundlePath, out bundleElement);
        if (bundle == null)
        {
            //bundle==null那么bundleElement也是==null的
            bundle = LoadBunleFromLocalFile(bundlePath);
            if (bundle == null)
            {
                Debug.LogError("磁盘加载AssetBundle失败 " + bundlePath);
                return bundle;
            }
            bundleElement = new BundleElement(bundle);
            m_cacheBundles.Add(bundlePath, bundleElement);
        }

        if (isProcessRefCount)
        {
            bundleElement.AddRef();

            var dependencies = m_manifest.GetDirectDependencies(bundlePath);
            foreach (var dependency in dependencies)
            {
                Debug.Log("bundle name: " + bundlePath + " dependent by :" + dependency);
                LoadBundle(dependency, isProcessRefCount);
            }
        }

        return bundle;
    }

    void ReleaseBundle(string bundlePath, bool isProcessRefCount = true)
    {
        AssetBundle bundle = null;
        BundleElement bundleElement = null;
        bundle = GetBundleFromCache(bundlePath, out bundleElement);
        if (bundle == null)
        {
            Debug.LogError("缓存中没有找到AssetBundle" + bundlePath);
            return;
        }
        Debug.Log("ReleaseBundle " + bundlePath);
        if (isProcessRefCount)
        {
            bundleElement.MinusDef();

            var dependencies = m_manifest.GetDirectDependencies(bundlePath);
            foreach (var dependency in dependencies)
            {
                Debug.Log("bundle name: " + bundlePath + " dependent by :" + dependency);
                ReleaseBundle(dependency, isProcessRefCount);
            }
        }

    }

    AssetBundle GetBundleFromCache(string bundlePath, out BundleElement bundleElement)
    {
        AssetBundle bundle = null;
        bool retCode = false;
        bundleElement = null;
        retCode = m_cacheBundles.TryGetValue(bundlePath, out bundleElement);
        if (retCode)
        {
            bundle = bundleElement.Bundle;
        }

        return bundle;
    }

    AssetBundle LoadBunleFromLocalFile(string bundlePath)
    {
        AssetBundle bundle = null;

        bundle = AssetBundle.LoadFromFile(bundlePath);

        return bundle;
    }

    bool LoadAssetsPathMapBunlesPath()
    {
        bool result = false;
        TextAsset assetListAsset = LoadAsset<TextAsset>(
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

        m_manifest = LoadAsset<AssetBundleManifest>("AssetBundleManifest", path, false);
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

    public void Print_CacheBundles()
    {
        Debug.Log("************cacheBundles**********");
        foreach(var pair in m_cacheBundles)
        {
            Debug.Log(pair.Key + " " + pair.Value.RefCount);
        }
        Debug.Log("*******************************");
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

