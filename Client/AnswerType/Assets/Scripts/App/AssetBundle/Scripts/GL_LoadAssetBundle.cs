//2021.9.22 关林
//加载pad

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GL_LoadAssetBundle : GL_LoadAsset
{
    private void ParseName(string assetName, out string abName, out string resName)
    {
        //assetName, 包含 ab包名 + 资源名

        int index = assetName.LastIndexOf("/");
        abName = assetName.Substring(0, index);
        resName = assetName.Substring(index + 1, assetName.Length - index - 1); //资源名
    }
    public Object Load(string abName, string resName)
    {
        abName = abName.ToLower();
        resName = resName.ToLower();
        AssetBundleLoader loader;
        GL_LoadAssetMgr._instance._assetBundleDic.TryGetValue(abName, out loader);
        DDebug.LogError("~~~loader: " + loader);
        if (loader == null)
        {
            //资源未找到
            Debug.LogError("~~~未找到, 重新加载ab: " + abName);

            //重新加载
            loader = new AssetBundleLoader();
            loader.LoadAssetBundle(abName);
        }

        var result = loader.LoadAsset(resName, false);
        //result = null;
        DDebug.LogError("~~~result: " + result);

#if UNITY_EDITOR
        if (result == null)
        {
            //abName = string.Format("{0}/{1}", "Assets", abName);
            //var vlaue = UnityEditor.AssetDatabase.LoadAssetAtPath(abName, typeof(Object));
            //return vlaue;

            string directoryName = string.Format("{0}/{1}", "Assets", abName);
            string[] assets = UnityEditor.AssetDatabase.FindAssets(resName, new[] { directoryName });
            string assetFullPath = "";

            //获取文件名后缀
            resName += ".";
            foreach (var temp in assets)
            {
                var fullPath = UnityEditor.AssetDatabase.GUIDToAssetPath(temp);
                if (fullPath.ToLower().Contains(resName))
                {
                    assetFullPath = fullPath;
                    break;
                }
            }
            return UnityEditor.AssetDatabase.LoadAssetAtPath(assetFullPath, typeof(Object));
        }
#endif

        return result;

    }
    public override Object Load(string assetName)
    {
        string abName;
        string resName;
        ParseName(assetName, out abName, out resName);
        return Load(abName, resName);


    }

    public T Load<T>(string abName, string resName) where T : Object
    {
        return Load(abName, resName) as T;
    }

    public override T Load<T>(string assetName) 
    {
        return Load(assetName) as T;
    }


    public IEnumerator LoadAsync<T>(string abName, string resName, System.Action<T> action)
    {
        yield return null;
    }
    public override IEnumerator LoadAsync<T>(string assetName, System.Action<T> action)
    {
        yield return null;

#if UNITY_EDITOR
        string directoryName = string.Format("{0}/{1}", "Art", assetName);
        string[] assets = UnityEditor.AssetDatabase.FindAssets(assetName, new[] { directoryName });
        string assetFullPath = "";

        //获取文件名后缀
        assetName += ".";
        foreach (var temp in assets)
        {
            var fullPath = UnityEditor.AssetDatabase.GUIDToAssetPath(temp);
            if (fullPath.Contains(assetName))
            {
                assetFullPath = fullPath;
                break;
            }
        }
        var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetFullPath);
        action.Invoke(asset);
        yield return null;
#endif
    }
}
