using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class TagAssetBundle
{
    public static void TagInDirectory(
        string path, string filter = "", bool useFileName = false)
    {
        //Debug.LogFormat("Build Texture Bundle {0}", path);
        string assetName = GenAssetBundleName(path);
        var files = GetAssetFiles(path, filter);

        EditorUtility.DisplayProgressBar(assetName, path, 0.0f / files.Length);

        int counter = 0;

        foreach (var file in files)
        {
            if (useFileName)
            {
                var filename = GenAssetBundleName(
                    file.Substring(0, file.LastIndexOf(".")));
                assetName = GenAssetBundleName(filename);
            }
            TagOneAssetBundle(file, assetName, "");
            EditorUtility.DisplayProgressBar("Build Asset Bundle (" + assetName + ")", file, (float)++counter / files.Length);
        }


        var dirs = AssetDatabase.GetSubFolders(path);
        foreach (var dir in dirs)
        {
            TagInDirectory(dir, filter, useFileName);
        }

    }

    private static string GenAssetBundleName(string path)
    {
        return path.Replace("Assets/", "").
            Replace("Resources/", "").ToLower();
    }

    private static string[] GetAssetFiles(string path, string filter)
    {
        List<string> results = new List<string>();
        var files = Directory.GetFiles(
            path, "*" + filter + ".meta", SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            var filepath = file.Replace(".meta", "");
            var fileinfo = new FileInfo(filepath);

            if (fileinfo.Attributes != FileAttributes.Directory 
                && fileinfo.Attributes != FileAttributes.Hidden 
                && !filepath.EndsWith(".cs") 
                //&& !filepath.EndsWith(".mat") 
                && !filepath.EndsWith(".fnt"))
            {
                results.Add(filepath);
            }
        }

        return results.ToArray();
    }

    private static void TagOneAssetBundle(string filepath, string assetName, string variantName)
    {

        var importer = AssetImporter.GetAtPath(filepath);
        if (!assetName.Equals(importer.assetBundleName))
        {
            importer.assetBundleName = assetName;
            if (!string.IsNullOrEmpty(variantName))
            {
                importer.assetBundleVariant = variantName;
            }
            importer.SaveAndReimport();
        }
    }

    public static void UntagAssetBundles(string path, string filter)
    {
        Debug.LogFormat("Clear Bundle {0}", path);

        var files = GetAssetFiles(path, filter);

        EditorUtility.DisplayProgressBar("Clear Bundle ", path, 0.0f / files.Length);

        int counter = 0;

        foreach (var file in files)
        {
            TagOneAssetBundle(file, "", "");
            EditorUtility.DisplayProgressBar("Clear Bundle " + path, file, ++counter / files.Length);
        }

        var dirs = AssetDatabase.GetSubFolders(path);
        foreach (var dir in dirs)
        {
            UntagAssetBundles(dir, filter);
        }

    }
}
