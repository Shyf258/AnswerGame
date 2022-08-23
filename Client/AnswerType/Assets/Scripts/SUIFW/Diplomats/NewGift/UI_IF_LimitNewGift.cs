using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SUIFW;
using System;

public class UI_IF_LimitNewGift : BaseUIForm
{
    private Text _textCD;

    private int _totalTime = 30 * 60;
    private float _timer;
    private Action _callback;
    public override void Init()
    {
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;
        _isFullScreen = false;

        _textCD = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "CD");

        RigisterButtonObjectEvent("Button1", (go => { OnClickButton(); }));
        RigisterButtonObjectEvent("Button2", (go => { OnClickButton(); }));
        RigisterButtonObjectEvent("Button3", (go => { OnClickButton(); }));

        _timer = _totalTime;
    }

    public override void InitData(object data)
    {
        base.InitData(data);

        var datas = data as object[];
        if (datas == null)
            return;

        //1.回调
        if (datas.Length > 0 && datas[0] is Action action)
        {
            _callback = action;
        }
    }
    private void OnClickButton()
    {
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.NewPlayClickRed);
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.NewPlayGetRed);
        CloseUIForm();
        _callback?.Invoke();
        _callback = null;
    }

    public override void onUpdate()
    {
        _timer -= Time.deltaTime;
        if (_timer < 0)
            _timer = _totalTime;

        int _showHour = (int)(_timer / 3600);
        int value = (int)_timer % 3600;
        int _showMinute = (int)(value / 60);
        int _showSecond = (int)(value % 60);

        _textCD.text = GL_Tools.GetFormatCd(_showHour, _showMinute, _showSecond, 3);
    }

    public override void Refresh(bool recall)
    {
    }
}
