//2021.9.22 关林
//打包后, 资源加载管理器

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GL_LoadAssetMgr : Singleton<GL_LoadAssetMgr>
{
    //加载类
    private GL_LoadAsset _loadAsset;
    private GL_LoadAssetBundle _loadAssetAB;    //ab加载对象
    public Dictionary<string, AssetBundleLoader> _assetBundleDic = new Dictionary<string, AssetBundleLoader>();

    public void Init()
    {
        _loadAsset = new GL_LoadAsset();
        _loadAssetAB = new GL_LoadAssetBundle();
    }

    #region Resources加载
    public object Load(string name)
    {
        return _loadAsset.Load(name);
    }


    public T Load<T>(string name) where T : Object
    {
        return _loadAsset.Load<T>(name);
    }

    public void LoadAsync<T>(string name, System.Action<T> action) where T : Object
    {
        MethodExeTool.StartCoroutine(_loadAsset.LoadAsync(name, action));
    }
    #endregion

    #region AssetBundle加载
    //加载整体包中的 某个文件
    //一般用于不需要依赖的单独资源
    public object LoadAB(string abName, string resName)
    {
        return _loadAssetAB.Load(abName, resName);
    }
    //加载单独包中的文件
    public object LoadAB(string assetBundleName)
    {
        return _loadAssetAB.Load(assetBundleName);
    }

    public T LoadAB<T>(string abName, string resName) where T : Object
    {
        return LoadAB(abName, resName) as T;
    }
    public T LoadAB<T>(string assetBundleName) where T : Object
    {
        return LoadAB(assetBundleName) as T;
    }
    public void LoadABAsync<T>(string abName, string resName, System.Action<T> action) where T : Object
    {
        MethodExeTool.StartCoroutine(_loadAssetAB.LoadAsync(abName, resName, action));
    }

    public void LoadABAsync<T>(string assetBundleName, System.Action<T> action) where T : Object
    {
        MethodExeTool.StartCoroutine(_loadAssetAB.LoadAsync(assetBundleName, action));
    }

    public void AddAssetBundle(AssetBundleLoader ab)
    {
        Debug.LogError("~~~加载完毕ab包: " + ab._assetBundle.name);
        _assetBundleDic.Add(ab._assetBundleName, ab);
    }
    #endregion

}
