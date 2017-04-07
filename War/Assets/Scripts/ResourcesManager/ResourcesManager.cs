using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

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
            assetListBundle = AssetBundle.LoadFromFile(assetListBundlePath);
            Debug.Log(assetListBundlePath);
        }
        else
        {
            assetListBundle = AssetBundle.LoadFromFile(assetListBundlePath);
        }
        
        if (assetListBundle == null)
        {
            Debug.Log("assetListBundle == null");
        }

        TextAsset assetListAsset = assetListBundle.LoadAsset<TextAsset>("assetlist");
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
