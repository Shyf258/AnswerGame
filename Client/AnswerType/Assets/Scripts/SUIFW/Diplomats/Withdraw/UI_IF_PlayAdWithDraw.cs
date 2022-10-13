using System;
using System.Collections;
using System.Collections.Generic;
using Logic.Fly;
using Logic.System.NetWork;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

public class UI_IF_PlayAdWithDraw : BaseUIForm
{
    private Button _btnWithDraw;
    private Net_CB_WithDraw _withDrawData;
    private Text _textWithDraw;
    public override void Init()
    {
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForms_Type = UIFormType.Topside;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        _textWithDraw = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "WithDrawText");
        _btnWithDraw = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "BtnSure");
        RigisterButtonObjectEvent(_btnWithDraw, go =>
        {
            WithDraw();
            CloseUIForm();
        });
        RigisterButtonObjectEvent("BtnClose", go =>
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDrawTipClose);
            GL_SDK._instance.CoolDown(GL_PlayerData._instance._withDrawTarget[EWithDrawType.TipsPage].withDrawLimit);
            CloseUIForm();
        });
    }
    public override void RefreshLanguage()
    {
        
    }

    public override void Refresh(bool recall)
    {
        _withDrawData = GL_PlayerData._instance._withDrawTarget[EWithDrawType.TipsPage];
    }

    public override void onUpdate()
    {
       
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    private void WithDraw()
    {
        DDebug.LogError("******当前提现要求ecpm:"+_withDrawData.ecpm+
                        " 当前获取到的ecpm:"+ Convert.ToInt32(GL_CoreData._instance._adECPM) +
                        "  当前reqAdId："+ GL_CoreData._instance._adReqAdid);
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDrawTipClick);
        //提现广告
        GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_WithDrawCoin, (set) =>
        {
            if (set)
            {
                GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDrawTipFinish);
                if ( Convert.ToInt32(GL_CoreData._instance._adECPM)>_withDrawData.ecpm)
                {
                    //提现广告播放成功
                    Net_WithDraw draw = new Net_WithDraw();
                    draw.withDrawId = _withDrawData.id;
                    draw.type = 2;
                    draw.withDrawType = (int) EWithDrawType.TipsPage;
                    draw.ecpm = Convert.ToInt32(GL_CoreData._instance._adECPM);
                    draw.reqAdId = GL_CoreData._instance._adReqAdid;
                
                    GL_ServerCommunication._instance.Send(Cmd.WithDraw, JsonUtility.ToJson(draw), CB_WithDraw);
                    // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.ClickWalletWithdraw);
                    // if(set)
                    //     GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.AD_WithdrawCallback,"true");
                    // else
                    //     GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.AD_WithdrawCallback,"false");
                }
                else
                {
                    YS_NetLogic._instance.UpgradeDouble(GL_PlayerData._instance.CurLevel,3, (rewards =>
                    {
                        switch ((EItemType)rewards.type)
                        {
                            case EItemType.Coin:
                                //获取奖励
                                GL_PlayerData._instance.Coin += rewards.num;
                                GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency, new EventParam<EFlyItemType>(EFlyItemType.Coin));
                                UI_HintMessage._.ShowMessage($"恭喜！获得{rewards.num}金币");
                                break;
                        }
                    }));
                }
            }
            else
            {
                UI_HintMessage._.ShowMessage("广告观看失败，请重新观看");
            }
            
            GL_SDK._instance.CoolDown(GL_PlayerData._instance._withDrawTarget[EWithDrawType.TipsPage].withDrawLimit);
        });
        
    }

    
    //提现回调
    private void CB_WithDraw(string param)
    {
        GL_PlayerData._instance.Net_CB_WithDrawResult(param);
        float money = _withDrawData.money * 0.01f;
        EWithDrawType _eWithDrawType = EWithDrawType.Normal;
        var obj = new object[]
        {
            money,
            _eWithDrawType,
            GL_PlayerData._instance._netCbWithDraw.money
        };
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WithdrawSuccess, obj);
        GL_PlayerData._instance._withDrawTarget[EWithDrawType.TipsPage].withDrawLimit--;
        
        
        // GL_PlayerData._instance.Dcoupon -= _withDrawData.needDcoupon;
        // GL_PlayerData._instance.Coin -= _withDrawData.coupon;
        // //GL_PlayerData._instance._showWithDraw = true;
        // _withDrawData.withDrawLimit = 0;
        // UI_IF_Withdraw with = UIManager.GetInstance().GetUI(SysDefine.UI_Path_Withdraw) as UI_IF_Withdraw;
        // with?.RefreshData();
        // GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency);
        
        // if (_withDrawType == EWithDrawType.DailyWithDraw)
        // {
        //     GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.CoinDailySuccess);
        // }
        
    }
    
}
