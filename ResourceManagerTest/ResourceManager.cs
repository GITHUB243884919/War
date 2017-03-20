using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ResourceManager
{
    private static ResourceManager m_instance = null;
    public static ResourceManager GetInstance()
    {
        if (m_instance == null)
        {
            m_instance = new ResourceManager();
        }
        return m_instance;
    }

    public void Init()
    {
        InitPath();
        InitAssetsDict();
        InitManifest();

    }

    private string m_streamPathWithOutWWW  = "";
    private string m_streamPathWithWWW     = "";
    private string m_appPathWithOutWWW     = "";
    private string m_appPathWithWWW        = "";
    private void InitPath()
    {
#if UNITY_STANDALONE
        m_streamPathWithOutWWW = Application.streamingAssetsPath;
        m_streamPathWithWWW    = Application.streamingAssetsPath;
        m_appPathWithOutWWW    = Application.persistentDataPath;
        m_appPathWithWWW       = Application.persistentDataPath;
        Debug.Log("Application.persistentDataPath :" + Application.persistentDataPath);
#elif UNITY_IPHONE
        m_streamPathWithOutWWW = Application.streamingAssetsPath;
        m_streamPathWithWWW    = Application.streamingAssetsPath;
        m_appPathWithOutWWW    = Application.persistentDataPath;
        m_appPathWithWWW       = Application.persistentDataPath;
#elif UNITY_ANDROID
        m_streamPathWithOutWWW = Application.dataPath + "!assets";
        m_streamPathWithWWW    = Application.streamingAssetsPath;
        m_appPathWithOutWWW    = Application.persistentDataPath;
        m_appPathWithWWW       = Application.persistentDataPath;
#endif

    }

    public string GetBundlePathWithOutWWW(string bundleName)
    {
        string result = m_appPathWithWWW + "/" + bundleName;
        if (!File.Exists(result))
        {
            result = m_streamPathWithOutWWW + "/" + bundleName;
        }
        return result;
    }

    private Dictionary<string, string> m_assetsDict = new Dictionary<string, string>();
    private void InitAssetsDict()
    {
        Debug.Log("InitAssetsDict");
        string bundlePath = GetBundlePathWithOutWWW("bundleinfo/assetlist.bundle");
        if (!File.Exists(bundlePath))
        {
            return;
        }

        AssetBundle assetListBundle = AssetBundle.LoadFromFile(bundlePath);
        TextAsset assetListAsset = assetListBundle.LoadAsset<TextAsset>("assetlist");
        using (StringReader sr = new StringReader(assetListAsset.text))
        {
            string line = null;
            while ((line = sr.ReadLine()) != null)
            {
                string[] split = line.Split(',');
                Debug.Log(split[0] + " " + split[1]);
                m_assetsDict.Add(split[0], split[1]);
            }

        }

    }
    private AssetBundle m_manifestAB = null;
    private AssetBundleManifest m_manifest = null;
    //assetbundle缓存，key=ab名字，已经load的ab不用再load，引用计数记录多少资源使用它
    //private Dictionary<string, AssetBundleDictValue> m_bundlesDict = new Dictionary<string, AssetBundleDictValue>();
    private void InitManifest()
    {
        Debug.Log("InitManifest");
        if (m_manifest)
        {
            return;
        }

        if (!m_manifestAB)
        {
            string bundlePath = GetBundlePathWithOutWWW("StreamingAssets");
            if (!File.Exists(bundlePath))
            {
                return;
            }
            m_manifestAB = AssetBundle.LoadFromFile(bundlePath);
        }

        if (m_manifestAB != null)
        {
            m_manifest = m_manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
    }

    public GameObject GetPrefab(string assetName)
    {
        return GetGameObject(assetName);
    }

    public void ReleasePrefab(GameObject go)
    {
        ReleaseGameObject(go);
    }

    public GameObject GetGameObject(string assetName)
    {
        GameObject obj = LoadResource<GameObject>(assetName);
        GameObject result = GameObject.Instantiate<GameObject>(obj);
        obj = null;
        return result;
    }

    public void ReleaseGameObject(GameObject go)
    {
        string assetName = go.name.Replace("(Clone)", "");
        GameObject.Destroy(go);
        go = null;
        string bundleName = null;
        if (m_assetsDict.TryGetValue(assetName, out bundleName))
        {
            ReleaseBundle(bundleName);
        }
    }

    public void ReleaseNonGameObject(Object obj)
    {
        string assetName = obj.name.Replace("(Clone)", "");
        obj = null;
        string bundleName = null;
        if (m_assetsDict.TryGetValue(assetName, out bundleName))
        {
            ReleaseBundle(bundleName);
        }
    }

    public Material GetMaterial(string assetName)
    {
        Material material = LoadResource<Material>(assetName);
        return material;
    }

    public void DestoryMaterial(Material material)
    {
        ReleaseNonGameObject(material);
    }

    public Sprite GetSprite(string assetName)
    {
        Sprite sprite = LoadResource<Sprite>(assetName);
        return sprite;
    }

    public void ReleaseSprite(Sprite sprite)
    {
        ReleaseNonGameObject(sprite);
    }

    public Texture2D GetTexture2D(string assetName)
    {
        Texture2D texture = LoadResource<Texture2D>(assetName);
        return texture;
    }

    public T LoadResource<T>(string assetName) where T : Object
    {
        T obj = null;
        string abName;
        AssetBundle bundle = null;
        if (m_assetsDict.TryGetValue(assetName, out abName))
        {
            bundle = LoadBundle(abName);
        }

        if (bundle)
        {
            obj = bundle.LoadAsset<T>(Path.GetFileName(assetName));
        }
        else
        {
            obj = Resources.Load<T>(Path.GetFileName(assetName));
        }
        return obj;
    }

    private Dictionary<string, BundleNode> m_bundleCache = new Dictionary<string, BundleNode>();
    public AssetBundle LoadBundle(string bundleName)
    {
        BundleNode bundleNode = null;
        if (!m_bundleCache.TryGetValue(bundleName, out bundleNode))
        {
            string bundlePath = GetBundlePathWithOutWWW(bundleName);
            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
            bundleNode = new BundleNode(bundle);
            m_bundleCache.Add(bundleName, bundleNode);
        }
        else
        {
            bundleNode.AddRef();
        }

        var dependencies = m_manifest.GetDirectDependencies(bundleName);
        foreach (var dependency in dependencies)
        {
            Debug.Log("bundle name: " + bundleName + " dependent by :" + dependency);
            LoadBundle(dependency);
        }

        return bundleNode.Bundle;
    }

    public void ReleaseBundle(string bundleName)
    {
        BundleNode bundleNode = null;
        if (!m_bundleCache.TryGetValue(bundleName, out bundleNode))
        {
            Debug.LogError("bundle name not exist in cache");
            return;
        }

        bundleNode.MinusDef();
        if (bundleNode.RefCount == 0)
        {
            Debug.Log(bundleName + " will be Unload(true)");
            bundleNode.Bundle.Unload(true);
            m_bundleCache.Remove(bundleName);
        }

        var dependencies = m_manifest.GetDirectDependencies(bundleName);
        foreach (var dependency in dependencies)
        {
            Debug.Log("bundle name: " + bundleName + " dependent by :" + dependency);
            ReleaseBundle(dependency);
        }
    }

    public void PrintBundleCache(int ID)
    {
        Debug.Log("begin PrintBundleCache ID:" + ID);
        foreach (KeyValuePair<string, BundleNode> kv in m_bundleCache)
        {
            Debug.Log("ID:" + ID + " bundle name: " + kv.Key + " refCount: " + kv.Value.RefCount);
        }
        Debug.Log("end PrintBundleCache ID:" + ID);
    }
    //IEnumerator OnClick()
    //{
    //    Resources.UnloadUnusedAssets();//清干净以免影响测试效果
    //    yield return new WaitForSeconds(3);
    //    float wait = 0.5f;
    //    //用www读取一个assetBundle,里面是一个Unity基本球体和带一张大贴图的材质，是一个Prefab
    //    WWW aa = new WWW(@"file://SpherePrefab.unity3d");
    //    yield return aa;
    //    AssetBundle asset = aa.assetBundle;
    //    yield return new WaitForSeconds(wait);//每步都等待0.5s以便于分析结果
    //    Texture tt = asset.LoadAsset("BallTexture") as Texture;//加载贴图
    //    yield return new WaitForSeconds(wait);
    //    GameObject ba = asset.LoadAsset("SpherePrefab") as GameObject;//加载Prefab
    //    yield return new WaitForSeconds(wait);
    //    GameObject obj1 = GameObject.Instantiate(ba) as GameObject;//生成实例
    //    yield return new WaitForSeconds(wait);
    //    GameObject.Destroy(obj1);//销毁实例
    //    yield return new WaitForSeconds(wait);
    //    asset.Unload(false);//卸载Assetbundle
    //    yield return new WaitForSeconds(wait);
    //    Resources.UnloadUnusedAssets();//卸载无用资源
    //    yield return new WaitForSeconds(wait);
    //    ba = null;//将prefab引用置为空以后卸无用载资源
    //    Resources.UnloadUnusedAssets();
    //    yield return new WaitForSeconds(wait);
    //    tt = null;//将texture引用置为空以后卸载无用资源
    //    Resources.UnloadUnusedAssets();
    //}
}



