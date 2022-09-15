using System;
using System.Text.RegularExpressions;
using SUIFW;
using UnityEngine.UI;

public class UI_IF_WithdrawSuccess : BaseUIForm
{
    private string _content = /*<size=80>¥ :</size>*/"{0}元";

    private Text _moneyText;
    
   
    
    private EWithDrawType _eWithDrawType; 
    public override void Init()
    {
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;

        _moneyText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "MoneyText");

        RigisterButtonObjectEvent("BtnSure", (go => { CloseUIForm(); }));
        RigisterButtonObjectEvent("ExitPage", go =>
        {
            CloseUIForm();
        });
    }

    public override void InitData(object data)
    {
        base.InitData(data);
        
        var datas = data as object[];
        if (datas == null)
            return;
        if (datas.Length>0 && datas[0] is float)
        {
            _waitWithDraw= (float) datas[0];
            _moneyText.text = datas[0].ToString();
        }

        if (datas.Length>1 && datas[1] is EWithDrawType)
        {
            _eWithDrawType =(EWithDrawType) datas[1];
            // DDebug.LogError("***** 体现类型："+ _eWithDrawType);
        }
    }

    public override void onUpdate()
    {
    }

    public override void Refresh(bool recall)
    {

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

        if (_eWithDrawType!= EWithDrawType.WaitWithDraw)
        {
            GL_PlayerData._instance.BankConfig.nowMoney += _waitWithDraw;
            GL_GameEvent._instance.SendEvent(EEventID.RefreshWaitWithDraw);
        }
    }

    #region 存储奖池

    private float _waitWithDraw;

    #endregion
}