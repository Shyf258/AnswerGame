using System;
using Logic.Fly;
using UnityEngine;

namespace Logic.MainUp
{
    /// <summary>
    /// 货币飞行接口
    /// </summary>
    public interface IF_MainUp
    {
        RectTransform GetCoinTargetPos();

        RectTransform GetBogusTargetPos();

        RectTransform GetFlyRootPos();
    }
}