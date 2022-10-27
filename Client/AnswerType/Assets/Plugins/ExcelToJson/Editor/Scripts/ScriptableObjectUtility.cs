//2018.01.23
//asset 通用创建方法

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;

public static class ScriptableObjectUtility
{
    public static T CreateAsset<T>(string path) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        if(asset is ExcelToJson.EJ_ExportToolInfo)
        {
            (asset as ExcelToJson.EJ_ExportToolInfo).Init();
        }
        return asset;
    }
}

