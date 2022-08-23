//2021.9.22 关林
//加载ab包和 pad的基类

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GL_LoadAsset
{
    public virtual void Init()
    {

    }
    public virtual Object Load(string assetName)
    {
        return Resources.Load(assetName);
    }
    public virtual T Load<T>(string assetName) where T : Object
    {
        return Resources.Load<T>(assetName);
    }

    public virtual IEnumerator LoadAsync<T>(string assetName, System.Action<T> action) where T : Object
    {
        ResourceRequest rr = Resources.LoadAsync<T>(assetName);
        yield return rr;
        action.Invoke(rr.asset as T);
    }
}
