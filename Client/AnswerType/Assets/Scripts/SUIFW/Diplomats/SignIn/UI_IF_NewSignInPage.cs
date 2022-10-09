using System;
using System.Collections;
using System.Collections.Generic;
using Logic.System.NetWork;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

public class UI_IF_NewSignInPage : BaseUIForm
{

    #region 信息
    /// <summary>
    /// 提示文本
    /// </summary>
    private List<string> _tipsText = new List<string>()
    {
        "恭喜领取成功",
        "请您按照顺序领取",
        "您还没有满足条件"
    };

    private List<int> _signPlanList = new List<int>()
    {
        1,2,3,4,7
    };

    private string _planDescription = "提现条件：观看<color=#ff0000>{0}</color>次视频";
    
    private Action _callback;

    #endregion
    
    #region 对象

   
    
    /// <summary>
    /// 提现列表list
    /// </summary>
    private List<GameObject> _signList;
    
    /// <summary>
    /// 进度描述文本
    /// </summary>
    private Text _sliderText;
    // /// <summary>
    // /// 进度条
    // /// </summary>
    // private Image _sliderBar;

    /// <summary>
    /// 进度
    /// </summary>
    private Slider _slider;
    /// <summary>
    /// 按键组
    /// </summary>
    private Transform _groupSign;
    /// <summary>
    /// 当前已打卡天数
    /// </summary>
    private Text _date;
    /// <summary>
    /// 进度百分百显示
    /// </summary>
    private Text _sliderPlan;
    
    
    public Button _signBtn;

    private Image _signBtnIcon;
    private Text _btnText;
    // private bool _full = false;
    /// <summary>
    /// 0 提现  1打卡 
    /// </summary>
    [GL_Name("按键图片")]
    public List<Sprite> _btnSprite;
    #endregion
    
    #region override
    public override void Init()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.PopUp;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;
        
        _groupSign = UnityHelper.FindTheChildNode(gameObject, "SignIn_Group");

