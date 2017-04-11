using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

#if _WAR_TEST_
using Debug = LogMediator;
#endif

public class ResourcesManager : MonoBehaviour
{
    private static ResourcesManager s_instance = null;

    private string assetListBundlePath = null;

    public Dictionary<string, string> m_assets 
        = new Dictionary<string, string>();

    public Dictionary<string, AssetBundle> m_assetBundles
        = new Dictionary<string, AssetBundle>();

    public static ResourcesManager Instance
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
        LoadAssetList();
        //StartCoroutine(CoLoadAssetList());
    }

    private void LoadAssetList()
    {
        Debug.Log("LoadAssetList");

        assetListBundlePath = Application.persistentDataPath + "/assetlist.bundle";
        Debug.Log(assetListBundlePath);
        AssetBundle assetListBundle = null;
        if (!File.Exists(assetListBundlePath))
        {
            assetListBundlePath = Application.streamingAssetsPath + "/bundleinfo/assetlist.bundle";
            //assetListBundle = AssetBundle.LoadFromFile(assetListBundlePath);
            //Debug.Log(assetListBundlePath);
            assetListBundlePath = Application.dataPath+"!assets/bundleinfo/assetlist.bundle";
        }
        //else
        //{
        //    assetListBundle = AssetBundle.LoadFromFile(assetListBundlePath);
        //}
#if UNITY_EDITOR
        assetListBundlePath = Application.dataPath + "/StreamingAssets/" + "bundleinfo/assetlist.bundle";
#elif UNITY_ANDROID
        assetListBundlePath = Application.dataPath + "!assets/bundleinfo/assetlist.bundle";
#endif

        assetListBundle = AssetBundle.LoadFromFile(assetListBundlePath);
        Debug.Log("assetListBundle " + (assetListBundle != null));
        if (assetListBundle == null)
        {
            Debug.Log("no co assetListBundle == null " + assetListBundlePath);
        }

        TextAsset assetListAsset = assetListBundle.LoadAsset<TextAsset>("assetlist");
        //Debug.Log("assetListAsset " + (assetListAsset != null));
        using (StringReader sr = new StringReader(assetListAsset.text))
        {
            string line = null;
            while ((line = sr.ReadLine()) != null)
            {
                string[] split = line.Split(',');
                //Debug.Log(split[0] + " " + split[1]);
                m_assets.Add(split[0], split[1]);
            }
        }
    }

    public IEnumerator CoLoadAssetList()
    {
        Debug.Log("CoLoadAssetList");

        //assetListBundlePath = Application.persistentDataPath + "/assetlist.bundle";
        //Debug.Log(assetListBundlePath);
        AssetBundle assetListBundle = null;
        //if (!File.Exists(assetListBundlePath))
        {

            //assetListBundlePath = Application.streamingAssetsPath + "/bundleinfo/assetlist.bundle";
//#if UNITY_EDITOR
            //assetListBundlePath = "file:///" + Application.streamingAssetsPath + "/bundleinfo/assetlist.bundle";
//#elif UNITY_ANDROID 
//#else
            assetListBundlePath = "jar:file:/" + Application.dataPath + "!/assets/" + "bundleinfo/assetlist.bundle";  
//#endif
        }
        Debug.Log(assetListBundlePath);
        using (WWW www = new WWW(assetListBundlePath))
        {
            
            if (www.error != null)
            {
                Debug.Log("WWW download had an error:" + www.error);
                yield break;
            }
            yield return www;
            assetListBundle = www.assetBundle;
            //if (AssetName == "")
            //    Instantiate(bundle.mainAsset);
            //else
            //    Instantiate(bundle.LoadAsset(AssetName));
            // Unload the AssetBundles compressed contents to conserve memory
            //bundle.Unload(false);

            if (assetListBundle == null)
            {
                Debug.Log("co assetListBundle == null " + assetListBundlePath);
            }

            TextAsset assetListAsset = assetListBundle.LoadAsset<TextAsset>("assetlist");
            assetListBundle.Unload(false);
            Debug.Log("assetListAsset  " + (assetListAsset != null));
            using (StringReader sr = new StringReader(assetListAsset.text))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] split = line.Split(',');
                    //Debug.Log(split[0] + " " + split[1]);
                    m_assets.Add(split[0], split[1]);
                }
            }

            yield return www;

        } 


        yield return null;

    }

    void Release()
    {
        s_instance = null;
    }

    void Awake()
    {
        s_instance = this;
    }

    void OnDestroy()
    {
        Release();
    }
}
