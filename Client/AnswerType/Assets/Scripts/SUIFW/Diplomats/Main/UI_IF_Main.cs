using UnityEngine;
using SUIFW;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataModule;
using DG.Tweening;
using Helpser;
using Logic.Fly;
using Logic.System.NetWork;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using SUIFW.Diplomats.Common;
using SUIFW.Diplomats.Main.MyWithdraw;
using Unity.Collections;
using Object = System.Object;
using Random = UnityEngine.Random;

public partial class UI_IF_Main : BaseUIForm
{
    #region 游戏主界面


    #region 财神
    /// <summary>
    /// 财神
    /// </summary>
    private Button _moneyPool;

    #endregion

    #region 新手签到
    private Button _btnNewbieSign;
    private Text _textNewbieSign;
    #endregion


    #endregion

    #region 底部导航
    [HideInInspector]
    public Transform _taskCoinPageShow ;   //每日任务?
    [HideInInspector]
    public Transform _answerPageShow ;     //答题区
    [HideInInspector]
    public Transform _withdrawPageShow ;   //提现区
    [HideInInspector]
    public Toggle _answerPageToggle;       //答题按钮
    [HideInInspector]
    public Toggle _taskPageToggle;         //任务按钮
    [HideInInspector]
    public Toggle _withdrawPageToggle;     //提现按钮
    [HideInInspector]
    public Button _productionPageToggle;   //全民大生产按钮
    [HideInInspector]
    public Toggle _activityPageToggle;     //活动按钮
    [HideInInspector]
    public Transform _activityPageShow;    //活动区

    //签到按键
    [HideInInspector]
    public Button _newSignInPage;

    private Text _tipsTaskText;
    /// <summary>
    /// 当前显示界面
    /// </summary>
    private Transform _showNow;
    #endregion

    #region 任务

    public List<Sprite> listSprite;
    public GameObject taskItem;
    /// <summary>
    /// 打卡领现金
    /// </summary>
    private Button _signBtn;

    #endregion


    //防沉迷
    private Button _anti;
    
    public override void Init()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.Normal;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Light;
        _isOpenMainUp = true;
        
        GL_GameEvent._instance.RegisterEvent(EEventID.RefreshGameMode,RefreshGameMode);
        Transform pageControl = UnityHelper.FindTheChildNode(gameObject, "PageControl");
        _taskCoinPageShow = UnityHelper.FindTheChildNode(pageControl.gameObject, "TaskCoinPage");
        _answerPageShow = UnityHelper.FindTheChildNode(pageControl.gameObject, "AnswerPage");
         _withdrawPageShow = UnityHelper.FindTheChildNode(pageControl.gameObject, "WithdrawPage");
         _activityPageShow = UnityHelper.FindTheChildNode(pageControl.gameObject, "ActivityPage");

        _anti = UnityHelper.GetTheChildNodeComponetScripts<Button>(_answerPageShow.gameObject, "Anti");
        RigisterButtonObjectEvent(_anti, go =>
        {
            ShowTips(true);
        });

        _anti.gameObject.SetActive(GL_CoreData._instance.Nonage);

