using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Mono.Xml;
using System.Security;
using System.Text;
public class MakeAtlas
{
    [MenuItem("自定义菜单/AtlasMaker")]
    static private void CreateAtlasPrefab()
    {
        Debug.Log("CreateAtlasPrefab");
        string spriteDir = Application.dataPath + "/Resources/Sprite";

        if (!Directory.Exists(spriteDir))
        {
            Directory.CreateDirectory(spriteDir);
        }

        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Atlas");
        foreach (DirectoryInfo dirInfo in rootDirInfo.GetDirectories())
        {
            foreach (FileInfo pngFile in dirInfo.GetFiles("*.png", SearchOption.AllDirectories))
            {
                string allPath = pngFile.FullName;
                string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                GameObject go = new GameObject(sprite.name);
                go.AddComponent<SpriteRenderer>().sprite = sprite;
                allPath = spriteDir + "/" + sprite.name + ".prefab";
                string prefabPath = allPath.Substring(allPath.IndexOf("Assets"));
                PrefabUtility.CreatePrefab(prefabPath, go);
                GameObject.DestroyImmediate(go);
            }
        }	
    }

    [MenuItem("自定义菜单/Build Assetbundle")]
    static private void BuildAssetBundle()
    {
        string dir = Application.dataPath + "/StreamingAssets";

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Atlas");
        foreach (DirectoryInfo dirInfo in rootDirInfo.GetDirectories())
        {
            List<Sprite> assets = new List<Sprite>();
            string path = dir + "/" + dirInfo.Name + ".assetbundle";
            foreach (FileInfo pngFile in dirInfo.GetFiles("*.png", SearchOption.AllDirectories))
            {
                string allPath = pngFile.FullName;
                string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                assets.Add(AssetDatabase.LoadAssetAtPath<Sprite>(assetPath));
            }

            BuildPipeline.BuildAssetBundle(null, assets.ToArray(), path, BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.CollectDependencies, GetBuildTarget());
        }
    }
    [MenuItem("自定义菜单/Build Assetbundle2")]
    static private void BuildAssetBundle2()
    {
        string tagetPath = Application.dataPath + "/StreamingAssets";
        BuildPipeline.BuildAssetBundles(tagetPath, BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.CollectDependencies, GetBuildTarget());
    }

    [MenuItem("自定义菜单/Build Assetbundle3")]
    static private void BuildAssetBundle3()
    {
        string tagetPath = Application.dataPath + "/StreamingAssets";
        BuildPipeline.BuildAssetBundles(tagetPath, BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.CollectDependencies, GetBuildTarget());
       
        string       bundleListPath     = "Resources/bundlelist.txt";
        string       assetListPath      = "Resources/assetlist.txt";
        FileStream fsBundleList = new FileStream(Application.dataPath + "/" + bundleListPath, FileMode.Create);
        FileStream fsAssetList = new FileStream(Application.dataPath + "/" + assetListPath, FileMode.Create);
        StreamWriter swBundleList       = new StreamWriter(fsBundleList);
        StreamWriter swAssetList        = new StreamWriter(fsAssetList);
        string       lineBundleList     = "";
        string       lineAssetList      = "";
        var bundles = AssetDatabase.GetAllAssetBundleNames();
        foreach (var bundle in bundles)
        {
            uint    crc;
            BuildPipeline.GetCRCForAssetBundle(Application.dataPath + "/StreamingAssets/" + bundle, out crc);
            Hash128 hash;
            BuildPipeline.GetHashForAssetBundle(Application.dataPath + "/StreamingAssets/" + bundle, out hash);
            
            Debug.Log("AssetBundle: " + bundle + " CRC: " + crc + " Hash:" + hash);
            lineBundleList = string.Format("{0:S},{1:S},{2:S}", bundle.ToString(), crc.ToString(), hash.ToString());
            //Debug.Log("format " + string.Format("%s,%s,%s,1", bundle.ToString(), crc.ToString(), hash.ToString()));
            swBundleList.WriteLine(lineBundleList);
            
            var assets = AssetDatabase.GetAssetPathsFromAssetBundle(bundle);
            foreach (var asset in assets)
            {
                Debug.Log("Asset :" + asset);
                string [] assetName = asset.Substring(7).Split('.');
                lineAssetList = string.Format("{0:S},{1:S}",
                    assetName[0], bundle);
                swAssetList.WriteLine(lineAssetList);
                
            }
        }

        if (swBundleList != null)
        {
            swBundleList.Dispose();
        }
        if (fsBundleList != null)
        {
            fsBundleList.Dispose();
        }

        if (swAssetList != null)
        {
            swAssetList.Dispose();
        }
        if (fsAssetList != null)
        {
            fsAssetList.Dispose();
        }

        Debug.Log("bundleListPath:" + bundleListPath);
        var importer = AssetImporter.GetAtPath("Assets/" + bundleListPath);
        importer.assetBundleName = "bundleinfo/bundlelist.bundle";
        var importer2 = AssetImporter.GetAtPath("Assets/" + assetListPath);
        importer2.assetBundleName = "bundleinfo/assetlist.bundle";
        BuildPipeline.BuildAssetBundles(tagetPath, BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.CollectDependencies, GetBuildTarget());

    }

    [MenuItem("自定义菜单/Test Monoxml")]
    static private void TestMonoxml()
    {
        SecurityParser SP = new SecurityParser();

        // 假设xml文件路径为 Resources/test.xml
        //string xmlPath = "test";
       // TextAsset xmlAsset = Resources.Load<TextAsset>(xmlPath);
        //SP.LoadXml(xmlAsset.text);
        //SP.LoadXml(Resources.Load(xmlPath).ToString());
        //FileStream file = new FileStream(Application.dataPath + "/Resources/text.xml", FileMode.Open);
        string path = Application.dataPath + "/Resources/test.xml";
        StreamReader sr = new StreamReader(path);
        string text = sr.ReadToEnd();
        Debug.Log("test:" + text);
        SP.LoadXml(text);
        SecurityElement SE = SP.ToXml();
        Debug.Log("root :" + SE.Tag);
        SecurityElement aaaa;
        foreach (SecurityElement child in SE.Children)
        {
            //if (child.Tag == "Windows")
            {
                //获得节点得属性
                string name = child.Attribute("name");
                string script = child.Attribute("script");
                Debug.Log("child Tag1:" + child.Tag + " text:" + child.Text + " name:" + name + " script:" + script);
               
            }
        }

    }

    static private BuildTarget GetBuildTarget()
    {
        BuildTarget target = BuildTarget.WebPlayer;
#if UNITY_STANDALONE
        target = BuildTarget.StandaloneWindows;
#elif UNITY_IPHONE
			target = BuildTarget.iPhone;
#elif UNITY_ANDROID
			target = BuildTarget.Android;
#endif
        return target;
    }
}

