// using System;
// using System.Collections;
// using System.Collections.Generic;
// using SUIFW;
// using SUIFW.Diplomats.Common;
// using UnityEngine;
// using UnityEngine.UI;
//
// public class UI_IF_NewLogin : BaseUIForm
// {
//    
//     #region 对象
//
//     /// <summary>
//     /// 已登录天数
//     /// </summary>
//     private Text _dayNumber;
//     /// <summary>
//     /// 登录提现提示
//     /// </summary>
//     private Text _loginTips;
//     
//     /// <summary>
//     /// 提现天数要求
//     /// </summary>
//     private Text _textDescription;
//     /// <summary>
//     /// 每日提现金额
//     /// </summary>
//     private Text _rewardCountEvery;
//     /// <summary>
//     /// 每日提现按键
//     /// </summary>
//     private Button _btnEveryDay;
//     /// <summary>
//     /// 每日提现金额
//     /// </summary>
//     private Text _withDrawCountEveryDay;
//     
//     
//     /// <summary>
//     /// 连续登录提现金额
//     /// </summary>
//     private Text _rewardCountLogin;
//     /// <summary>
//     /// 累计登录提现按键
//     /// </summary>
//     private Button _btnLoginDay;
//     /// <summary>
//     /// 累计天差距天数
//     /// </summary>
//     private Text _dayGap;
//     /// <summary>
//     /// 累计提现金额
//     /// </summary>
//     private Text _withDrawCountLogin;
//     
//     #endregion
//
//
//     #region 参数
//
//     private List<string> _listDescription = new List<string>()
//     {
//         "{1}<size=72>天</size>",
//         "差{0}天可额外提现",
//         "每登录<color=#ff0000>5</color>天，可额外提现<color=#ff0000>{0}</color>元",
//         "{0}元",
//         "{0}元提现",
//     };
//
//     /// <summary>
//     /// 登录配置
//     /// </summary>
//     // private Net_CB_LoginConfig _netCbLoginConfig;
//     
//     /// <summary>
//     /// 提现配置
//     /// </summary>
//     private Net_CB_WithDraw _netCbWithDraw = new Net_CB_WithDraw();
//
//     #endregion
//     
//     public override void Init()
//     {
//         this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
//         this.CurrentUIType.UIForms_Type = UIFormType.PopUp;
//         this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;
//         
//         RigisterButtonObjectEvent("Btn_Back", go =>
//         {
//             CloseUIForm();
//         });
//
//         _dayNumber = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "DayNumber");
//         _loginTips = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "LoginDescription");
//         Transform _top = UnityHelper.FindTheChildNode(gameObject, "TopFrame");
//
//         _rewardCountEvery = UnityHelper.GetTheChildNodeComponetScripts<Text>(_top.gameObject, "RewardCount");
//         _btnEveryDay = UnityHelper.GetTheChildNodeComponetScripts<Button>(_top.gameObject, "Btn_WithDraw");        
//         RigisterButtonObjectEvent(_btnEveryDay, go =>
//         {
//             WithDrawOnClick(WithDrawType.EveryDay);
//         });
//         _withDrawCountEveryDay = UnityHelper.GetTheChildNodeComponetScripts<Text>(_btnEveryDay.gameObject, "Text");
//         Transform _bottom = UnityHelper.FindTheChildNode(gameObject, "BottomFrame");
//
//         _rewardCountLogin = UnityHelper.GetTheChildNodeComponetScripts<Text>(_bottom.gameObject, "RewardCount");
//         _btnLoginDay = UnityHelper.GetTheChildNodeComponetScripts<Button>(_bottom.gameObject, "Btn_WithDraw");
//         RigisterButtonObjectEvent(_btnLoginDay, go =>
//         {
//             WithDrawOnClick(WithDrawType.Login);
//         });
//         _withDrawCountLogin = UnityHelper.GetTheChildNodeComponetScripts<Text>(_btnLoginDay.gameObject, "Text");
//         _dayGap = UnityHelper.GetTheChildNodeComponetScripts<Text>(_bottom.gameObject, "Text_Description");
//     }
//     public override void Refresh(bool recall)
//     { 
//         GL_PlayerData._instance.SendLoginWithDraw(() =>
//         {
//             RefreshPage();
//         });
//     }
//
//     public override void onUpdate()
//     {
//        
//     }
//
//     private void RefreshPage()
//     {
//         _netCbLoginConfig = GL_PlayerData._instance._NetCbLoginConfig;
//         
//         //登录天数
//         _dayNumber.text = String.Format(_listDescription[0], _netCbLoginConfig.day);
//         //累计登录天数提醒
//         _loginTips.text = String.Format(_listDescription[2], _netCbLoginConfig.withdraws[1].needDay);
//         
//         //每日登录
//         
//         //按键文字每日提现
//         _withDrawCountEveryDay.text = String.Format(_listDescription[4],
//             ( _netCbLoginConfig.withdraws[0].money/100f).ToString("0.00"));
//         
//         //图标提现金额显示
//         _rewardCountEvery.text = String.Format(_listDescription[3],
//             ( _netCbLoginConfig.withdraws[0].money/100f).ToString("0.00"));
//         
//         //按键可交互开关
//         _btnEveryDay.interactable =  _netCbLoginConfig.withdraws[0].withDrawLimit > 0;
//         
//         
//         //累计登录
//         
//         //按键文字累计提现
//         _withDrawCountLogin.text = String.Format(_listDescription[4],
//             ( _netCbLoginConfig.withdraws[1].money/100f).ToString("0.00"));
//         
//         //图标提现金额显示
//         _rewardCountLogin.text = String.Format(_listDescription[3],
//             ( _netCbLoginConfig.withdraws[1].money/100f).ToString("0.00"));
//         
//         //累计登录天数差距提醒
//         int day = _netCbLoginConfig.withdraws[1].needDay - _netCbLoginConfig.day ;
//         if (day>0)
//         {
//           
//             _dayGap.text =  String.Format(_listDescription[1],day);
//         }
//         else
//         {
//             _dayGap.text = "已获得额外提现机会!";
//         }
//         //按键可交互开关
//         if (_netCbLoginConfig.withdraws[1].withDrawLimit<=0)
//         {
//             _dayGap.SetActive(false);
//             _btnLoginDay.interactable = false;
//         }
//         else
//         {
//             _dayGap.SetActive(true);
//             _btnLoginDay.interactable = true;
//         }
//     }
//
//     private void WithDrawOnClick(WithDrawType drawType)
//     {
//       _netCbWithDraw = new Net_CB_WithDraw();
//         switch (drawType)
//         {
//             case WithDrawType.EveryDay:
//                 _netCbWithDraw = _netCbLoginConfig.withdraws[0];
//                 break;
//             case WithDrawType.Login:
//                 _netCbWithDraw = _netCbLoginConfig.withdraws[1];
//                 break;
//             default:
//                 break;
//         }
//
//         if (_netCbWithDraw.withDrawLimit <=0)
//         {
//             return;
//         }
//         
//         if (GL_PlayerData._instance._NetCbLoginConfig.viewAds < _netCbWithDraw.needAd)
//         {
//             UI_HintMessage._.ShowMessage($"今日再浏览{_netCbWithDraw.needAd-GL_PlayerData._instance._NetCbLoginConfig.viewAds}次视频即可提现哦！");
//         }
//         else
//         {
//             CloseUIForm();
//             WithDraw();
//         }
//         
//     }
//
//     private void WithDraw()
//     {
//
//         GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_LoginWithDraw, (show=>
//         {
//             if (show)
//             {
//                 //提现广告播放成功
//                 Net_WithDraw draw = new Net_WithDraw();
//                 draw.withDrawId =  _netCbWithDraw.id;
//                 draw.type = 2;
//                 draw.withDrawType = (int) EWithDrawType.LoginWithDraw;
//                 
//                 GL_ServerCommunication._instance.Send(Cmd.WithDraw, JsonUtility.ToJson(draw), CB_WithDraw);
//             }
//             else
//             {
//                 UI_HintMessage._.ShowMessage("广告播放失败，请重新观看！");
//             }
//         }));
//         
//    
//     }
//     
//     private void CB_WithDraw(string param)
//     {
//         GL_PlayerData._instance.Net_CB_WithDrawResult(param);
//         float money = _netCbWithDraw.money * 0.01f;
//         EWithDrawType _eWithDrawType = EWithDrawType.Normal;
//         var obj = new object[]
//         {
//             money,
//             _eWithDrawType,
//             GL_PlayerData._instance._netCbWithDraw.money
//         };
//         UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WithdrawSuccess, obj);
//     }
//
//     enum WithDrawType
//     {
//      EveryDay,
//      Login,
//     }
//
// }
