//11.26 转盘点击旋转 更新提现进度 刷新红包券显示

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using Logic.Fly;
using Logic.System.NetWork;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class UI_IF_TurnTable : BaseUIForm
{
    #region 游戏对象

    /// <summary>
    /// 转盘进度条显示
    /// </summary>
    private Transform _drawPlanRGrow;
    private Image _middle;
    private Image _end;
    /// <summary>
    /// 转盘父对象
    /// </summary>
    private Transform _turnTable;
    /// <summary>
    /// 转盘
    /// </summary>
    private Transform _turnTableDraw;

    /// <summary>
    /// 关闭按键
    /// </summary>
    private Button _closePage;


    private Image _mask;
    
    #endregion

    #region 参数设置

    private float _maxSpeed = 600f;
    private float _speed;
    private float _timerTurn;
    private float _moveTime = 0;

    
    /// <summary>
    /// 转盘状态
    /// </summary>
    private EStage _stage;
    
    /// <summary>
    /// 转盘当前可用次数
    /// </summary>
    private int _drawTimeNow;
    /// <summary>
    /// 转盘当前可免费用次数
    /// </summary>
    private int _drawTimeFree;
    /// <summary>
    /// 转盘配置
    /// </summary>
    private Net_CB_GamecoreConfig _drawConfig;
    /// <summary>
    /// 道具显示父对象
    /// </summary>
    private Transform _iconCenter;
    /// <summary>
    /// 0.5元提现进度
    /// </summary>
    private Image _fill;

    private Text _barText;
    
    /// <summary>
    /// 0.5元提现
    /// </summary>
    private Button _getBar;
    
    /// <summary>
    /// 奖励图片显示
    /// </summary>
    public List<Sprite> rewardIcon;
    private Slider _drawPlanBar;
    /// <summary>
    /// 奖励对应 UI的索引
    /// </summary>
    public int _itemIndex;

    private Text _description;

    private string descripString = "剩余次数：{0}\n每日0点刷新抽奖次数";
    
    public Button _turnGet;

    private Text _getText;
    
    private Image _playAd;
    
    private bool freshTurn = false;
    
    // private bool _isPlayVideo;
    #endregion

    public override void Refresh(bool recall)
    {
        RefreshPage();
    }

    public override void onUpdate()
    {
        
    }

    public override void Init()
    {
        InitTurn();
    }

    public  void InitTurn()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.PopUp;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;
        _isFullScreen = true;
        _isOpenMainUp = false;
        
        #region 获取数值
        _uiIfMainUp = UIManager.GetInstance().GetMainUp();
        #endregion

        Transform _transform = UnityHelper.FindTheChildNode(gameObject, "UI_IF_TurnTable");
        
        _iconCenter = UnityHelper.FindTheChildNode(_transform.gameObject, "IconCenter");
     
        _turnTable = UnityHelper.FindTheChildNode(_transform.gameObject, "TurnTable");
        _turnTableDraw = UnityHelper.GetTheChildNodeComponetScripts<Transform>(_turnTable.gameObject, "TurnTable_Draw");
        _closePage = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "ClosePage");
        // RigisterButtonObjectEvent(_drawPoint,go => Turn());  //转盘开始旋转
        RigisterButtonObjectEvent(_closePage,go => ExitPage());

        _turnGet = UnityHelper.GetTheChildNodeComponetScripts<Button>(_transform.gameObject, "TurnGet");
        RigisterButtonObjectEvent(_turnGet, go =>
        {
            Turn();
        });
        _getText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_turnGet.gameObject, "Text");
        _playAd = UnityHelper.GetTheChildNodeComponetScripts<Image>(_turnGet.gameObject, "TC_icon_3");
        _getBar = UnityHelper.GetTheChildNodeComponetScripts<Button>(_transform.gameObject, "GetBar");
        
        RigisterButtonObjectEvent(_getBar, go =>
        {
            if (_drawConfig.stage == 1)
            {
                GetWithDraw();
            }
            else
            {
                UI_HintMessage._.ShowMessage("不可以重复提现！");
            }
           
        });
        
        _fill = UnityHelper.GetTheChildNodeComponetScripts<Image>(_transform.gameObject, "Fill");

        _barText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_transform.gameObject, "BarText");
        
        _description = UnityHelper.GetTheChildNodeComponetScripts<Text>(_transform.gameObject, "Description");

        _mask = UnityHelper.GetTheChildNodeComponetScripts<Image>(_transform.gameObject, "Mask");


        _state = State.Get;
        _turn = new Timer(null,true);
        // GetRotate();
    }

    public  void RefreshPage()
    {
        _drawConfig = GL_PlayerData._instance.GetGamecoreConfig(EGamecoreType.Turntable);
        GetRealCount();
        ChangeTurn();
        
        // _isPlayVideo = false;
    }

   
    #region 转盘旋转

    /// <summary>
    /// 转盘旋转点击事件
    /// </summary>
    public void Turn()
    {

        if (_stage!= EStage.None)
        {
            return;
        }

        if (_state == State.Wait)
        {
            return;
        }
        
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.TurnTableGet);
        if (_drawConfig.dayProgress==0)
        {
            CB_AD_Turn(true);
                return;
                
        }
         
     
      
        if (_drawTimeNow > 0)
        {
            GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_TurnTable, CB_AD_Turn,GL_ConstData.SceneID_Turn);
        }
        else
        {
            UI_HintMessage._.ShowMessage(transform,"领取次数已满，请明日再来");
        }
    }

    public void CB_AD_Turn(bool isSuccess)
    {
        // _isPlayVideo = isSuccess;
        if(isSuccess)
        {
            //假值用于界面显示
            _drawTimeNow--;
            ChangeTurn(); //刷新界面显示显示
            StartRotate();   // 转动转盘
            CoolDown();
        }
        else
        {
            UI_HintMessage._.ShowMessage("需完整观看视频即可领取奖励");
        }
    }
    public EStage Stage
    {
        get => _stage;
        set
        {
            if (value != _stage)
            {
                _stage = value;
                switch (value)
                {
                    case EStage.None:
                        _speed = 0;
                        _timerTurn = 0;
                        _mask .SetActive(false);
                        break;
                    case  EStage.Stage1:
                        _mask.SetActive(true);
                         // _itemIndex = _rewardIndex;
                        _targetAngle = 360f;
                        _speed = 0;
                        _moveTime = 2f;
                        break;
                    case  EStage.Stage2:
                        _speed = _maxSpeed;
                        float number = UnityEngine.Random.Range(2, 5);
                        float total = number * 360 + (_targetAngle - _turnTableDraw.transform.eulerAngles.z);
                        _moveTime = total / _speed;
                        break;
                    case  EStage.Stage3:
                        DDebug.Log(_turnTableDraw.transform.eulerAngles.z.ToString());
                        _moveTime = 3;
                        _timerTurn = 0;
                        _targetAngle = _itemIndex * 60f;

                        float target =360+ (_targetAngle + _turnTableDraw.transform.eulerAngles.z);
                        _moveTime = target / (_speed/2);
                        
                        break;
                    case EStage.Stage4:
                        //GL_Game._instance._tableConfig.Callback = _callback;
                        // GL_Game._instance._tableConfig.ReportDraw();
                        TipsTurnGet(_rewards);
                        
                     GL_PlayerData._instance.SendGamecoreConfig(EGamecoreType.Turntable,(() =>
                     {
                         RefreshPage();
                     }));
                        
                        Stage = EStage.None;
                        break;

                }
            }
        }
    }
    
    private float _targetAngle;

    private void StartRotate()
    {
        GL_PlayerData._instance.SendGamecoreAccept(EGamecoreType.Turntable, 1, CB_Reward);
    }

    public  void TurnUpdate()
    {
        if (Stage == EStage.Stage1)
        {
            _timerTurn += Time.deltaTime;
            _speed = Mathf.Lerp(0, _maxSpeed, _timerTurn / _moveTime);
            if (_timerTurn > _moveTime)
            {
                Stage = EStage.Stage2;
            }
        }
        else if (Stage == EStage.Stage2)
        {
            _timerTurn += Time.deltaTime;
            if (_timerTurn > _moveTime)
            {
                Stage = EStage.Stage3;
            }
        }
        else if (Stage == EStage.Stage3)
        {
            _timerTurn += Time.deltaTime;
            _speed = Mathf.Lerp(_maxSpeed, 0, _timerTurn / _moveTime);
            
            if (_timerTurn > _moveTime)
            {
                
                Stage = EStage.Stage4;
            }
        }
        float value = Time.deltaTime;
        // _speed = 400f;
         // DDebug.LogError("~~~_speed: " + value);
         //
         // DDebug.LogError("~~~_speed: " + value);
         
        _turnTableDraw.Rotate (Vector3.back *(_speed*value) /*(_speed *  value/ 10f)*/);
    }
    
    #endregion

    #region 界面显示
    
    /// <summary>
    /// 获取真实数值
    /// </summary>
    private void GetRealCount()
    {
        //获取转盘剩余次数C
        _drawTimeNow= _drawConfig.dayAcceptTimes - _drawConfig.dayProgress;

        _drawTimeNow = _drawTimeNow <= 0 ? 0 : _drawTimeNow;
        
        
        _description.text = String.Format(descripString, _drawTimeNow);

        _fill.fillAmount = (float) _drawConfig.dayProgress / _drawConfig.dayAcceptTimes;
        _barText.text = string.Format("抽奖{0}次额外赠送0.5元微信零钱",_drawTimeNow);
        // _drawTimeNow =GL_PlayerData._instance.DrawConfig.drawLimit;
        
        if (_drawConfig.dayProgress==0)
        {
            _playAd.SetActive(false);
        }
        else
        {
            _playAd.SetActive(true);
        }
    }

    /// <summary>
    /// 刷新界面显示
    /// </summary>
    private void ChangeTurn()
    {
        Stage = EStage.None;
    }
    //
    // private void ShowTableCount()
    // {
    //     _drawTimeNow--;
    //     _drawTime.text = (_drawTimeNow.ToString()) + "/" + (_drawTimeMax.ToString());
    // }

    /// <summary>
    /// 进入界面旋转图标
    /// </summary>
    private void GetRotate()
    {
        for (int i = 0; i < _iconCenter.childCount; i++)
        {
            _iconCenter.GetChild(i).eulerAngles = new Vector3(0,0,(360/6f)*i);
            // _iconCenter.GetChild(i).GetChild(0).GetComponent<Image>().sprite = data._rewardSprite;
        }
    }

    private int _rewardIndex;

    private Rewards _rewards;

    private List<int> coinCount = new List<int>()
    {
        2000,
        10000,
    };

    private List<Rewards> _rewardses = new List<Rewards>();
    /// <summary>
    /// 抽奖回调
    /// </summary>
    /// <param name="Accept"></param>
    private void CB_Reward(Net_CB_GamecoreAccept Accept)
    {
        
        _rewards = Accept.rewards[0];


        _rewardses = Accept.rewards;
        _itemIndex = 0;
        switch (_rewards.type)
        {
            case 1: //领取金币
               GetCoin(_rewards.num);
                break;
            case 13: //提现到账
                switch (_rewards.num)
                {
                    case 30:
                        _itemIndex = 1;
                        break;
                    case 50:
                        _itemIndex = 2;
                        break;
                }
                break;
        }
        
        Stage = EStage.Stage1;
        
    }

    private Timer _turn;
    private State _state;
    private DateTime _turnTime;
    private void CoolDown()
    {
        _state = State.Wait;
        var offset =/* now +*/ 30;
        _turnTime = DateTime.Now;
        _turnTime = _turnTime.AddSeconds(offset);
        // if (_timer == null)
        // {
        //     _timer = new Timer(null, true);
        // }
        _turn.StartCountdown(_turnTime,(() =>
        {
            _state = State.Get;
            _turn?.Stop();
            _getText.text = "开始抽奖";
        }), _getText);
    }


    
    
    enum State
    {
        Wait,
        Get
    }

    
    /// <summary>
    /// 金币额度
    /// </summary>
    /// <param name="number"></param>
    private void GetCoin(int number)
    {
        _itemIndex = 4;
        for (int i = 0; i < coinCount.Count; i++)
        {
            if (number<coinCount[i])
            {
                switch (i)
                {
                  case 0:
                      _itemIndex = 0;
                      return;
                  case 1:
                      _itemIndex = 5;
                      return;
                }
            }
        }
    }

    /// <summary>
    /// 获得奖励提示
    /// </summary>
    public void TipsTurnGet(Rewards rewards)
    {
        switch (rewards.type)
        {
            case 1:
                GL_PlayerData._instance.Coin += rewards.num;
                // Fly_Manager._instance.MainUpFly(EFlyItemType.Coin, Vector3.zero,true);
                // UI_HintMessage._.ShowMessage($"恭喜您，领取{rewards.num}金币！");
                GL_RewardLogic._instance.GetReward(_rewardses,true);
                // Invoke("HideMainUp",3f);
                break;
            case 3:
                GL_PlayerData._instance.Coin += rewards.num;
                // Fly_Manager._instance.MainUpFly(EFlyItemType.Bogus, Vector3.zero,true);
                // UI_HintMessage._.ShowMessage($"恭喜您，领取{(rewards.num/100f).ToString("0.00")}元红包！");
                GL_RewardLogic._instance.GetReward(_rewardses,true);
                // Invoke("HideMainUp",3f);
                break;
            case 13:
                float money = rewards.num /100f;
                EWithDrawType _eWithDrawType = EWithDrawType.Normal;
                if (rewards.num==30)
                {
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.TurnTableLowReward);
                }
                else
                {
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.TurnTableHighReward);
                }
                var obj = new object[]
                {
                    money,
                    _eWithDrawType
                };
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WithdrawSuccess, obj);
                break;
        }
    }
    private UI_IF_MainUp _uiIfMainUp;
    private void HideMainUp()
    {
        _uiIfMainUp.SetActive(false);
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    private void ExitPage()
    {
        if ( Stage != EStage.None)
            return;
        
        CloseUIForm();
    }


    /// <summary>
    /// 0.5元提现
    /// </summary>
    private void GetWithDraw()
    {
        if (_drawConfig.dayProgress >= _drawConfig.dayAcceptTimes)
        {
            GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_WithDrawTurn, go =>
            {
                if (go)
                {
                    GL_PlayerData._instance.SendGamecoreAccept(EGamecoreType.Turntable, 2, (accept =>
                    {
                        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.TurnTableGetFinal);
                        TipsTurnGet(accept.rewards[0]);
                    } ));
                }
                else
                {
                    UI_HintMessage._.ShowMessage("完整观看视频完成提现！");
                }
            },GL_ConstData.SceneID_Turn);
            
          
        }
        else
        {
            UI_HintMessage._.ShowMessage("当前抽奖进度不足！");
        }
       
    }




    #endregion
}




