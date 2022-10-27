// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Logic.System.NetWork;
// using UnityEngine;
// using SUIFW;
// using SUIFW.Diplomats.Common;
// using UnityEditor;
// using UnityEngine.Serialization;
// using UnityEngine.UI;
//
// public partial class UI_IF_Game : BaseUIForm
// {
//     private Text _levelText;
//
//     private Transform _gameArea;    //游戏区
//     private GridLayoutGroup _selectArea;    //选择区
//
//     private UI_Button _btnRen;
//     private Text _txtCash;
//     
//     //计时
//     private Image _uiImgCountdown;
//     private Text _uiTxtCountdown;
//     
//     //免费提示
//     private Text _txtFreeTip;
//
//     private int _time;
//     
//     
//     public override void Init()
//     {
//         this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
//         this.CurrentUIType.UIForms_Type = UIFormType.Normal;
//         this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Light;
//         
//         var countdown = UnityHelper.FindTheChildNode(gameObject, "_SliderCountdown");
//         _uiImgCountdown = UnityHelper.GetTheChildNodeComponetScripts<Image>(countdown.gameObject, "Fill");
//         _uiTxtCountdown = UnityHelper.GetTheChildNodeComponetScripts<Text>(countdown.gameObject, "FillText");
//         
//         _btnRen = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(gameObject, "BtnRen");
//         RigisterButtonObjectEvent(_btnRen, (go) => OnBtnRen());
//         _txtCash = UnityHelper.GetTheChildNodeComponetScripts<Text>(_btnRen.gameObject, "Text");
//
//         _levelText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "LevelText");
//         _gameArea = UnityHelper.FindTheChildNode(gameObject, "GameArea");
//         _selectArea = UnityHelper.FindTheChildNode(gameObject, "SelectArea").GetComponentInChildren<GridLayoutGroup>();
//         
//         var btnFreeTip = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(gameObject, "BtnFreeTips");
//         //RigisterButtonObjectEvent(btnFreeTip, (go) => OnBtnFreeTip());
//         _txtFreeTip = UnityHelper.GetTheChildNodeComponetScripts<Text>(btnFreeTip.gameObject, "Text");
//
//         //RigisterButtonObjectEvent("BtnBlack", (go) => OnClickBlack());
//         //RigisterButtonObjectEvent("BtnRestart", (go) => OnClickRestart());
//
//     }
//
//     public override void InitData(object data)
//     {
//         base.InitData(data);
//         
//     }
//
//     //private void StartCountdown()
//     //{
//     //    _time = GL_SceneManager._instance.CurGameMode.Timer;
//     //    GL_SceneManager._instance.CurGameMode.CountDown.StartCountdown(_time,0,0,(() =>
//     //    {
//     //        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Fail);
//     //    }),_uiTxtCountdown);
//         
//     //}
//
//
//     void SetCanvas()
//     {
//         var canvas = this.GetComponent<Canvas>();
//         if (canvas != null)
//         {
//             canvas.overrideSorting = true;
//             canvas.sortingOrder = 1;
//         }
//     }
//
//     public override void onUpdate()
//     {
//         float vaule = float.Parse(_uiTxtCountdown.text) / _time;
//         _uiImgCountdown.fillAmount = vaule;
//
//     }
//
//     public override void Refresh(bool recall)
//     {
//         //刷新游戏区域
//         //GL_SceneManager._instance.CurGameMode.SetActive(true);
//         //GL_Tools.TransformMakeZero(GL_SceneManager._instance.CurGameMode.transform, _gameArea);
//
//         ////刷新选择区域
//         ////_selectArea.enabled = true;
//         ////foreach (var word in GL_SceneManager._instance.CurGameMode._selectWordList)
//         ////{
//         ////    GL_Tools.TransformMakeZero(word.transform, _selectArea.transform);
//         ////}
//         ////强制刷新后, 隐藏排序组件
//         //LayoutRebuilder.ForceRebuildLayoutImmediate(_selectArea.transform as RectTransform);
//         //_selectArea.enabled = false;
//
//         //GL_GameEvent._instance.RegisterEvent(EEventID.RefreshCurrency, RegisterCurrencyCallBack);
//         //RegisterCurrencyCallBack(null);
//         
//         //YS_NetLogic._instance.FreeTipConfig((() =>
//         //{
//         //    GL_GameEvent._instance.RegisterEvent(EEventID.RefreshFreeTip, RegisterFreeTipCallBack);
//         //    RegisterFreeTipCallBack(null);
//         //}));
//         
//         
//         ////StartCountdown();
//
//         //_levelText.text = "第{0}关";
//         //int level = GL_PlayerData._instance.CurLevel;
//         //_levelText.text = String.Format(_levelText.text,level);
//         
//         //GL_GameEvent._instance.RegisterEvent(EEventID.RefreshBogus, RegisterBogusCallBack);
//         //RegisterBogusCallBack(null);
//
//     }
//
//     public override void OnHide()
//     {
//         base.OnHide();
//
//         GL_GameEvent._instance.UnregisterEvent(EEventID.RefreshCurrency, RegisterCurrencyCallBack);
//         GL_GameEvent._instance.UnregisterEvent(EEventID.RefreshFreeTip, RegisterFreeTipCallBack);
//         GL_GameEvent._instance.UnregisterEvent(EEventID.RefreshBogus, RegisterBogusCallBack);
//
//         GL_AD_Interface._instance.CloseBannerAd();
//     }
//     
//     private void RegisterCurrencyCallBack(EventParam param)
//     {
//         _btnRen.GetComponentInChildren<Text>().text = GL_PlayerData._instance.Bogus + "元";
//     }
//     
//     
//     private void RegisterBogusCallBack(EventParam param)
//     {
//         _txtCash.text = GL_PlayerData._instance.Bogus + "元";
//     }
//     
//     
//     private void RegisterFreeTipCallBack(EventParam param)
//     {
//         _txtFreeTip.text = GL_PlayerData._instance.FreeTipCount.ToString();
//     }
//
//     //返回主页
//     //private void OnClickBlack()
//     //{
//     //    CloseUIForm();
//     //    GL_Game._instance.DoLevelExit();   
//     //}
//
//     ////刷新关卡
//     //private void OnClickRestart()
//     //{
//     //    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Game_Again);
//
//     //    GL_Game._instance.DoRestartLevel();
//     //}
//
//     private void OnBtnRen()
//     {
//         UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Cash);
//     }
//
//     //private void OnBtnFreeTip()
//     //{
//         
//
//     //    if (GL_PlayerData._instance.FreeTipCount <= 0)
//     //    {
//     //        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Game_TipAd);
//     //        UI_HintMessage._.ShowMessage(transform,"免费提示次数不足");
//     //        GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_GameTip, CB_AD_Tip);
//     //    }
//     //    else
//     //    {
//     //        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Game_Tip);
//     //        CB_AD_Tip(false);
//     //    }
//     //}
//
//     //private void CB_AD_Tip(bool isSuccess)
//     //{
//     //    YS_NetLogic._instance.UseFreeTip((() =>
//     //    {
//     //        GL_SceneManager._instance.CurGameMode.Tip();
//     //        GL_GameEvent._instance.SendEvent(EEventID.RefreshFreeTip);
//     //    }));
//     //}
//
//     public override void RefreshLanguage()
//     {
//     }
// }
