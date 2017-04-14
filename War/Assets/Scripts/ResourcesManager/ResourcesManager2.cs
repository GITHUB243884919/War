using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

#if _LOG_MEDIATOR_
using Debug = LogMediator;
#endif

/// <summary>
/// Asset.Path-AB.Path对应表
/// manifest加载
/// 
/// Cache.AB表
/// AB.Path-AB.Hash对应表
/// </summary>
public class ResourcesManager2 : MonoBehaviour
{
    private static ResourcesManager2 s_instance = null;
    private bool m_isInit = false;

    public static readonly string STREAMING_ASSET_PATH =
#if (UNITY_ANDROID) && (!UNITY_EDITOR)
        Application.dataPath + "!assets";
#else
        Application.streamingAssetsPath;
#endif

    public static readonly string STREAMING_ASSET_LIST_PATH =
        STREAMING_ASSET_PATH + "/bundleinfo/assetlist.bundle";

    //Asset路径和Bundle路径映射
    private Dictionary<string, string> m_assetsPathMapBunlesPath
        = new Dictionary<string, string>();

    //Bundle的缓存
    private Dictionary<string, AssetBundle> m_cacheBundles
        = new Dictionary<string, AssetBundle>();

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

    public void Init()
    {
        if (m_isInit)
        {
            return;
        }
        LoadAssetsPathMapBunlesPath();
        LoadManifest();

        m_isInit = true;
    }

    bool LoadAssetsPathMapBunlesPath()
    {
        bool result = false;
        TextAsset assetListAsset = LoadAssetSync<TextAsset>(
            STREAMING_ASSET_LIST_PATH, 
            STREAMING_ASSET_LIST_PATH);

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

        m_manifest = LoadAssetSync<AssetBundleManifest>(path, "AssetBundleManifest");
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
    AssetBundle LoadAssetBundleSync(string path)
    {
        AssetBundle bundle = null;
        bool retCode = false;
        retCode = m_cacheBundles.TryGetValue(path, out bundle);
        if (retCode)
        {
            return bundle;
        }

        bundle = AssetBundle.LoadFromFile(path);
        if (bundle != null)
        {
            m_cacheBundles.Add(path, bundle);
        }

        return bundle;
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

    public T LoadAssetSync<T>(string bundlePath, string assetPath) where T : Object
    {
        T t = default(T);

        AssetBundle bundle = LoadAssetBundleSync(bundlePath);
        if (bundle == null)
        {
            Debug.Log("AssetBundle加载失败 " + bundlePath);
            return t;
        }

        string assetName = Path.GetFileNameWithoutExtension(assetPath);
        t = LoadAssetSync<T>(bundle, assetName);
        
        return t;
    }

    public T LoadAssetSync<T>(string assetPath) where T : Object
    {
        T t          = default(T);
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
