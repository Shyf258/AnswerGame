#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：Fly_MainUp
// 创 建 者：Yangsong
// 创建时间：2021年12月14日 星期二 17:01
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using System.Collections.Generic;
using Helpser;
using Logic.Fly;
using SUIFW;
using UnityEngine;

namespace Logic.MainUp
{
    /// <summary>
    /// 顶部货币飞行
    /// </summary>
    public sealed class Fly_MainUp : IF_MainUp
    {
        public static Fly_MainUp _;

        public static Fly_MainUp Init(BaseUIForm baseUIForm)
        {
            if (_ == null)
            {
                _ = new Fly_MainUp();
                _._objMainUp = (UI_IF_MainUp)baseUIForm;
            }
            
            return _;
        }

        /// <summary> 创建间隔时间 </summary>
        private float _ct = 0.2f;
        
        private UI_IF_MainUp _objMainUp;
        
        //mainFly飞行计数
        public int _mainUpFlyCount;
        
        private Vector3 _coinTargetPos;
        private Vector3 _strengthTargetPos;
        private Vector3 _bogusTargetPos;
        private Vector3 _cashCouponTargetPos;

        //飞行个数计数
        private int _coinHitCount;
        private int _bogusHitCount;
        private int _strengthHitCount;

        public void InitData()
        {
            _coinTargetPos = GL_Tools.UGUiToUGUIPosition(GetCoinTargetPos().position, GetCoinTargetPos());
            _bogusTargetPos = GL_Tools.UGUiToUGUIPosition(GetBogusTargetPos().position, GetBogusTargetPos());
        }

        public UI_IF_MainUp GetMainUp()
        {
            return _objMainUp;
        }
        
        /// <summary>
        /// 是否能关闭mainup
        /// </summary>
        /// <returns></returns>
        private bool IsCanCloseMainUp()
        {
            return _mainUpFlyCount <= 0 ? true : false;
        }

        /// <summary>
        /// 减少飞行计数
        /// </summary>
        private void ReduceMainUpFlyCount()
        {
            if (_mainUpFlyCount > 0) _mainUpFlyCount -= 1;
        }
        
        /// <summary>
        /// 激活mainup
        /// </summary>
        public void ActiveMainUp(bool isActiveFly)
        {
            if (isActiveFly)
            {
                _mainUpFlyCount += 1;
            }
            
            if (!_objMainUp.isActiveAndEnabled)
            {
                _objMainUp.SetActive(true);
            }
        }

        /// <summary>
        /// 关闭Mainup
        /// </summary>
        private void CloseMainUp()
        {
            ReduceMainUpFlyCount();
            
            if (_objMainUp.isActiveAndEnabled && UIManager.GetInstance().GetMain()._withdrawPageToggle.isOn || 
                UIManager.GetInstance().GetUI(SysDefine.UI_Path_TurnTable).isActiveAndEnabled)
            {
                if (IsCanCloseMainUp())
                {
                    _objMainUp.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 飞行金币
        /// </summary>
        /// <param name="count"></param>
        /// <param name="bornPos"></param>
        /// <param name="isActivityMainUp"></param>
        public void FlyCoin(int count, Vector3 bornPos = default,bool isActivityMainUp = false)
        {
            _coinHitCount = 0;
            MethodExeTool.Loop(()=> { CreateCoin(count,bornPos,isActivityMainUp); }, _ct, count);
        }

        /// <summary>
        /// 飞行现金
        /// </summary>
        /// <param name="count"></param>
        /// <param name="bornPos"></param>
        /// <param name="isActivityMainUp"></param>
        public void FlyBogus(int count, Vector3 bornPos = default,bool isActivityMainUp = false)
        {
            _bogusHitCount = 0;
            MethodExeTool.Loop(()=> { CreateBogus(count,bornPos,isActivityMainUp); }, _ct, count);
        }

        public void CreateCoin(int count,Vector3 bornPos,bool isActivityMainUp = false)
        {
            if (bornPos == default)
                bornPos = Vector3.zero;
            
            var startPos = GL_Tools.UGUiToUGUIPosition(bornPos, GetCoinTargetPos());
            
            Fly_Manager._instance.CreateFlyItem<Fly_Item>(EFlyItemType.Coin,
                GetCoinTargetPos(), startPos, _coinTargetPos,(delegate
                {
                    if (isActivityMainUp)
                    {
                        _coinHitCount++;
                        if (_coinHitCount == count)
                            CloseMainUp();
                        DDebug.Log($"!!!!{_coinHitCount}---{count}");
                    }
                }));
        }
        
        public void CreateBogus(int count,Vector3 bornPos,bool isActivityMainUp = false)
        {
            if (bornPos == default)
                bornPos = Vector3.zero;
            
            var startPos = GL_Tools.UGUiToUGUIPosition(bornPos, GetBogusTargetPos());
            
            Fly_Manager._instance.CreateFlyItem<Fly_Item>(EFlyItemType.Bogus,
                GetBogusTargetPos(), startPos, _bogusTargetPos,(delegate
                {
                    if (isActivityMainUp)
                    {
                        _bogusHitCount++;
                        if (_bogusHitCount == count)
                            CloseMainUp();
                    }
                }));
        }

        public RectTransform GetCoinTargetPos()
        {
            return _objMainUp._coinFlyTarget;
        }

        public RectTransform GetBogusTargetPos()
        {
            return _objMainUp._bogusFlyTarget;
        }

        public RectTransform GetFlyRootPos()
        {
            return _objMainUp._flyRoot;
        }
    }
}