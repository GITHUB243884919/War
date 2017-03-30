using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

//public class ResourcesManager : MonoBehaviour
//{
//    private static ResourcesManager s_instance = null;

//    public static ResourcesManager Instance 
//    {
//        get 
//        {
//            if (s_instance == null)
//            {
//                LogMediator.LogError("没有持有ResourcesManager的对象");
//            }
//            return s_instance;
//        }
//    }

//    void Init()
//    {

//    }

//    private Dictionary<string, string> m_assetsDict = new Dictionary<string, string>();
//    private void InitAssetsDict()
//    {
//        Debug.Log("InitAssetsDict");
//        string bundlePath = GetBundlePathWithOutWWW("bundleinfo/assetlist.bundle");
//        if (!File.Exists(bundlePath))
//        {
//            return;
//        }

//        AssetBundle assetListBundle = AssetBundle.LoadFromFile(bundlePath);
//        TextAsset assetListAsset = assetListBundle.LoadAsset<TextAsset>("assetlist");
//        using (StringReader sr = new StringReader(assetListAsset.text))
//        {
//            string line = null;
//            while ((line = sr.ReadLine()) != null)
//            {
//                string[] split = line.Split(',');
//                Debug.Log(split[0] + " " + split[1]);
//                m_assetsDict.Add(split[0], split[1]);
//            }

//        }

//    }
//    void Release()
//    {
//        s_instance = null;
//    }

//    void Awake()
//    {
//        s_instance = this;
//    }

//    void OnDestroy()
//    {
//        Release();
//    }
//}

public class GetAssetFromLocalAssetBundle<T> where T : Object
{
    public static StringBuilder m_path = new StringBuilder();

    public delegate void OnAfterGetAsset(T t);
    
    /// <summary>
    /// 1.查找Asset对应的Assetbundle文件名
    /// 
    /// 2.根据Assetbundle文件名，查找AssetBundle文件
    /// (1)在缓存中查找
    /// (2)缓存中没有在应用目录查找
    /// (3)应用目录没有在Streaming目录查找
    /// (4)只要在非缓存中查找到，加载，并且存在缓存，引用计数+1
    /// (5)否则报错
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    /// 
    void GetAsset(string assetName, OnAfterGetAsset callback)
    {
        ResourcesManager.Instance.StartCoroutine(Load(assetName, callback));
    }

    IEnumerator Load(string assetName, OnAfterGetAsset callback)
    {
        //暂时将assetName 设置成path
        m_path.Remove(0, m_path.Length);
        string path = m_path.Append(assetName).ToString();
        
        var createRequest = AssetBundle.LoadFromFileAsync(
            Path.Combine(Application.streamingAssetsPath, path));
        yield return createRequest;

        var assetBundle = createRequest.assetBundle;
        if (assetBundle == null)
        {
            LogMediator.LogError("加载AssetBundle失败 " + path);
            yield break;
        }

        var bundleRequest = assetBundle.LoadAssetAsync<T>(assetName);
        yield return bundleRequest;
        if (bundleRequest.asset == null)
        {
            LogMediator.LogError("加载Asset失败 " + assetName);
            yield break;
        }

        callback(bundleRequest.asset as T);
        //T prefab = bundleRequest.asset as T;
        //GameObject.Instantiate(prefab);

        //assetBundle.Unload(false);
    }

    public string GetAssetBundleName(string assetName)
    {
        string bundleName = "";
        return bundleName;
    }

    public AssetBundle GetAssetBundle(string bundlePath)
    {
        AssetBundle bundle = null;
        return bundle;
    }

    public T GetAsset(AssetBundle assetBundle)
    {
        T t = default(T);
        return t;
    }
}
