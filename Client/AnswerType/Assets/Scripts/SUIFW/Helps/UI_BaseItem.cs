using UnityEngine;

public class UI_BaseItem : MonoBehaviour
{
    protected bool _isInit = false;
    public virtual void Refresh<T>(T data, int dataIndex)
    {

    }

    public virtual void Init()
    {

    }
}
