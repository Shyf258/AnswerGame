#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：YS_Fly
// 创 建 者：Yangsong
// 创建时间：2021年12月14日 星期二 16:04
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using System.Collections.Generic;
using Logic.MainUp;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Logic.Fly
{
    /// <summary>
    /// 枚举值对应奖励获取 EItemType
    /// </summary>
    public enum EFlyItemType
    {
        Null = -1,
        None = 0,
        Coin = 1,
        Bogus = 3,
        Strength = 4,
        CostStrength,
        CashCoupon = 7
    }

    /// <summary>
    /// 飞行方法
    /// </summary>
    public class Fly_Manager : Singleton<Fly_Manager>
    {
        private Dictionary<EFlyItemType, GL_ObjectBase> _flyItemPathDic;

        //飞行物
        private List<Fly_ItemBase> _flyItems = new List<Fly_ItemBase>();

        public void Init()
        {
            _flyItemPathDic = new Dictionary<EFlyItemType, GL_ObjectBase>();
            _flyItemPathDic.Add(EFlyItemType.Coin, Resources.Load<Fly_ItemBase>(GL_ConstData.FlyPrefabPath + "Fly_Coin"));
            // _flyItemPathDic.Add(EFlyItemType.Strength, Resources.Load<Fly_ItemBase>(GL_ConstData.FlyPrefabPath + "Fly_Strength"));
            _flyItemPathDic.Add(EFlyItemType.Bogus, Resources.Load<Fly_ItemBase>(GL_ConstData.FlyPrefabPath + "Fly_Bogus"));
            // _flyItemPathDic.Add(EFlyItemType.CostStrength, Resources.Load<Fly_ItemBase>(GL_ConstData.FlyPrefabPath + "Fly_CostStrength"));
            _flyItemPathDic.Add(EFlyItemType.CashCoupon, Resources.Load<Fly_ItemBase>(GL_ConstData.FlyPrefabPath + "Fly_CashCoupon"));
        }

        public T CreateFlyItem<T>(EFlyItemType itemType, Transform parent, Vector3 bornPos, Vector3 target, Action onEnd) where T : Fly_ItemBase
        {
            if (!_flyItemPathDic.ContainsKey(itemType))
            {
                DDebug.Log("没有这个类型");
                return null;
            }

            var flyItem = GL_ResourcePool._instance.TakeObject<Fly_Item>(_flyItemPathDic[itemType]);
            var transform = flyItem.transform;
            //transform.parent = parent;
            transform.SetParent(parent, false);
            transform.localScale = Vector3.one;
            flyItem.SetPos(bornPos);
            flyItem.ToPos(target, onEnd);
            _flyItems.Add(flyItem);
            return flyItem as T;
        }

        public void DoUpdate(float dt)
        {
            for (int i = _flyItems.Count - 1; i >= 0; i--)
            {
                if (!_flyItems[i].DoUpdate(dt))
                {
                    _flyItems[i].Resource_Destory();
                    _flyItems.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 通用飞行接口
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="parent">父对象</param>
        /// <param name="born">出生位置</param>
        /// <param name="target">目标位置</param>
        /// <param name="count">数量</param>
        /// <param name="intervalTime">间隔时间</param>
        /// <param name="callback">回调</param>
        public void Fly(EFlyItemType type, Transform parent, Vector3 born, RectTransform target,
            int count, float intervalTime, Action callback = null)
        {
            var startPos = GL_Tools.UGUiToUGUIPosition(born, target);
            var targetPos = GL_Tools.UGUiToUGUIPosition(target.position, target);

            Action fun = () =>
            {
                CreateFlyItem<Fly_Item>(type, parent, startPos, targetPos, callback);
            };

            MethodExeTool.Loop((() => fun.Invoke()), intervalTime, count);
        }


        /// <summary>
        /// 新mainup飞行
        /// </summary>
        /// <param name="eFlyItemType"></param>
        /// <param name="startPos"></param>
        /// <param name="isActiveFly"></param>
        public void MainUpFly(EFlyItemType eFlyItemType,Vector3 startPos = default,bool isActiveFly = false)
        {
            if (isActiveFly)
                Fly_MainUp._.ActiveMainUp(true);
            else
                Fly_MainUp._.ActiveMainUp(false);
            GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency, 
                new EventParam<EFlyItemType,Vector3,bool>(eFlyItemType,startPos,isActiveFly));
        }
    }

}