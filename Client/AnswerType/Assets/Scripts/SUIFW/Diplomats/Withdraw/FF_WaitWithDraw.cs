using System;
using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

public class FF_WaitWithDraw : UI_BaseItem
{
    private Image _sliderBar;

    private Text _sliderText;
    
    private Button _button;

    private Image _btnImage;
    
    private Text _btnText;
    /// <summary>
    /// 条件描述
    /// </summary>
    private Text _demand;

    /// <summary>
    /// 奖励
    /// </summary>
    private Text _reward;

    #region 显示信息

    private string _demandStr = "累计玩{0}日，可提{1}倍";

    private string _rewardStr = "可提：{0}元";

    public List<Sprite> _spriteList;

    private WithDrawWaitConfig _config;
    
    #endregion
    public override void Init()
    {
        base.Init();

        _sliderBar = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "SliderBar");
        
        _sliderText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "SliderText");

        _button = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "Button");

        _btnImage = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "Button");
        
        _btnText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Button");
        
        _demand = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Demand");

        _reward = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Reward");

        _button.onClick.AddListener(WithDraw);
        
    }

    public override void Refresh<T>(T data, int dataIndex)
    {
        base.Refresh(data, dataIndex);

        if (data is WithDrawWaitConfig config)
        {
            _config = config;
            Show(config);
        }
    }

    private void Show(WithDrawWaitConfig config)
    {
        _sliderBar.fillAmount = (float) GL_PlayerData._instance.BankConfig.nowDay / config.targetDayCount;

        _sliderText.text = $"{GL_PlayerData._instance.BankConfig.nowDay}/{config.targetDayCount}";

        if (config.canWithDraw)
        {
            _btnImage.sprite = _spriteList[0];
            // _btnText.text = "立即提现";
        }
        else
        {
            _btnImage.sprite = _spriteList[1];
            // _btnText.text = "已提现";
        }
        
        _demand.text = string.Format(_demandStr,config.targetDayCount,config.multiple);

        _reward.text = string.Format(_rewardStr, (GL_PlayerData._instance.BankConfig.nowMoney * config.multiple));
    }

    private void WithDraw()
    {
 
        if (_config.canWithDraw)
        {

            if (GL_PlayerData._instance.BankConfig.nowDay<_config.targetDayCount)
            {
                UI_HintMessage._.ShowMessage($"您共登录了{GL_PlayerData._instance.BankConfig.nowDay}天，\n" +
                                             $"再登录{_config.targetDayCount-GL_PlayerData._instance.BankConfig.nowDay}天即可提现" +
                                             $"{(GL_PlayerData._instance.BankConfig.nowMoney * _config.multiple)}元"); 
                return;
            }
            
            if ((GL_PlayerData._instance.BankConfig.nowMoney * _config.multiple)<0.3f)
            {
                UI_HintMessage._.ShowMessage("最低0.3元可提现！");
            }
            else
            {
                Action<int> action = i =>
                {
                    if (i == 2)
                    {
                        GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_WaitWithDraw, (set) =>
                        {
                            if (set)
                            {
                                _btnImage.sprite = _spriteList[1];
                                GL_PlayerData._instance.BankConfig.bankConfig[transform.GetSiblingIndex()].canWithDraw =
                                    false;
                                float money = GL_PlayerData._instance.BankConfig.nowMoney * _config.multiple;
                                EWithDrawType _eWithDrawType = EWithDrawType.WaitWithDraw;
                                var obj = new object[]
                                {
                                    money,
                                    _eWithDrawType
                                };
                                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WithdrawSuccess, obj);
                            }
                        });

                    }
                };

                int index = transform.GetSiblingIndex()+1;
                DDebug.LogError("序号"+ index);

                if (index<GL_PlayerData._instance.BankConfig.bankConfig.Count)
                {
                    WithDrawWaitConfig config = GL_PlayerData._instance.BankConfig.bankConfig[index];

                    var objWait = new object[]
                    {
                        config,
                        action,
                    };
                    UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WaitTips, objWait);
                }
                else
                {
                    action?.Invoke(2);
                }
            }
        }
        else
        {
            UI_HintMessage._.ShowMessage("当前档位已提现完成");
        }
    }
}