        _sliderText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "SliderText");

        // _sliderBar = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "SliderBar");

        _slider = UnityHelper.GetTheChildNodeComponetScripts<Slider>(gameObject, "Slider");
        
        _sliderPlan = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "SliderPlan");
        _date = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "DayNumber");
        RigisterButtonObjectEvent("Btn_Back", go =>
        {
            CallBack();
            CloseUIForm();
        });

        for (int i = 0; i < _groupSign.childCount; i++)
        {
         
            _groupSign.GetChild(i).GetComponent<FF_SignNew>().Init();
        }

        _signBtn = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "SignBtn");
        RigisterButtonObjectEvent(_signBtn, go =>
        {
            SignClick();
        });
        _signBtnIcon = _signBtn.GetComponent<Image>();
        _btnText =  UnityHelper.GetTheChildNodeComponetScripts<Text>(_signBtn.gameObject, "BtnText");
        
        
    }
    
    public override void InitData(object data)
    {
        base.InitData(data);

        _callback = data as Action;
    }
    public override void RefreshLanguage()
    {
       
    }

    public override void Refresh(bool recall)
    {
        ShowMessage();
       
        
        // GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Banner_SignInPage);
        // YS_NetLogic._instance.SearchClockin(ShowMessage);

      
    }

    public override void onUpdate()
    {
       
    }

    public override void Hiding()
    {
        base.Hiding();
        
        // GL_AD_Interface._instance.CloseBannerAd();
        // Logic.System.NetWork.YS_NetLogic._instance.RefreshTask();
    }

    #endregion
  
    
    private void CallBack()
    {
        _callback?.Invoke();
        _callback = null;
    }

    /// <summary>
    /// 已成功打卡天数
    /// </summary>
    private int _clockinDay;
    /// <summary>
    /// 显示内容信息
    /// </summary>
    private void ShowMessage()
    {


        
        float plan = (float) 
                     GL_PlayerData._instance.SigNetCbClockinConfig.hasViewAd/
                     GL_PlayerData._instance.SigNetCbClockinConfig.needViewAd;
        if (plan >= 1)
        {
            plan = 1;
        }
        _clockinDay = GL_PlayerData._instance.SigNetCbClockinConfig.day;
         ChangeDate();
        _slider.value = plan;
        // _sliderBar.fillAmount = plan;
        _sliderText.text = string.Format(_planDescription,
            GL_PlayerData._instance.SigNetCbClockinConfig.needViewAd.ToString());
        _sliderPlan.text = (plan * 100).ToString("0.00") +"%"; //进度百分百文字
        
         RefreshChild();
  
        if (GL_PlayerData._instance.SigNetCbClockinConfig.hasViewAd >=
            GL_PlayerData._instance.SigNetCbClockinConfig.needViewAd &&
            GL_PlayerData._instance.SigNetCbClockinConfig.hasClock==2 )
        {
            YS_NetLogic._instance.Clockin((() =>
            {
                GetSignInConfig();
            }));
        }
    }

    private void ChangeDate()
    {
        if (GL_PlayerData._instance.SigNetCbClockinConfig.hasClock!=1)
        {
            _clockinDay -= 1;
        }
        _date.text = _clockinDay.ToString(); //显示已打卡天数
        // if (  _signPlanList[_nowSignId]<=
        //       GL_PlayerData._instance.SigNetCbClockinConfig.day
        //       &&
        //       GL_PlayerData._instance.SigNetCbClockinConfig.hasViewAd >=
        //       GL_PlayerData._instance.SigNetCbClockinConfig.needViewAd 
        //       &&
        //       GL_PlayerData._instance.SigNetCbClockinConfig.hasClock==1  )
        // {
        //     _signBtnIcon.sprite = _btnSprite[0];
        //     _btnText.text = "提现";
        //     _full = true;
        // }
        // else
        // {
        //     _signBtnIcon.sprite = _btnSprite[1];
        //     _btnText.text = "打卡";
        //     _full = false;
        // }
    }

    private void GetSignInConfig()
    {
        YS_NetLogic._instance.SearchClockin(() =>
        {
            ShowMessage();
        });
    }

    
    private int  _nowSignId = 0;
    
    /// <summary>
    /// 刷新子对象
    /// </summary>
    private void RefreshChild()
    {
        _nowSignId = -1;
        var list = GL_PlayerData._instance.GetWithDrawConfig(EWithDrawType.Clockin);
        if (list == null)
            return;
        for (int i = 0; i < _groupSign.childCount; i++)
        {
            SignConfig _signConfig = new SignConfig();
            _signConfig._planNow =  _clockinDay;
            _signConfig._planTaget = _signPlanList[i];
            _signConfig.ID = list.couponWithDraws[i].id;
            _signConfig._rewardNumber = list.couponWithDraws[i].money /100f;
            
            if (_signPlanList[i]>= GL_PlayerData._instance.SigNetCbClockinConfig.day && _nowSignId == -1)
            {
                _nowSignId = i;
                _signConfig._nowSign = true;
            }
            if (list.couponWithDraws[i].withDrawLimit<=0)
            {
                _signConfig._planState = 1;
            }
            else
            {
                _signConfig._planState = 2;
            }
            _groupSign.GetChild(i).GetComponent<FF_SignNew>().ShowMessage(_signConfig);
        }
    }

    private void SignClick()
    {
        if (GL_PlayerData._instance.SigNetCbClockinConfig.hasClock==2 )
        {
            ReportReward();
        }
        else
        {
            UI_HintMessage._.ShowMessage("请勿重复打卡");
        }
    }

    private float _nextSigIn;
    private void ReportReward()
    {
        //----------------------- 按键间隔

        if (_nextSigIn<=0)
        {
            _nextSigIn = Time.time;
        }
        else
        {
            if (GL_PlayerData._instance.ClickBtn( _nextSigIn,GL_PlayerData._instance.CbPlayAd.waitTime))
            {
                _nextSigIn = Time.time;
                // DDebug.LogError("***** 当前按键生效！");
            }
            else
            {
                UI_HintMessage._.ShowMessage(
                    "冷却中，请稍等"
                );
                // DDebug.LogError("***** 当前按键冷却中...");
                return;
            }
        }
        
        GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_SignInReport, (set) =>
        {
            if (set)
            {
                InvokeRepeating("SignConfig",0.5f,1);
            }
            
        },GL_ConstData.SceneID_Sign);
    }

    private void SignConfig()
    {
        GetSignInConfig();

        CancelInvoke();
    }

    private void WithDraw()
    {

        var list = GL_PlayerData._instance.GetWithDrawConfig(EWithDrawType.Clockin);
        if (list == null)
            return;
        // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDraw_SignClockIn);
        if (list.couponWithDraws[_nowSignId].withDrawLimit <= 0)
        {
            UI_HintMessage._.ShowMessage(/*transform.parent,*/"当前奖励已领完");
            return;
        }

        if (_clockinDay < _signPlanList[_nowSignId])
        {
            UI_HintMessage._.ShowMessage(/*transform.parent,*/"当前打卡天数不足");
            return;
        }

        //CB_WithDraw(null);
        //return;
        //判断是否微信登陆.
        if (!GL_PlayerData._instance.IsLoginWeChat())
        {
            //登陆微信
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WeChatLogin);
        }
        else
        {
            //提现广告
            GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_WithDrawSign, (set) =>
            {
                //提现广告播放成功
                Net_WithDraw draw = new Net_WithDraw();
                // draw.withDrawType = (int) EWithDrawType.Clockin;
                draw.withDrawId = _nowSignId;
                draw.type = 2;
                GL_ServerCommunication._instance.Send(Cmd.WithDraw, JsonUtility.ToJson(draw), CB_WithDraw);
            });
        
        }
    }
    
    private void CB_WithDraw(string param)
    {
        GL_PlayerData._instance.Net_CB_WithDrawResult(param);
        UIManager.GetInstance().CloseUIForms(SysDefine.UI_Path_NewSignInPage);
        var list = GL_PlayerData._instance.GetWithDrawConfig(EWithDrawType.Clockin);
        if (list == null)
            return;

        float money = list.couponWithDraws[_nowSignId].money * 0.01f;
        EWithDrawType _eWithDrawType = EWithDrawType.Clockin;
        var obj = new object[]
        {
            money,
            _eWithDrawType,
            GL_PlayerData._instance._netCbWithDraw.money
        };
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
        GL_PlayerData._instance.SendWithDrawConfig(EWithDrawType.Clockin, () =>
        { 
            UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_NetLoading);
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WithdrawSuccess, obj);
        });
       
    }

}

public class SignConfig
{
    // public SRewardData _sRewardData; //图片 
     // public int _signDay; //签到天数
     /// <summary>
     /// 当前签到多少天
     /// </summary>
    public int _planNow;  
     /// <summary>
     /// //目标天数多少天
     /// </summary>
    public int _planTaget;
     /// <summary>
     /// // 1.灰色已领取 2，黄色未领取 3，绿色未完成
     /// </summary>
     public int _planState; 
     /// <summary>
     /// //奖励数量
     /// </summary>
     public float _rewardNumber; 
     public int ID;
     public bool _nowSign;
}
