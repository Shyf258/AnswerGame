using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SUIFW;
using SUIFW.Diplomats.Common;
using SUIFW.Diplomats.Common.Withdraw;
using UnityEngine.UI;

public class UI_IF_WithdrawSuccess : BaseUIForm
{
    private string _content = /*<size=80>¥ :</size>*/"{0}元";

    private Text _moneyText;

    private Text _tipsText;

    private Text _time;

    private Timer _timer;
    
    private float _withDrawResult;

    private float _money;

    private Button _close;
    
    private List<string> _list = new List<string>()
    {
        "提现成功<color=#fc0000>{0}</color>元",
        "提现<color=#fc0000>{0}</color>元，+登录增幅提现<color=#ff7800>{1}</color>元"
    };
    
    private EWithDrawType _eWithDrawType; 
    public override void Init()
    {
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;

        _moneyText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "MoneyText");

        _tipsText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "GrowTips");

        _close = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "BtnSure");
        
        RigisterButtonObjectEvent(_close, (go => { CloseUIForm(); }));

        _time = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "TimeText");
        
        _timer = new Timer(this,true);
    }

    public override void InitData(object data)
    {
        base.InitData(data);
        
        var datas = data as object[];
        if (datas == null)
            return;
        if (datas.Length>0 && datas[0] is float money)
        {
            _money = money;
        }

        if (datas.Length>1 && datas[1] is EWithDrawType)
        {
            _eWithDrawType =(EWithDrawType) datas[1];
            // DDebug.LogError("***** 体现类型："+ _eWithDrawType);
        }
        if (datas.Length>2 && datas[2] is int result)
        {
            _withDrawResult =(float) result/100f;
            _moneyText.text = string.Format(_list[0], _withDrawResult.ToString("0.00"));
            // DDebug.LogError("***** 体现类型："+ _eWithDrawType);
            
            _tipsText.SetActive(true);
            _tipsText.text = string.Format(_list[1], _money, (_withDrawResult - _money).ToString("0.00"));
            
            if (_withDrawResult <=_money )
            {
                int hour = (GL_PlayerData._instance._WithDrawGrowConfig.countDown / 3600);
                int min = (GL_PlayerData._instance._WithDrawGrowConfig.countDown -
                           (hour*60)) / 60;
                int second = GL_PlayerData._instance._WithDrawGrowConfig.countDown - (hour * 60) - (min * 60);
                UI_HintMessage._.ShowMessage($"当前福利放完毕\n{hour.ToString("00")}" + $":{min.ToString("00")}" + $"{second.ToString("00")}" + $"小时后继续发放");
                _tipsText.SetActive(false);
            }
        }
       


       
       
    }

    public override void onUpdate()
    {
    }

    public override void Refresh(bool recall)
    {
        _close.interactable = false;
        _timer.StartCountdown(3,0,0, () =>
        {
            _time.text = "确定";
            _close.interactable = true;
        },_time);
       
        MethodExeTool.InvokeDT((() =>
        {
            var ui = UIManager.GetInstance().GetUI(SysDefine.UI_Path_WechatWithdrawTip) as UI_IF_WechatWithdrawTip;
            ui?.Show();
        }),1f);
    }

    public override void RefreshLanguage()
    {
    }
    
    /// <summary>
    /// 日程提醒
    /// </summary>
    private void DateTips()
    {
        if (GL_SDK._instance.CheckCalendarPermission())
        {
            
            string title = "[冰雪大赢家]\\ud83d\\ude04 赚钱领现金，小额领不停。";
            title = Regex.Unescape(title);
            GL_SDK._instance.RequestCalendarPermission(title,
                "[冰雪大赢家] 友情提示：登录天数越多提现额度越高，单日无限制小额提现超过20元，错过了就麻烦拉！");
        }
        else
        {
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Date);
        }
    }
    public override void OnHide()
    {
        base.OnHide();
        // DDebug.LogError("提现类型："+_eWithDrawType);
        // DDebug.LogError("是否是B包："+!GL_CoreData._instance.AbTest);
        // if (_eWithDrawType == EWithDrawType.Normal && !GL_CoreData._instance.AbTest )
        // {
        //     DateTips();
        // }
        
        
    }
}