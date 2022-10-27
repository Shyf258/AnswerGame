//2021.10.11    关林
//assetbunlde包加载

//2021.9.27 关林
//PAD游戏资源包下载

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using GL.Assetbundle;


public class AssetBundleLoader
{
    public string _assetBundleName;
    public AssetBundle _assetBundle;

    private AssetBundleManifest _manifest;
    // 缓存集合
    protected Hashtable _ht = new Hashtable();

    //预加载
    //public virtual void  PreLoad(string assetBundleName)
    //{
    //    _assetBundleName = assetBundleName;
    //    DownloadAssetBundle();
    //}

    public AssetBundle LoadAssetBundle(string assetBundleName)
    {
        _assetBundleName = assetBundleName;
        _assetBundle = AssetbundleLoad.LoadAssetBundleDependcy(assetBundleName);

        if(_assetBundle != null)
            GL_LoadAssetMgr._instance.AddAssetBundle(this);

        return _assetBundle;
    }


    #region 加载指定资源
    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="assetName">资源名称</param>
    /// <param name="isCache">是否缓存</param>
    /// <returns></returns>
    public Object LoadAsset(string assetName, bool isCache = false)
    {
        return LoadResource<Object>(assetName, isCache);
    }


    /// <summary>
    /// 加载资源
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    /// <param name="assetName">资源名称</param>
    /// <param name="isCache">是否缓存</param>
    /// <returns></returns>
    private T LoadResource<T>(string assetName, bool isCache) where T : Object
    {

        // 是否缓存集合中已存在
        if (_ht.Contains(assetName))
        {
            return _ht[assetName] as T;
        }
        if (_assetBundle == null)
            return null;
        //int assetNameStart = assetName.LastIndexOf("/") + 1;
        //int assetNameEnd = assetName.Length;
        //string aName = assetName.Substring(assetNameStart, assetNameEnd - assetNameStart);
        // 没有，则加载
        T result = _assetBundle.LoadAsset<T>(assetName);

        // 根据要求是否保存到缓存中
        if (result != null && isCache == true)
        {
            _ht.Add(assetName, result);
        }
        else if (result == null)
        {
            Debug.LogError(GetType() + "/LoadResource<T>()/ 加载资源为空，请检查 参数 assetName = " + assetName);
        }

        return result;
    }
    #endregion

    #region 释放
    /// <summary>
    /// 卸载指定资源
    /// </summary>
    /// <param name="asset">资源</param>
    /// <returns></returns>
    public bool UnLoadAsset(Object asset)
    {
        if (asset != null)
        {
            Resources.UnloadAsset(asset);
            return true;
        }

        Debug.LogError(GetType() + "/UnLoadAsset()/参数 asset is null,请检查");

        return false;
    }

    /// <summary>
    /// 卸载内存镜像资源
    /// </summary>
    public void Dispose()
    {
        _assetBundle.Unload(false);
    }

    /// <summary>
    /// 释放当前 AssetBundle 内存镜像资源，且释放内存资源
    /// 所有引用都释放
    /// </summary>
    public void DisposeAll()
    {
        _assetBundle.Unload(true);
    }
    #endregion

    #region 接口

    //是否下载完毕
    public virtual bool IsDownloaded()
    {
        return true;
    }
    #endregion
}
