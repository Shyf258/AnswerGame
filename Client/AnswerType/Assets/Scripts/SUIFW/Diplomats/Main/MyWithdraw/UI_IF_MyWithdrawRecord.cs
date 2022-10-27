#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_IF_MyWithdrawRecord
// 创 建 者：Yangsong
// 创建时间：2022年04月07日 星期四 15:28
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System.Collections.Generic;
using Logic.System.NetWork;
using UnityEngine;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Main.MyWithdraw
{
    /// <summary>
    /// 我的提现记录
    /// </summary>
    public class UI_IF_MyWithdrawRecord : BaseUIForm
    {
        #region UIField

        private Transform _srcContent;

        private Text _txtNone;

        #endregion

        #region Override

        public override void Init()
        {
            CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
            CurrentUIType.UIForms_Type = UIFormType.PopUp;
            CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;

            var scroll = UnityHelper.GetTheChildNodeComponetScripts<Transform>(gameObject, "Scroll View");
            _srcContent = UnityHelper.GetTheChildNodeComponetScripts<Transform>(scroll.gameObject, "Content");
            _txtNone = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtNone");

            RigisterButtonObjectEvent("_btnClose", (go => { OnBtnClose(); }));

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
            Create();
            // GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Banner_WithDrawRecord);
        }

        public override void onUpdate()
        {

        }

        public override void OnHide()
        {
            base.OnHide();
            // GL_AD_Interface._instance.CloseBannerAd();
        }

        #endregion

        #region Event

        /// <summary>
        /// 关闭
        /// </summary>
        private void OnBtnClose()
        {
            CloseUIForm();
        }
        

        #endregion

        #region CustomField

        private string _itemPrefabPath = "SUIFW/Prefab/Main/MyWithdraw/UI_Obj_RecordItem";

        private Dictionary<int, UI_Obj_RecordItem> _items = new Dictionary<int, UI_Obj_RecordItem>();

        #endregion

        #region Logic

        private void Create()
        {
            YS_NetLogic._instance.WithdrawRecord((config =>
            {
                var withdraws = config.withDraws;
                // WithdrawRecord withdrawRecord = new WithdrawRecord
                // {
                //     id = 1, status = 2, withDrawNum = 100, withDrawTime = 1649241271197
                // };
                // withdraws.Add(withdrawRecord);

                if (withdraws.Count <= 0)
                    _txtNone.SetActive(true);
                else
                    _txtNone.SetActive(false);

                for (int i = 0; i < withdraws.Count; i++)
                {
                    if (!_items.ContainsKey(i))
                    {
                        var prefab = GL_LoadAssetMgr._instance.Load<UI_Obj_RecordItem>(_itemPrefabPath);
                        var item = Instantiate(prefab, _srcContent, false);
                        item.InitObjectNode();
                        item.Init(withdraws[i]);
                        _items[i] = item;
                    }
                
                }
            }));
        }

        #endregion

    }
}