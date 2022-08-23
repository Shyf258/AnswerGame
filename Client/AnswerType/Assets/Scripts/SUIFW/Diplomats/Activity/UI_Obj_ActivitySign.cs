#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_Obj_ActivitySign
// 创 建 者：Yangsong
// 创建时间：2022年07月29日 星期五 18:45
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using DataModule;
using SUIFW.Diplomats.Common;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Main
{
    /// <summary>
    /// 活动签到对象
    /// </summary>
    public class UI_Obj_ActivitySign : UIObjectBase
    {
        #region UIField

        /// <summary> 金币 </summary>
        private Text _txtCoin;

        /// <summary> 第几天 </summary>
        private Text _txtDay;

        /// <summary> 签到按钮 </summary>
        private UI_Button _btn;
        
        #endregion

        #region Override

        public override void InitObjectNode()
        {
            _txtCoin = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtCoin");
            _txtDay = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtDay");
            _btn = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(gameObject, "_btn");
            RigisterButtonObjectEvent(_btn,(go => OnBtnItem()));
        }

        public void RefreshNode(TableActivitySignData data,int signProgress)
        {
            _txtCoin.text = data.Reward.ToString();
            int curDay = GL_CoreData._instance.ActivitySignDay + 1;
            if (signProgress != 0 && GL_CoreData._instance.ActivitySignDay != 0) //规避已签到清除缓存
                curDay = GL_CoreData._instance.ActivitySignDay;
            if (curDay != 7)
                curDay %= 7;
            _txtDay.text = $"第{data.ID}天";
            SetSignState(curDay == data.ID);
        }

        #endregion

        #region Event

        private void OnBtnItem()
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.ActivitySignClick);
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_ActivitySign);

            // //记录签到日期
            // DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            // double signTime = GL_Time._instance.CalculateSeconds(dt);
            // GL_CoreData._instance.ActivitySignSeconds = signTime;
        }

        #endregion

        #region CustomField

        
        
        

        #endregion

        #region Logic

        /// <summary>
        /// 设置签到状态
        /// </summary>
        private void SetSignState(bool isCanSign)
        {
            _txtDay.SetActive(!isCanSign);
            _btn.SetActive(isCanSign);
        }

        // private void InitSignTime()
        // {
        //     double signTime = GL_CoreData._instance.ActivitySignSeconds;
        //     DateTime lastSign = GL_Time._instance.TimestampToDateTime(signTime);
        //     TimeSpan ts = DateTime.Now - lastSign;
        //     if(ts.Days > 1 && GL_CoreData._instance.ActivitySignDay < 7) //超过一天
        //     {
        //         GL_CoreData._instance.IsActivitySigned = false;
        //     }
        // }

        #endregion
    }
}