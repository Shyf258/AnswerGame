#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_IF_ActivitySign
// 创 建 者：Yangsong
// 创建时间：2022年08月01日 星期一 20:42
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using DataModule;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Main
{
    /// <summary>
    /// 活动签到页
    /// </summary>
    public class UI_IF_ActivitySign : BaseUIForm
    {
        #region UIField

        /// <summary> 签到 </summary>
        private Text _txtSign;

        /// <summary> 签到按钮 </summary>
        private UI_Button _btn;

        private Transform _tfSignParent;

        #endregion

        #region Override

        public override void Init()
        {
            CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
            CurrentUIType.UIForms_Type = UIFormType.PopUp;
            CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;

            _txtSign = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtSign");
            _btn = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(gameObject, "_btnGet");
            RigisterButtonObjectEvent(_btn,(go => OnBtnSign()));
            RigisterButtonObjectEvent("_btnClose",(go => OnBtnClose()));
            _tfSignParent = UnityHelper.GetTheChildNodeComponetScripts<Transform>(gameObject, "Content");
        }

        public override void InitData(object data)
        {
            base.InitData(data);
            
            
        }

        public override void RefreshLanguage()
        {

        }

        public override void Refresh(bool recall)
        {
            RefreshSign();
            UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_DragRedpack);
        }

        public override void onUpdate()
        {

        }

        #endregion

        #region Event

        /// <summary>
        /// 签到
        /// </summary>
        private void OnBtnSign()
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.ActivitySignGet);
            GL_PlayerData._instance.SendGamecoreConfig(EGamecoreType.ActivitySign, (() =>
            {
                var config = GL_PlayerData._instance.GetGamecoreConfig(EGamecoreType.ActivitySign);
                if (config.dayProgress != 0)
                {
                    UI_HintMessage._.ShowMessage("今日已签到");
                }
                else
                {
                    if (_nextTime<=0)
                    {
                        _nextTime = Time.time;
                    }
                    else
                    {
                        float offsetTime = Time.time - _nextTime;
                        if (offsetTime > config.intervalTime)
                        {
                            _nextTime = Time.time;
                        }
                        else
                        {
                            int second = (int)(config.intervalTime - offsetTime);
                            UI_HintMessage._.ShowMessage($"请在{second}秒后在进行领取");
                            return;
                        }
                    }
                    
                    GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_ActivitySign,(success =>
                    {
                        if (success)
                        {
                            GL_PlayerData._instance.SendGamecoreAccept(EGamecoreType.ActivitySign,0,(accept =>
                            {
                                Action action = () =>
                                {
                                    UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_DragRedpack);
                                };
                                object[] datas = { accept.rewards,action,false,true};
                                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetResult,datas);
                                
                                GL_CoreData._instance.ActivitySignDay += 1;
                                if (GL_CoreData._instance.ActivitySignDay == 7)
                                    GL_CoreData._instance.ActivitySignDay = 0;
                                RefreshSign();
                                UI_HintMessage._.ShowMessage("签到成功");
                            }));
                        }
                    }));
                }
            }));
        }
        
        private void OnBtnClose()
        {
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_DragRedpack);
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.ActivitySignClose);
           CloseUIForm();
        }

        #endregion

        #region CustomField

        private float _nextTime;

        #endregion

        #region Logic

        /// <summary>
        /// 刷新签到
        /// </summary>
        private void RefreshSign()
        {
            GL_PlayerData._instance.SendGamecoreConfig(EGamecoreType.ActivitySign, (() =>
            {
                var config = GL_PlayerData._instance.GetGamecoreConfig(EGamecoreType.ActivitySign);
                var list = DataModuleManager._instance.TableActivitySignData_Dictionary.Values.ToList();
                list.ForEach((
                    delegate(TableActivitySignData data)
                    {
                        var item = _tfSignParent.GetChild(data.ID - 1).GetComponent<UI_Obj_ActivitySubSign>();
                        item.InitObjectNode();
                        int curDay = GL_CoreData._instance.ActivitySignDay + 1;
                        if (config.dayProgress != 0 && GL_CoreData._instance.ActivitySignDay != 0)
                            curDay = GL_CoreData._instance.ActivitySignDay;
                        if (curDay != 7)
                            curDay %= 7;
                        item.RefreshNode(data,curDay,config.dayProgress);
                        if (curDay == data.ID)
                            _txtSign.text = $"今日签到+{data.Reward}金币";
                    }));
            }));
        }

        #endregion

    }
}