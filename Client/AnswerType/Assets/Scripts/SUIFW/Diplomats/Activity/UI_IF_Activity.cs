#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_IF_Activity
// 创 建 者：Yangsong
// 创建时间：2022年07月29日 星期五 18:20
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System.Collections.Generic;
using System.Linq;
using DataModule;
using UnityEngine;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Main
{
    /// <summary>
    /// 活动
    /// </summary>
    public class UI_IF_Activity : BaseUIForm
    {
        #region UIField

        /// <summary> 签到列表 </summary>
        private ScrollRect _srcActivitySign;
        private Transform _tfActivitySign;

        /// <summary> 活动列表 </summary>
        private ScrollRect _srcActivity;
        private Transform _tfActivity;


        #endregion

        #region Override

        public override void Init()
        {
            CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
            CurrentUIType.UIForms_Type = UIFormType.Normal;
            CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

            _srcActivitySign = UnityHelper.GetTheChildNodeComponetScripts<ScrollRect>(gameObject, "_srcActivitySign");
            _tfActivitySign = UnityHelper.GetTheChildNodeComponetScripts<Transform>(_srcActivitySign.gameObject, "Content");
            
            _srcActivity = UnityHelper.GetTheChildNodeComponetScripts<ScrollRect>(gameObject, "_srcActivity");
            _tfActivity = UnityHelper.GetTheChildNodeComponetScripts<Transform>(_srcActivity.gameObject, "Content");
            

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
            CreateActivity();
            CreateSign();
        }

        public override void onUpdate()
        {

        }

        #endregion

        #region Event
        
        

        #endregion

        #region CustomField

        private string _activityPath = "SUIFW/Prefab/Activity/UI_Obj_Activity";
        private string _signPath = "SUIFW/Prefab/Activity/UI_Obj_ActivitySign";

        private List<UI_Obj_Activity> _activities = new List<UI_Obj_Activity>();
        private List<UI_Obj_ActivitySign> _activitySigns = new List<UI_Obj_ActivitySign>();

        #endregion

        #region Logic

        /// <summary>
        /// 创建活动
        /// </summary>
        private void CreateActivity()
        {
            if (_activities.Count > 0)
                return;

            DataModuleManager._instance.TableActivityData_Dictionary.Values.ToList().ForEach(
                delegate(TableActivityData data)
                {
                    if (data.ID != 2) //先屏蔽红包群
                    {
                        var prefab = GL_LoadAssetMgr._instance.Load<UI_Obj_Activity>(_activityPath);
                        var item = Instantiate(prefab, _tfActivity, false);
                        item.InitObjectNode();
                        item.RefreshNode(data);
                        _activities.Add(item);
                    }
                });
        }

        /// <summary>
        /// 创建签到
        /// </summary>
        private void CreateSign()
        {
            GL_PlayerData._instance.SendGamecoreConfig(EGamecoreType.ActivitySign, (() =>
            {
                var config = GL_PlayerData._instance.GetGamecoreConfig(EGamecoreType.ActivitySign);
                var list = DataModuleManager._instance.TableActivitySignData_Dictionary.Values.ToList();
                list.ForEach((
                    delegate(TableActivitySignData data)
                    {
                        if (_activitySigns.Count != list.Count)
                        {
                            var prefab = GL_LoadAssetMgr._instance.Load<UI_Obj_ActivitySign>(_signPath);
                            var item = Instantiate(prefab, _tfActivitySign, false);
                            item.InitObjectNode();
                            _activitySigns.Add(item);
                            item.RefreshNode(data,config.dayProgress);
                        }
                        else
                        {
                            if (data.ID <= _activitySigns.Count && data.ID != 0)
                                _activitySigns[data.ID - 1].RefreshNode(data,config.dayProgress);
                        }
                    }));
            }));
        }

        #endregion

    }
}