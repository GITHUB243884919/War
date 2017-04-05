using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

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
        string assetBundleName = GetAssetBundleName(assetName);
        if (string.IsNullOrEmpty(assetBundleName))
        {
            LogMediator.LogError("资源没有找到对应的assetbundle " + assetName);
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
                LogMediator.LogError("加载Asset失败 " + assetName);
                yield break;
            }

            callback(bundleRequest.asset as T);
            yield break;
        }
        
        m_path.Remove(0, m_path.Length);
        m_path.Append(Application.persistentDataPath);
        m_path.Append("/");
        m_path.Append(assetName);

        string path = null;
        path = m_path.ToString();
        if (!File.Exists(m_path.ToString()))
        {
            m_path.Remove(0, m_path.Length);
            m_path.Append(Application.streamingAssetsPath);
            m_path.Append("/");
            m_path.Append(assetName);
        }
        path = m_path.ToString();
        
        var createRequest = AssetBundle.LoadFromFileAsync(path);
        yield return createRequest;

        assetBundle = createRequest.assetBundle;
        if (assetBundle == null)
        {
            LogMediator.LogError("加载AssetBundle失败 " + path);
            yield break;
        }
        ResourcesManager.Instance.m_assetBundles.Add(assetBundleName, assetBundle);

        bundleRequest = assetBundle.LoadAssetAsync<T>(assetName);
        yield return bundleRequest;
        if (bundleRequest.asset == null)
        {
            LogMediator.LogError("加载Asset失败 " + assetName);
            yield break;
        }

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
