using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

#if _WAR_TEST_
using Debug = LogMediator;
#endif

public class ResourcesManager2 : MonoBehaviour
{
    private static ResourcesManager2 s_instance = null;

    public static readonly string STREAMING_ASSET_PATH =
#if (UNITY_ANDROID) && (!UNITY_EDITOR)
        Application.dataPath + "!assets";
#else
        Application.streamingAssetsPath;
#endif

    public static readonly string STREAMING_ASSET_LIST_PATH =
        STREAMING_ASSET_PATH + "/bundleinfo/assetlist.bundle";

    public Dictionary<string, string> m_assets
        = new Dictionary<string, string>();

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
        InitAssetList();
    }

    bool InitAssetList()
    {
        bool result = false;
        TextAsset assetListAsset = LoadAssetSync<TextAsset>(STREAMING_ASSET_LIST_PATH, 
            STREAMING_ASSET_LIST_PATH);

        Debug.Log("assetListAsset " + (assetListAsset != null));
        if (assetListAsset == null)
        {
            return result;
        }

        using (StringReader sr = new StringReader(assetListAsset.text))
        {
            string line = null;
            while ((line = sr.ReadLine()) != null)
            {
                string[] split = line.Split(',');
                Debug.Log(split[0] + " " + split[1]);
                m_assets.Add(split[0], split[1]);
            }
        }

        return result;
    }

    AssetBundle LoadAssetBundleSync(string path)
    {
        AssetBundle result = null;
        result = AssetBundle.LoadFromFile(path);
        return result;
    }

    T LoadAssetSync<T>(AssetBundle bundle, string assetName) where T : Object
    {
        T t = default(T);

        if (bundle != null)
        {
            t = bundle.LoadAsset<T>(assetName);
        }

        return t;
    }

    T LoadAssetSync<T>(string bundlePath, string assetPath) where T : Object
    {
        T t = default(T);

        AssetBundle bundle = LoadAssetBundleSync(bundlePath);
        if (bundle != null)
        {
            string assetName = Path.GetFileNameWithoutExtension(assetPath);
            t = LoadAssetSync<T>(bundle, assetName);
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

        if (m_assets != null)
        {
            m_assets.Clear();
            m_assets = null;
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
