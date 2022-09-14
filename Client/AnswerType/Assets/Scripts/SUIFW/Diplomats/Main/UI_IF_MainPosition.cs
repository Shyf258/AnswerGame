//2021.12.3 关林
//官职的 UI逻辑

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SUIFW;
using Spine.Unity;
using DataModule;
using System;
using Logic.Fly;
using Logic.System.NetWork;
using SUIFW.Diplomats.Common;

public partial class UI_IF_Main
{
    //进度部分
    private List<string> _explainList = new List<string>()
    {
        "还差<color=#fdff3c>{0}关</color>即可领取奖励",
    };

    private Text _contentText;  //内容
    private Image _fillImage;   //进度条图片
    private Text _fillText;     //进度条内容
    private Transform _fillEffect;  //进度条特效

    private Button _btnSlider;    //进度条按钮
    private Image _buttonIcon;    //进度条按钮 图标
    private Text _buttonText;     //进度条按钮文本
    private ParticleSystem _positionEffect;  //按钮特效

    private ParticleSystem _brokenEffect;
    
    private Net_CB_MilestoneConfigList _milestoneConfig => GL_PlayerData._instance._milestoneConfig;    //里程碑信息
    private int _curMilestoneInfoIndex; 

    private Net_CB_WithDraw _withDraw;

    private Transform _tfLevelSlider;
    
    public void InitPosition()
    {
        _tfLevelSlider = UnityHelper.FindTheChildNode(gameObject, "SmallLevelSlider");
        _contentText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_tfLevelSlider.gameObject, "Text");
        _fillImage = UnityHelper.GetTheChildNodeComponetScripts<Image>(_tfLevelSlider.gameObject, "Fill");
        _fillText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_tfLevelSlider.gameObject, "FillText");
        _fillEffect = UnityHelper.FindTheChildNode(_tfLevelSlider.gameObject, "FillEffect");

        _btnSlider = UnityHelper.GetTheChildNodeComponetScripts<Button>(_tfLevelSlider.gameObject, "Button");
        _buttonIcon = UnityHelper.GetTheChildNodeComponetScripts<Image>(_btnSlider.gameObject, "Icon");
        _buttonText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_btnSlider.gameObject, "Text");
        _positionEffect = UnityHelper.GetTheChildNodeComponetScripts<ParticleSystem>(_btnSlider.gameObject, "Sun_Shines_03");
        _brokenEffect = UnityHelper.GetTheChildNodeComponetScripts<ParticleSystem>(_btnSlider.gameObject, "ParticleBroken");

        RigisterButtonObjectEvent(_btnSlider, (go) => OnClickButton());
    }
    
    protected void RefreshPosition(EventParam param)
    {
        //b包关卡大于5关闭里程碑
        if (!GL_CoreData._instance.AbTest && GL_PlayerData._instance.CurLevel > 5)
        {
            _tfLevelSlider.SetActive(false);
            return;
        }
        
        if (_milestoneConfig == null)
        {
            _btnSlider.interactable = false;
            return;
        }

        int beginLevel = 1;
        _curMilestoneInfoIndex = GL_PlayerData._instance.GetMilestoneInfo(ref beginLevel);
        //DDebug.LogError("~~~_curMilestoneInfo:1:" + _curMilestoneInfo.GetHashCode());
        if (_curMilestoneInfoIndex < 0)
        {
            _btnSlider.interactable = false;
            return;
        }

        _btnSlider.interactable = true;
        int targetValue = _milestoneConfig.mileposts[_curMilestoneInfoIndex].level;
        int curValue = GL_PlayerData._instance.CurLevel;
        //进度条

        targetValue -= _milestoneConfig.lastGroupLevel;
        curValue -= _milestoneConfig.lastGroupLevel;
        
        _fillImage.fillAmount = Mathf.Clamp01((float)curValue / targetValue);
        _fillText.text = curValue + "/" + targetValue;
        if (_fillImage.fillAmount < 1)
        {
            _fillEffect.gameObject.SetActive(false);
            _positionEffect.Stop();
            //_brokenEffect.Stop();
        }
        else
        {
            _fillText.text = "MAX";
            _fillEffect.gameObject.SetActive(true);
            _positionEffect.Play();
            _brokenEffect.Play();
            OnClickButton();
        }

        int value = Mathf.Clamp((targetValue - curValue), 0, 10000);
        EItemType type = (EItemType)_milestoneConfig.mileposts[_curMilestoneInfoIndex].winRewards[0].type;
        _buttonIcon.sprite = GL_RewardLogic._instance.GetItemSprite(type);
        _buttonIcon.rectTransform.sizeDelta = new Vector2(240, 240);
        _buttonText.text = GL_RewardLogic._instance.GetItemNumber(type, _milestoneConfig.mileposts[_curMilestoneInfoIndex].winRewards[0].num);
        _contentText.text = string.Format(_explainList[0], value);
    }


    //点击里程碑领取
    private void OnClickButton()
    {
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.MilestoneClick);
        
        if (_milestoneConfig.mileposts[_curMilestoneInfoIndex] == null)
            return;
        if(GL_PlayerData._instance.CurLevel < _milestoneConfig.mileposts[_curMilestoneInfoIndex].level)
        {
            UI_HintMessage._.ShowMessage(transform, "关卡数不足");
            return;
        }

        GL_PlayerData._instance.DrawMilestone(_milestoneConfig.mileposts[_curMilestoneInfoIndex].id, CB_DrawMilestone);
    }

    private void CB_DrawMilestone(Net_CB_DrawMilestone msg)
    {
        _milestoneConfig.mileposts[_curMilestoneInfoIndex].status = 1;
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.MilestonceGet);
        if (_curMilestoneInfoIndex == 2)
        {
            //最后一个, 刷新数据
            GL_PlayerData._instance.GetMilestoneConfig(() =>
            {
                RefreshPosition(null);
            });
        }

        //1.奖励物品
        var reward = msg.rewards[0];
        var type = (EItemType) reward.type;
        RefreshPosition(new EventParam<bool>(false));
        switch (type)
        {
            case EItemType.Coin:
                GL_PlayerData._instance.Coin += reward.num;
                GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency, new EventParam<EFlyItemType>(EFlyItemType.Coin));
                UI_HintMessage._.ShowMessage($"恭喜您，领取{reward.num}金币！");
                break;
            case EItemType.Bogus:
                GL_PlayerData._instance.Bogus += reward.num;
                GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency, new EventParam<EFlyItemType>(EFlyItemType.Bogus));
                UI_HintMessage._.ShowMessage($"恭喜您，领取{reward.num * 0.01f}元红包！");
                break;
        }
    }
}
