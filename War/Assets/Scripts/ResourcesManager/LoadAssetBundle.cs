using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

#if _WAR_TEST_
using Debug = LogMediator;
#endif

public class GetAssetFromLocalAssetBundle<T> where T : Object
{
    public static StringBuilder m_path = new StringBuilder();
    //public static StringBuilder m_assetName = new StringBuilder();

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
    public void GetAsset(string assetName, OnAfterGetAsset callback)
    {
        LogMediator.Log("assetName " + assetName);
        ResourcesManager.Instance.StartCoroutine(Load(assetName, callback));
    }

    IEnumerator Load(string assetName, OnAfterGetAsset callback)
    {
        Debug.Log("assetName " + assetName);
        string assetBundleName = GetAssetBundleName(assetName);
        Debug.Log("assetBundleName " + assetBundleName);
        if (string.IsNullOrEmpty(assetBundleName))
        {
            //Debug.LogError("资源没有找到对应的assetbundle " + assetName);
            Debug.LogError("资源没有找到对应的assetbundle " + assetName);
            yield break;
        }

        AssetBundle assetBundle = null;
        ResourcesManager.Instance.m_assetBundles.
            TryGetValue(assetBundleName, out assetBundle);
        AssetBundleRequest bundleRequest;
        if (assetBundle != null)
        {
            bundleRequest = assetBundle.LoadAssetAsync<T>(assetName);
            yield return bundleRequest;
            if (bundleRequest.asset == null)
            {
                //Debug.LogError("加载Asset失败 " + assetName);
                Debug.LogError("加载Asset失败 " + assetName);
                yield break;
            }

            callback(bundleRequest.asset as T);
            yield break;
        }
        
        m_path.Remove(0, m_path.Length);
        m_path.Append(Application.persistentDataPath);
        m_path.Append("/");
        m_path.Append(assetBundleName);

        string path = null;
        path = m_path.ToString();
        if (!File.Exists(path))
        {
            //Debug.Log("persistentDataPath 目录不存在 " + path);
            Debug.Log("persistentDataPath 目录不存在 " + path);
            m_path.Remove(0, m_path.Length);
            m_path.Append(Application.streamingAssetsPath);
            m_path.Append("/");
            m_path.Append(assetBundleName);
        }
        path = m_path.ToString();
        
        var createRequest = AssetBundle.LoadFromFileAsync(path);
        yield return createRequest;
        assetBundle = createRequest.assetBundle;
        if (assetBundle == null)
        {
            //Debug.LogError("加载AssetBundle失败 " + path);
            Debug.LogError("加载AssetBundle失败 " + path);
            yield break;
        }
        //Debug.Log("加载AssetBundle成功 " + path);
        LogMediator.Log("加载AssetBundle成功 " + path);

        ResourcesManager.Instance.m_assetBundles.Add(assetBundleName, assetBundle);
        assetName = Path.GetFileNameWithoutExtension(assetName);
        Debug.Log("assetName " + assetName);
        bundleRequest = assetBundle.LoadAssetAsync<T>(assetName);
        yield return bundleRequest;
        if (bundleRequest.asset == null)
        {
            Debug.LogError("加载Asset失败 " + assetName);
            yield break;
        }
        Debug.Log("加载Asset成功 " + assetName);
        callback(bundleRequest.asset as T);
    }

    public string GetAssetBundleName(string assetName)
    {
        string bundleName = null;
        ResourcesManager.Instance.
            m_assets.TryGetValue(assetName, out bundleName);
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
