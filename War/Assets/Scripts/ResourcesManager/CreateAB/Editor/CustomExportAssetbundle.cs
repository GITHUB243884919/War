using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
public class CustomExportAssetbundle
{
    [MenuItem("自定义菜单/生成所有Assetbundle")]
    static private void BuildAssetBundles()
    {
        //清理资源上的AB名字
        UntagAssetBundles();
        //为需要打AB的资源打上名字
        TagAssetBundles();
        
        //打出AB包
        GenAssetBundles();

        //为资源更新记录AssetBundle详单
        GenDetailFile();
    }
#if _WAR_TEST_
    [MenuItem("自定义菜单/清除Assetbundle（调试用）")]
#endif
    static private void ClearAssetBundles()
    {
        //清理资源上的AB名字
        UntagAssetBundles();
    }

    private static void UntagAssetBundles()
    {
        //AssetDatabase.RemoveUnusedAssetBundleNames();

        //TagAssetBundle.UntagInDirectory(
        //    "Assets/Resources/RuntimeMeshBaker/M_Arm_Tank", "");

        //TagAssetBundle.UntagAssetBundles(
        //    "Assets/Resources/TestAB", "");

        TagAssetBundle.UntagInDirectory(
            "Assets/Resources/RuntimeMeshBaker", "");

        AssetDatabase.RemoveUnusedAssetBundleNames();
    }
    private static void TagAssetBundles()
    {
        //TagAssetBundle.TagInDirectory(
        //    "Assets/Resources/RuntimeMeshBaker/M_Arm_Tank", "", true);

        //TagAssetBundle.TagInDirectory(
        //    "Assets/Resources/TestAB", "", true);

        TagAssetBundle.TagInDirectory(
            "Assets/Resources/RuntimeMeshBaker", "", false);

        AssetDatabase.RemoveUnusedAssetBundleNames();
    }

    private static void GenAssetBundles()
    {
        BuildAssetBundleOptions buildAssetBundleOptions =
            BuildAssetBundleOptions.DisableWriteTypeTree |
            BuildAssetBundleOptions.DeterministicAssetBundle |
            BuildAssetBundleOptions.ForceRebuildAssetBundle |
            BuildAssetBundleOptions.ChunkBasedCompression;
        //BuildTarget buildTarget = GetBuildTarget();
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        string outputPath = Application.dataPath + "/StreamingAssets";
        BuildPipeline.BuildAssetBundles(outputPath, buildAssetBundleOptions, buildTarget);
    }

    private static void GenDetailFile()
    {
        string bundleListPath = "Resources/bundlelist.txt";
        string assetListPath  = "Resources/assetlist.txt";
        FileStream fsBundleList = new FileStream(
            Application.dataPath + "/" + bundleListPath, FileMode.Create);
        FileStream fsAssetList = new FileStream(
            Application.dataPath + "/" + assetListPath, FileMode.Create);

        StreamWriter swBundleList = new StreamWriter(fsBundleList);
        StreamWriter swAssetList = new StreamWriter(fsAssetList);
        string lineBundleList = "";
        string lineAssetList = "";
        string [] bundles = AssetDatabase.GetAllAssetBundleNames();
        foreach (string bundle in bundles)
        {
            uint crc;
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
                string[] assetName = asset.Substring(7).Split('.');
                lineAssetList = string.Format("{0:S},{1:S}",
                    assetName[0], bundle);
                swAssetList.WriteLine(lineAssetList);
            }
        }

        if (swBundleList != null)
        {
            swBundleList.Dispose();
            swBundleList = null;
        }
        if (fsBundleList != null)
        {
            fsBundleList.Dispose();
            fsBundleList = null;
        }

        if (swAssetList != null)
        {
            swAssetList.Dispose();
            swAssetList = null;
        }
        if (fsAssetList != null)
        {
            fsAssetList.Dispose();
            fsAssetList = null;
        }

        //把详单打成AB
        //Debug.Log("bundleListPath:" + bundleListPath);
        //var importer = AssetImporter.GetAtPath("Assets/" + bundleListPath);
        //importer.assetBundleName = "bundleinfo/bundlelist.bundle";
        //var importer2 = AssetImporter.GetAtPath("Assets/" + assetListPath);
        //importer2.assetBundleName = "bundleinfo/assetlist.bundle";
        //BuildPipeline.BuildAssetBundles(
        //    tagetPath, BuildAssetBundleOptions.UncompressedAssetBundle 
        //    | BuildAssetBundleOptions.CollectDependencies, 
        //    GetBuildTarget());
    }

    //    static private BuildTarget GetBuildTarget()
    //    {
    //        BuildTarget target = BuildTarget.StandaloneWindows;
    //#if UNITY_STANDALONE
    //        target = BuildTarget.StandaloneWindows;
    //#elif UNITY_IPHONE
    //            target = BuildTarget.iPhone;
    //#elif UNITY_ANDROID
    //            target = BuildTarget.Android;
    //#endif
    //        return target;
    //    }
}