        if (GL_CoreData._instance.Nonage)
        {
            Text  timeText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_anti.gameObject, "Text");
            Timer timer = new Timer(null,true);
            timer.StartCountdown((GL_ConstData.AntiTime - GL_CoreData._instance.AntiTime) ,0,0,(() =>
            {
                ShowTips(false);
            }),timeText);
        }

        InitGameMode();

        

        #region 底部导航

        Transform bottom = UnityHelper.FindTheChildNode(gameObject, "Bottom");
        _answerPageToggle = UnityHelper.GetTheChildNodeComponetScripts<Toggle>(bottom.gameObject, "AnswerPageToggle");
      
        _taskPageToggle = UnityHelper.GetTheChildNodeComponetScripts<Toggle>(bottom.gameObject, "TaskPageToggle");
        _tipsTaskText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_taskPageToggle.gameObject, "TipsText");
        _productionPageToggle = UnityHelper.GetTheChildNodeComponetScripts<Button>(bottom.gameObject, "ProductionPageToggle");
        
        _withdrawPageToggle = UnityHelper.GetTheChildNodeComponetScripts<Toggle>(bottom.gameObject, "WithDrawToggle");
        _activityPageToggle = UnityHelper.GetTheChildNodeComponetScripts<Toggle>(bottom.gameObject, "ActivityPageToggle");
        
        #region 主页打卡按键 

        
         _newSignInPage = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "NewSignInPage");
        
         RigisterButtonObjectEvent(_newSignInPage,(go =>
         {
             UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NewSignInPage);
         }));
         
         RigisterButtonObjectEvent("TurnTable",(go =>
         {
             GL_PlayerData._instance.SendGamecoreConfig(EGamecoreType.Turntable,(() =>
             {
                 UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_TurnTable);
             }));
         }));

        #endregion

        #region 主界面切页
        _answerPageToggle.onValueChanged.AddListener(go =>
        {
            // answerSelect.SetActive(go);
            if (go)
            {
                // answerPage.SetAsLastSibling();
                ShowNow = _answerPageShow;
                UIManager.GetInstance().GetMainUp().SetActive(true);
                // UIManager.GetInstance().GetMainUp().SetGoldIngotActive(true);
                
                UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_NewWithdraw);
                UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_Activity);
            }
         
        });
        _taskPageToggle.onValueChanged.AddListener(go =>
        {
            // taskSelect.SetActive(go);
            if (go)
            {
                _tipsTaskText.transform.parent.SetActive(false);
                UIManager.GetInstance().GetMainUp().SetActive(true);
                FreshPage();
                ShowNow = _taskCoinPageShow;
                
                 UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_NewWithdraw);
                 UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_Activity);
            }
            else
            {
                _tipsTaskText.transform.parent.SetActive(ShowTask());
            }
          
        });    
        
        _withdrawPageToggle.onValueChanged.AddListener(go =>
        {
            if (go)
            {
                ShowNow = _withdrawPageShow;
                UIManager.GetInstance().GetMainUp().SetActive(false);
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NewWithdraw);
                UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_Activity);
            }
        });
        
        _activityPageToggle.onValueChanged.AddListener(go =>
        {
            if (go)
            {
                ShowNow = _activityPageShow;
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Activity);
                UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_NewWithdraw);
                UIManager.GetInstance().GetMainUp().SetActive(true);
            }
        });
        
        RigisterButtonObjectEvent(_productionPageToggle, go =>
        {
            if (!GL_PlayerData._instance.IsLoginWeChat())
            {
                //登陆微信
                // Action show =()=> PlayerIcon();
                Action show = () => { ChangeProduce(); };
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WeChatLogin, show);
                // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Setting);
            }
            else
            {
                ChangeProduce();
            }
        });
        #endregion

        #endregion

        // _newSignInPage.SetActive(true);
        //
        // //bottom
        // _productionPageToggle.SetActive(false);
        //
        // _activityPageToggle.SetActive(true);
        //
        // _withdrawPageToggle.SetActive(true);

        _tipsTaskText.transform.parent.SetActive(true);
        GL_PlayerData._instance.GetTaskConfig();

        InitTask();

        //缩减里程碑
        InitPosition();

        _answerPageToggle.isOn = true;

        _showNow = _answerPageShow;


        if (GL_CoreData._instance.AbTest)
        {
            Transform _turnTable = UnityHelper.FindTheChildNode(gameObject, "TurnTable");
            _turnTable.SetActive(false);
        }
    }


    public Transform ShowNow
    {
        get => _showNow;
        set
        {
            _showNow.SetActive(false);
            _showNow = value;
            _showNow.SetActive(true);
        }
    }
    
    #region 存钱罐

    public void OnBtnGoldenpig()
    {
        YS_NetLogic._instance.GoldenpigConfig((config =>
        {
            Object[] objects = { config };
            UI_Diplomats._instance.ShowUI(SysDefine.UI_IF_Goldenpig,objects);
        } ));
    }

    #endregion

    #region 声音控制

    // private void ShowAudio()
    // {
    //     _audioControl.isOn = GL_CoreData._instance.AudioOn;
    // }
    //
    // private void SetAudio(bool set)
    // {
    //     GL_CoreData._instance.AudioOn = set;
    //     GL_CoreData._instance.BGMAudioOn = set;
    // }

    #endregion
    
    #region 底部导航栏

    private void ChangeMessage(bool pick,  Text toggle)
    {
        if (pick)
        {
            toggle.fontSize = 45;
            toggle.GetComponent<Outline8>().effectColor = new Color(0,99,142,128)/255;
        }
        else
        {
            toggle.fontSize = 40;
            toggle.GetComponent<Outline8>().effectColor = new Color(100,100,100,128)/255;
        }
    }

    public void ChangePage(string page)
    {
        switch (page)
        {
            case GL_ConstData.TaskPage:
                _taskPageToggle.isOn = true;
                break;
            case GL_ConstData.AnswerPage:
                _answerPageToggle.isOn = true;
                break;
            // case GL_ConstData.PersonPage:
            //     _personPageToggle.isOn = true;
            //     break;
        }
    }
    #endregion

    #region 主页新手签到
    private void RefreshNewbieSign(EventParam param)
    {
        bool set = GL_NewbieSign._instance.IsShowIcon();
        _btnNewbieSign.SetActive(set);
        if(set)
        {
            if (GL_NewbieSign._instance._gamecoreConfig == null
                || GL_NewbieSign._instance._gamecoreConfig.rewards == null
                || GL_NewbieSign._instance._gamecoreConfig.rewards.Count == 0)
            {
                _textNewbieSign.text = string.Format("<size=60>¥</size>{0}", 668);
            }
            else
            {
                _textNewbieSign.text = string.Format("<size=60>¥</size>{0}",
                GL_NewbieSign._instance._gamecoreConfig.rewards[0].num / 100f);
            }
        }
    }

    private void OnClickNewbieSign()
    {
        if(GL_NewbieSign._instance.NewbieSignState == ENewbieSignState.WaitSelect)
        {
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_ChangeSignType);
        }
        else
        {
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NewbieSign);
        }
    }
    #endregion

    #region override
    void SetCanvas()
    {
        var canvas = this.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = -1;
        }
    }
    public override void onUpdate()
    {
    }

    public override void Refresh(bool recall)
    {
        RefreshGameMode(null);
        GL_GameEvent._instance.RegisterEvent(EEventID.RefreshPosition, RefreshPosition);
        RefreshPosition(null);

        GL_GameEvent._instance.RegisterEvent(EEventID.RefreshNewbieSignUI, RefreshNewbieSign);
        RefreshNewbieSign(null);
    }

    public override void OnHide()
    {
        GL_GameEvent._instance.UnregisterEvent(EEventID.RefreshGameMode, RefreshGameMode);
        GL_GameEvent._instance.UnregisterEvent(EEventID.RefreshPosition, RefreshPosition);
        GL_GameEvent._instance.UnregisterEvent(EEventID.RefreshNewbieSignUI, RefreshNewbieSign);
        StopAllCoroutines();
        CancelInvoke();
    }
    
    #endregion

    public void ChangeProduce()
    {
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
        GL_PlayerData._instance.GetProduceConfig((() =>
        {
            UIManager.GetInstance().CloseUIForms(SysDefine.UI_Path_NetLoading);
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Production);
        }));
    }    
    #region 防沉迷
    private void ShowTips(bool exit)
    {
        int time = ((GL_CoreData._instance.AntiTime))/60 ;
        Object[] objects = { time , exit};
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_TipsPage,objects);
    }
    #endregion
}

                    