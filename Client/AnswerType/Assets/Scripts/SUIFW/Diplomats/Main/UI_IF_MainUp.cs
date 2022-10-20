using UnityEngine;
using SUIFW;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Logic.Fly;
using Logic.MainUp;
using Logic.System.NetWork;
using SUIFW.Diplomats.Common;
using SUIFW.Helps;
using UnityEngine.EventSystems;
using Object = System.Object;

public class UI_IF_MainUp : BaseUIForm
{
    public RectTransform _flyRoot;
    public RectTransform _coinFlyTarget;
    private Transform _coin;
    public RectTransform _bogusFlyTarget;
    public Button _btnCoin;    //提现按钮
    private Text _coinText;     //金币数值
    private Text _coinBubble;   //金币气泡
    private Text _bogusTargetText;

    private string _coinNow = "再赚<color=#E34318>{0}金币</color>";

    private UI_IF_Main _main;

    private Button _bogusBtn;
    private Transform _bogus;
    /// <summary>
    /// 假现金数值
    /// </summary>
    private Text _bogusNumber;

    private List<string> _bogusStr = new List<string>()
    {
        "再赚：<color=#fb3234>{0}</color>元",
        "提现：<color=#fb3234>300</color>元",
        "今日通关<color=#fb3234>{0}</color>关可提现",
        "连续登录7天即可提现",
        "即可提现",
        "还差{0}天",
        "还差<color=#fb3234>{0}</color>元可提现<color=#fb3234>{1}</color>元"
        
    };
    
    public override void Init()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.PopUp;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate;
        
