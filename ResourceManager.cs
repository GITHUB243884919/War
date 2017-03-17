using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ResourceManager
{
	public static readonly string DOWNLOAD_DIRECTORY = Application.get_persistentDataPath() + "/download/";
	private static Dictionary<string, AssetBundleInfo> bundleCache = new Dictionary<string, AssetBundleInfo>();
	private static Dictionary<string, Object> resourceCache = new Dictionary<string, Object>();
	private static AssetBundleManifest manifest = null;
	public static bool isCompressed = false;
	private ResourceManager()
	{
	}
	public static bool Initialize()
	{
		string text = "Bundles";
		byte[] array = IOUtils.ReadFromFile(text);
		if (array != null && array.Length > 0)
		{
			AssetBundle assetBundle = AssetBundle.LoadFromMemory(array);
			ResourceManager.manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		}
		else
		{
			Debug.LogErrorFormat("manifest not found ({0})", new object[]
			{
				text
			});
		}
		return ResourceManager.manifest != null;
	}
	public static IEnumerator StartLoadResource(string path, string name, ProgressListener listener)
	{
		listener("", 0L, 1L);
		yield return null;
		yield break;
	}
	public static ResourceLoadingRequest<T> LoadResourceAsync<T>(string path, string name) where T : Object
	{
		ResourceLoadingRequest<T> result = null;
		T t = default(T);
		AssetBundle assetBundle = ResourceManager.LoadAssetBundle(path.ToLower() + ".assets", true);
		if (assetBundle != null)
		{
			AssetBundleRequest req = assetBundle.LoadAssetAsync<T>(name);
			result = new ResourceLoadingRequest<T>(req);
		}
		if (t == null)
		{
			string resourcePath = ResourceManager.GetResourcePath(path, name);
			ResourceRequest req2 = Resources.LoadAsync<T>(resourcePath);
			result = new ResourceLoadingRequest<T>(req2);
		}
		return result;
	}
	public static T LoadResource<T>(string path, string name) where T : Object
	{
		T t = default(T);
		AssetBundle assetBundle = ResourceManager.LoadAssetBundle(path.ToLower() + ".assets", true);
		if (assetBundle != null)
		{
			t = assetBundle.LoadAsset<T>(name);
		}
		if (t == null)
		{
			string resourcePath = ResourceManager.GetResourcePath(path, name);
			t = Resources.Load<T>(resourcePath);
		}
		return t;
	}
	private static AssetBundle LoadAssetBundle(string path, bool cascade = true)
	{
		AssetBundleInfo assetBundleInfo = null;
		AssetBundle result;
		if (!ResourceManager.bundleCache.TryGetValue(path, out assetBundleInfo))
		{
			byte[] array = IOUtils.ReadFromFile(path);
			if (array == null || array.Length <= 0)
			{
				result = null;
				return result;
			}
			if (cascade)
			{
				ResourceManager.LoadDependencies(path);
			}
			if (ResourceManager.isCompressed)
			{
				array = CompressUtils.DecompressLZMA(array);
			}
			AssetBundle bundle = AssetBundle.LoadFromMemory(array);
			assetBundleInfo = new AssetBundleInfo(bundle);
			ResourceManager.bundleCache.Add(path, assetBundleInfo);
		}
		else if (!cascade)
		{
			assetBundleInfo.refCounter++;
		}
		result = assetBundleInfo.bundle;
		return result;
	}
	private static void LoadDependencies(string assetBundleName)
	{
		if (ResourceManager.manifest == null)
		{
			ResourceManager.Initialize();
		}
		string[] allDependencies = ResourceManager.manifest.GetAllDependencies(assetBundleName);
		string[] array = allDependencies;
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			Debug.LogFormat("LoadDependency {0}", new object[]
			{
				text
			});
			if (ResourceManager.LoadAssetBundle(text, false) == null)
			{
				Debug.LogWarningFormat("LoadDependency {0} Failed", new object[]
				{
					text
				});
			}
		}
	}
	public static void DestroyResource(string path)
	{
		string assetBundleName = path.ToLower() + ".assets";
		ResourceManager.DestroyAssetBundle(assetBundleName, true);
		Object @object = null;
		if (ResourceManager.resourceCache.TryGetValue(path, out @object))
		{
			ResourceManager.resourceCache.Remove(path);
			Resources.UnloadUnusedAssets();
		}
	}
	private static void DestroyAssetBundle(string assetBundleName, bool cascade = true)
	{
		AssetBundleInfo assetBundleInfo = null;
		if (ResourceManager.bundleCache.TryGetValue(assetBundleName, out assetBundleInfo) && --assetBundleInfo.refCounter <= 0)
		{
			Debug.LogFormat("DestroyAssetBundle {0}", new object[]
			{
				assetBundleName
			});
			ResourceManager.bundleCache.Remove(assetBundleName);
			assetBundleInfo.bundle.Unload(true);
			if (cascade)
			{
				string[] allDependencies = ResourceManager.manifest.GetAllDependencies(assetBundleName);
				string[] array = allDependencies;
				for (int i = 0; i < array.Length; i++)
				{
					string assetBundleName2 = array[i];
					ResourceManager.DestroyAssetBundle(assetBundleName2, false);
				}
			}
		}
	}
	public static void DestroyUIResource(string path)
	{
		ResourceManager.DestroyResource("UI/" + path);
	}
	public static T LoadResource<T>(string path) where T : Object
	{
		return ResourceManager.LoadResource<T>(ResourceManager.GetDirectoryName(path), ResourceManager.GetFileName(path));
	}
	public static ResourceLoadingRequest<T> LoadResourceAsync<T>(string path) where T : Object
	{
		return ResourceManager.LoadResourceAsync<T>(ResourceManager.GetDirectoryName(path), ResourceManager.GetFileName(path));
	}
	public static Object LoadResource(string path, string name)
	{
		return ResourceManager.LoadResource<Object>(path, name);
	}
	public static Object LoadResource(string path)
	{
		return ResourceManager.LoadResource<Object>(ResourceManager.GetDirectoryName(path), ResourceManager.GetFileName(path));
	}
	public static Sprite LoadUISprite(string path)
	{
		return ResourceManager.LoadResource<Sprite>("UI/" + path);
	}
	public static GameObject LoadUIResource(string path)
	{
		return ResourceManager.LoadResource<GameObject>("UI/" + path, ResourceManager.GetFileName(path));
	}
	public static ResourceLoadingRequest<GameObject> LoadUIResourceAsync(string path)
	{
		return ResourceManager.LoadResourceAsync<GameObject>("UI/" + path, ResourceManager.GetFileName(path));
	}
	public static GameObject LoadModelResource(string path)
	{
		return ResourceManager.LoadResource<GameObject>("Model/" + path, ResourceManager.GetFileName(path));
	}
	public static ResourceLoadingRequest<GameObject> LoadModelResourceAsync(string path)
	{
		return ResourceManager.LoadResourceAsync<GameObject>("Model/" + path, ResourceManager.GetFileName(path));
	}
	public static GameObject LoadEffectResource(string path)
	{
		return ResourceManager.LoadResource<GameObject>("Effects/" + path, ResourceManager.GetFileName(path));
	}
	public static ResourceLoadingRequest<GameObject> LoadEffectResourceAsync(string path)
	{
		return ResourceManager.LoadResourceAsync<GameObject>("Effects/" + path, ResourceManager.GetFileName(path));
	}
	public static TextAsset LoadDataResource(string path)
	{
		return ResourceManager.LoadResource<TextAsset>("Data/" + path);
	}
	public static ResourceLoadingRequest<TextAsset> LoadDataResourceAsync(string path)
	{
		return ResourceManager.LoadResourceAsync<TextAsset>("Data/" + path);
	}
	public static Sprite LoadIconResource(string path)
	{
		return ResourceManager.LoadResource<Sprite>("Icon/" + path);
	}
	public static ResourceLoadingRequest<Sprite> LoadIconResourceAsync(string path)
	{
		return ResourceManager.LoadResourceAsync<Sprite>("Icon/" + path);
	}
	private static string GetResourcePath(string path, string name)
	{
		string text = path.EndsWith(name) ? ResourceManager.GetDirectoryName(path) : path;
		return string.IsNullOrEmpty(text) ? name : (text + "/" + name);
	}
	public static string GetFileName(string path)
	{
		int num = path.LastIndexOf("/");
		string result;
		if (num >= 0)
		{
			result = path.Substring(num + 1);
		}
		else
		{
			result = path;
		}
		return result;
	}
	public static string GetDirectoryName(string path)
	{
		int num = path.LastIndexOf("/");
		string result;
		if (num > 0)
		{
			result = path.Substring(0, num);
		}
		else
		{
			result = "";
		}
		return result;
	}
}
