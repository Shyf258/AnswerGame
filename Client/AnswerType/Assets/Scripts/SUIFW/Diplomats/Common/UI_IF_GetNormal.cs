using System;
using System.Collections;
using System.Collections.Generic;
using Logic.Fly;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

public class UI_IF_GetNormal : BaseUIForm
{
    private List<string> _titleTextList = new List<string>()
    {
        "转圈红包",
        "任务奖励",
    };

    private List<string> _buttonTextList = new List<string>()
    {
        "领双份奖励",
        "立即领取",
    };

    private List<string> _offTextList = new List<string>()
    {
        "只领{0}红包",
        "放弃领取",
        "只领{0}金币"
    };

    #region 对象

    private Text _title;
    private Transform _coinReward;
    private Text _coinRewardText;

    private Transform _redReward;
    private Text _redRewardText;


    private ERewardSource _redpackType;
    private Action _callback;
    //private Action _action;
    
    private Text _sureText;
    private Text _onlyRed;

    #endregion

    public override void Init()
    {
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;

        _title = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "TitleText");
        _coinReward = UnityHelper.FindTheChildNode(gameObject, "CoinReward");
        _coinRewardText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_coinReward.gameObject, "RewardText");
        _redReward = UnityHelper.FindTheChildNode(gameObject, "RedReward");
        _redRewardText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_redReward.gameObject, "RewardText");

        _sureText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "SureText");
        _onlyRed = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "OnlyRedGet");

        RigisterButtonObjectEvent("BtnSure", (go) =>
        {
            OnClickSure();
        });

        RigisterButtonObjectEvent("BtnClose", (go) =>
        {
            OnClickClose();
        });
    }

    private void OnClickSure()
    {
        if (_redpackType== ERewardSource.DragRedpack)
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.TurnGetRed);
        }
      
        GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_NormalGetCoin, (set) =>
        {
            if(set)
            {
                if (_redpackType== ERewardSource.DragRedpack)
                {
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.TurnRedGetFinish);
                }
                CloseUIForm();
                
                if (_redpackType == ERewardSource.TaskReward)
                {
                    if (coin>0)
                    {
                        GL_PlayerData._instance.Coin += coin;
                        GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency,new EventParam<EFlyItemType>(EFlyItemType.Coin));
                        UI_HintMessage._.ShowMessage($"恭喜获得{coin}金币");
                    }

                    if (bogus>0)
                    {
                         GL_PlayerData._instance.Bogus += bogus;
                         GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency,new EventParam<EFlyItemType>(EFlyItemType.Bogus));
                         UI_HintMessage._.ShowMessage($"恭喜获得{bogus/100f}元");
                    }
                    
                   
                    _callback?.Invoke();
                    _callback = null;
                    return;
                }
                
                GL_PlayerData._instance.SendGetVideoRedpack((EVideoRedpackType)_redpackType, true, (msg) =>
                {
                    _callback?.Invoke();
                    _callback = null;

                    object[] datas = { msg.rewards, null };
                    UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetResult, datas);
                });
                //if (_action != null)
                //{
                //    _action?.Invoke();
                //    _action = null;
                //}
            }
        });
    }

    private void OnClickClose()
    {
        CloseUIForm();
        if(_redpackType == ERewardSource.DragRedpack)
        {
            GL_PlayerData._instance.SendGetVideoRedpack((EVideoRedpackType)_redpackType, false, (msg) =>
            {
                _callback?.Invoke();
                _callback = null;
                object[] datas = { msg.rewards, null };
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetResult, datas);
            });
        }
    }


    private int coin;
    private int bogus;
    public override void InitData(object data)
    {
        base.InitData(data);

        var datas = data as object[];
        if (datas == null)
            return;
        //奖励
        if (datas.Length > 0 && datas[0] is ERewardSource type)
        {
            _redpackType = type;
            if (_redpackType == ERewardSource.DragRedpack)
            {
                _title.text = _titleTextList[0];
                _sureText.text = _buttonTextList[0];
                _onlyRed.text = _offTextList[0];
            }
            else
            {
                _title.text = _titleTextList[1];
                _sureText.text = _buttonTextList[1];
                _onlyRed.text = _offTextList[1];
            }
        }
        if (datas.Length > 1 && datas[1] is int value1)
        {
            coin = value1;
            _coinReward.SetActive(value1 > 0);
            _coinRewardText.text = "+" + value1;
        }
        if (datas.Length > 2 && datas[2] is int value2)
        {
            bogus = value2;
            _redReward.SetActive(value2 > 0);
            _redRewardText.text = "+" + value2 * 1000;
        }
        if (datas.Length > 3 && datas[3] is int value3)
        {
            if(_redpackType == ERewardSource.DragRedpack)
            {
                _onlyRed.text = string.Format(_onlyRed.text, value3 * 1000);

                if (bogus <=0)
                {
                    _onlyRed.text = String.Format(_offTextList[2],value3);
                }
            }
        }
        if (datas.Length > 4 && datas[4] is Action action)
        {
            _callback = action;
        }
        //if (datas.Length > 5 && datas[5] is Action actionGet)
        //{
        //    _action = actionGet;
        //}
    }

    public override void onUpdate()
    {

    }
    //public override void OnHide()
    //{
    //    base.OnHide();
    //    _callback?.Invoke();
    //    _callback = null;
    //}
    public override void Refresh(bool recall)
    {
        //var data = GL_PlayerData._instance.GetVideoRedpackConfig(_redpackType);
        
        //if(_redpackType == EVideoRedpackType.)

    }
}