        _isNeedRefresh = false;
        _flyRoot = UnityHelper.GetTheChildNodeComponetScripts<RectTransform>(gameObject, "Root");
        _coin = UnityHelper.FindTheChildNode(gameObject, "Coin");
        _btnCoin = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "CoinBtn");
        RigisterButtonObjectEvent(_btnCoin, go => { OnClickCoin(); });
        _coinText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_coin.gameObject, "_txtGoldIngot");
        _coinFlyTarget =  _coinText.GetComponent<RectTransform>();
        _coinBubble = UnityHelper.GetTheChildNodeComponetScripts<Text>(_coin.gameObject, "CoinText");
        
        _flyRoot = UnityHelper.GetTheChildNodeComponetScripts<RectTransform>(gameObject, "Root");
        
        _bogus = UnityHelper.FindTheChildNode(gameObject, "Bogus");
        _bogusFlyTarget = _bogus.GetComponent<RectTransform>();
        _bogusNumber = UnityHelper.GetTheChildNodeComponetScripts<Text>(_bogus.gameObject, "Number");

        _bogusTargetText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_bogus.gameObject, "Target");
        _bogusBtn = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "BogusBtn");
        RigisterButtonObjectEvent(_bogusBtn, go => OnClickBogus());
        ChangeShowBogus();
        _main =  UIManager.GetInstance().GetUI(SysDefine.UI_Path_Main) as UI_IF_Main;
        
        RigisterButtonObjectEvent("Setting", go =>
        {
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Setting);
        });
        
        Fly_MainUp.Init(this).InitData();

        _bogusBtn.SetActive(false);
    }

    public override void Refresh(bool recall)
    {
        GL_GameEvent._instance.RegisterEvent(EEventID.RefreshCurrency, RefreshCurrency);
        
        //主页显示刷新，不播放音效
        RefreshCurrency(null);

        //显示微信头像
        //RefreshPlayInfoIcon();
        
        ShowTarget();
    }

    private void ShowTips()
    {
        //转盘不跳提现
        var turnTable = UIManager.GetInstance().GetUI(SysDefine.UI_Path_TurnTable);
        if (turnTable.isActiveAndEnabled)
        {
            return;
        }
        
        if (GL_PlayerData._instance.IsEnoughCoin() )
        {
            if (GL_CoreData._instance.IsEcpm)
            {
                // UI_Diplomats._instance.ShowUI(SysDefine.UI_IF_GoldIngot);
                //if (GL_CoreData._instance.AbTest)
                //{
                //    UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Withdraw);
                //}
                //else
                {

                    _main._withdrawPageToggle.isOn = true;
                    // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NewWithdraw);
                }
            }
            else
            {
                if (!GL_CoreData._instance.ShowPage)
                {
                    GL_CoreData._instance.ShowPage = true;
                    //if (GL_CoreData._instance.AbTest)
                    //{
                    //    UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Withdraw);
                    //}
                    //else
                    {
                        _main._withdrawPageToggle.isOn = true;
                        // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NewWithdraw);
                    }
                }
            }
        }
    }

    public override void Display(bool redisplay = false)
    {
        base.Display(redisplay);
        gameObject.transform.SetAsLastSibling();
    }

    //刷新货币
    private void RefreshCurrency(EventParam param)
    {
        EFlyItemType curFlyItemType = EFlyItemType.None;
        List<EFlyItemType> curFlyItemTypes;
        Vector3 startPos;
        bool isActivity;

        if (param != null)
        {
            try
            {
                if (param is EventParam<EFlyItemType> flyItem0)
                {
                    curFlyItemType = flyItem0._param;
                    DoUpLogic(curFlyItemType);
                }
                else if (param is EventParam<List<EFlyItemType>> flyItem1)
                {
                    curFlyItemTypes = flyItem1._param;
                    curFlyItemTypes.ForEach((type =>
                    {
                        DoUpLogic(type);
                    }));
                }
                else if (param is EventParam<EFlyItemType,Vector3> flyItem2)
                {
                    curFlyItemType = flyItem2._param;
                    startPos = flyItem2._param2;
                    DoUpLogic(curFlyItemType, startPos);
                }
                else if (param is EventParam<EFlyItemType,Vector3,bool> flyItem3) //处理激活MainUp
                {
                    curFlyItemType = flyItem3._param;
                    startPos = flyItem3._param2;
                    isActivity = flyItem3._param3;
                    DoUpLogic(curFlyItemType, startPos,isActivity);
                }
            }
            catch (Exception e)
            {
                DDebug.LogError("转换飞行类型失败，请检查");
            }
        }
        else
        {
            DoUpLogic(curFlyItemType);
        }
    }

    private bool ShowPoint = true;
    private void ShowTarget()
    {
        // GL_PlayerData._instance.CalculateWithDrawTarget(EWithDrawType.DailyWithDraw);
        if (GL_PlayerData._instance._withDrawTarget.ContainsKey(EWithDrawType.DailyWithDraw) && GL_PlayerData._instance._withDrawTarget[EWithDrawType.DailyWithDraw] != null)
        {
            // _bogusTargetText.transform.parent.SetActive(true);
            float value1 = (GL_PlayerData._instance._withDrawTarget[EWithDrawType.DailyWithDraw].coupon-GL_PlayerData._instance.Coin );
             if (value1<=0)
             {
                 _coinBubble.text = "立即提现";

             }
             else
             {
                 _coinBubble.text = $"差<color=#FF0000>{value1}</color>金币提现";
             }
        }
        else
        {
            _coinBubble.transform.parent.SetActive(false);
        }

    }

    private float _coinTextShow;
    private float _bogusTextShow;
    private void DoUpLogic(EFlyItemType type,Vector3 startPos = default,bool isActivityMainUp = false)
    {
        //金币
        string curCoin = GL_PlayerData._instance.Coin.ToString();

        //现金
        string curBogus = GL_PlayerData._instance.Bogus_Convert.ToString("0.00");

        switch (type)
        {
            case EFlyItemType.None:
                _coinText.text = curCoin;
                _bogusNumber.text = curBogus + "元";
                break;
            case EFlyItemType.Coin:
                // 金币增加
                 if (!_coinText.text.Equals(curCoin))
                 {
                     GL_AudioPlayback._instance.Play(5);
                
                     Fly_MainUp._.FlyCoin(5,startPos,isActivityMainUp);

                    UI_AnimHelper.AddVauleAnim(EItemType.Coin, _coinText, float.Parse(curCoin), 1.5f, _coinTextShow,
                        () =>
                        {
                            ShowTarget();
                            ShowTips();
                            _coinText.text = GL_PlayerData._instance.Coin.ToString();
                        });
                    _coinTextShow = GL_PlayerData._instance.Coin;
                 }
 
                break;
            case EFlyItemType.Bogus:
                //现金增加
                if (!_bogusNumber.text.Equals(curBogus))
                {
                    Fly_MainUp._.FlyBogus(5,startPos,isActivityMainUp);
                   
                    UI_AnimHelper.AddVauleAnim(EItemType.Bogus,_bogusNumber, float.Parse(curBogus), 0.3f, _bogusTextShow,() =>
                    {
                        ChangeShowBogus();
                        _bogusNumber.text = GL_PlayerData._instance.Bogus_Convert.ToString();
                    });
                    _bogusTextShow = GL_PlayerData._instance.Bogus_Convert;
                }

                break;
        }
    }

    #region 金币
    private void OnClickCoin()
    {
        //if (GL_CoreData._instance.AbTest)
        //{
        //    UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Withdraw);
        //}
        //else
        {
            UIManager.GetInstance().GetMain()._withdrawPageToggle.isOn = true;
        }
    }
    #endregion

    #region 假现金
    private void OnClickBogus()
    {
        // object[] objects = { null, true };
        // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Cash, objects);
        // _main.ChangePage(GL_ConstData.PersonPage);

        //if (GL_CoreData._instance.AbTest)
        //{
        //    UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_RedWithdraw);
        //}
        //else
        {
            UIManager.GetInstance().GetMain()._withdrawPageToggle.isOn = true;
        }
    }
    

    /// <summary>
    /// 切换提现提示
    /// </summary>
    private void ChangeShowBogus()
    {
        //if (!GL_CoreData._instance.AbTest)
        {
            _bogusTargetText.transform.parent.SetActive(false);
        }
        
        if (GL_PlayerData._instance.Bogus_Convert<300)
        {
            _bogusTargetText.text = string.Format(_bogusStr[6],(300-GL_PlayerData._instance.Bogus_Convert).ToString("0.00"),300);
            return;
        }

        if (GL_PlayerData._instance.UserDayLevel<80)
        {
            _bogusTargetText.text = string.Format(_bogusStr[2],80-GL_PlayerData._instance.SystemConfig.userDayLevel);
            return;
        }
        
        _bogusTargetText.text = string.Format(_bogusStr[4]);
    }

    #endregion


    public override void onUpdate()
    {
        Fly_Manager._instance.DoUpdate(Time.deltaTime);
    }

    public override void OnHide()
    {
        base.OnHide();
        GL_GameEvent._instance.UnregisterEvent(EEventID.RefreshCurrency, RefreshCurrency);
    }

    public override void Redisplay()
    {
        base.Redisplay();
        transform.SetAsLastSibling();
    }

    public override void RefreshLanguage()
    {

    }
}

