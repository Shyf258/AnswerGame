//2020.02.07    关林
//关卡玩法-答题

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataModule;
using Logic.Fly;
using Logic.System.NetWork;
using Newtonsoft.Json;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using Object = System.Object;


public class GL_GameMode_Answer : GL_GameMode
{
    public List<TableAnswerInfoData> _levelList;

    [HideInInspector]
    public TableAnswerInfoData _levelInfo; //关卡信息

  

    #region 初始化
    public override void Init()
    {
        ParseTable();
    }

    //解析表格
    private void ParseTable()
    {
        _levelList = DataModuleManager._instance.TableAnswerInfoData_Dictionary.Values.ToList();
    }


    #endregion

    #region 更新
    public override void DoUpdate(float dt)
    {
        if(LevelState == ELevelState.Playing)
        {
            //游戏阶段检测,输入
            var fingers = GL_Input.GetFingers(false);
            if(fingers != null && fingers.Count > 0)
            {
                if(fingers[0].Down)
                {
                    Vector3 v = UIUtils.GetUGUICam().ScreenToWorldPoint(fingers[0].ScreenPosition);
                    RaycastHit2D hitInfo = Physics2D.Raycast(new Vector2(v.x, v.y), new Vector2(v.x, v.y), 0.1f, GL_ConstData.LayerMask_UI);
                    //if(hitInfo.collider != null)
                    //{
                    //    UI_WordBase word = hitInfo.collider.GetComponentInParent<UI_WordBase>();
                    //    if(word != null)
                    //    {
                    //        ClickLogic(word);
                    //    }
                    //}
                }
            }
        }
    }
    #endregion

    #region 接口

    public override void Clear()
    {

    }

    public void CreateLevel(int levelIndex)
    {
        levelIndex = GL_SceneManager._instance.CalculateReallevelIndex(levelIndex);
        //创建新关卡
        levelIndex -= 1;
        if(levelIndex < 0) 
        {
            levelIndex = 0;
        }

        _levelInfo = _levelList[levelIndex];
        GL_GameEvent._instance.SendEvent(EEventID.RefreshGameMode);
    }

    #endregion


    //结算等待
    public override void DoSettleWait()
    {
        LevelState = ELevelState.None;
        //答对语音播报
        int index = UnityEngine.Random.Range(10, 17);
        GL_AudioPlayback._instance.PlayTips(index);
        GL_AudioPlayback._instance.Play(7);
        // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
        MethodExeTool.Loop(Net_SettleWait, 5f, -1);
    }

    //发送消息, 等待回调
    private void Net_SettleWait()
    {
        Net_Rq_Upgrad com = new Net_Rq_Upgrad();
        com.type = 3;
        GL_ServerCommunication._instance.Send(Cmd.Upgrade, JsonHelper.ToJson(com), CB_SettleWait);
    }

    private void CB_SettleWait(string json)
    {
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.LevelUp);
        
        //GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.ActiveGame);
        LevelState = ELevelState.Settle1;
        if (GL_PlayerData._instance.SystemConfig != null)
        {
            GL_PlayerData._instance.SystemConfig.userLevel += 1;
            GL_PlayerData._instance.UserDayLevel += 1;
        }

        Net_CB_Reward msg = JsonConvert.DeserializeObject<Net_CB_Reward>(json);
        _curRewards = msg.rewards[0];
        if (_curRewards == null)
            return;
        // UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_NetLoading);
        MethodExeTool.CancelInvoke(Net_SettleWait);
        ShowCashCoin();
        //if (!GL_CoreData._instance.AbTest)
        {
            GL_PlayerData._instance.GetTaskConfig();
        }
    }

    private void ShowCashCoin()
    {
        Action<int> ac1 = (int value) =>
        {
            //刷新关卡
            GL_SceneManager._instance.CreateGame();

            _uiIfMain.MoveChoiceGroup(true);
            _uiIfMain.MoveBack();

            //领取奖励
            Cb_ShowCashCoin(value);

            // //刷新里程碑
            // AutoGetSliderReward();


            // GL_PlayerData._instance._canChangeWithDraw = true;
            GL_GuideManager._instance.TriggerGuide(EGuideTriggerType.UIMain);
        };

        Object[] objects = { (EItemType)_curRewards.type, _curRewards.num, ac1 };
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetReward, objects);
    }
    /// <summary>
    /// 双奖励模式
    /// </summary>
    private void Cb_ShowCashCoin(int value)
    {
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.CompleteLevel.ToString() + (GL_PlayerData._instance.UserDayLevel));
        GetRed(value);
        // _withdrawCallback ?.Invoke();
        if (value == 1)
        {
            GetCoin();
        }
    }
    private void GetRed(int value)
    {
        bool isPlayAD = false;
        //普通领取
        if (value != 1  && GL_PlayerData._instance.AppConfig.isPassive!=1 && GL_PlayerData._instance.CurLevel >= 5)
        {
            //四关轮播激励视频
            int count = (GL_PlayerData._instance._idiomConjCoinReward) % 4;

            switch (count)
            {
                case 0:
                    DDebug.LogError("***** 播放被动插屏");
                    GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Interstitial_AllDialog);
                    GL_PlayerData._instance._idiomConjCoinReward++;
                    break;
                case 3:
                    DDebug.LogError("***** 播放被动激励");
                    GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_GetDoubleReward, delegate (bool b)
                    {
                        if (b)
                        {
                            GL_PlayerData._instance._idiomConjCoinReward++;
                        }
                    });
                    break;
                default:
                    GL_PlayerData._instance._idiomConjCoinReward++;
                    break;
            }

            if (GL_PlayerData._instance._idiomConjCoinReward > 4)
            {
                GL_PlayerData._instance._idiomConjCoinReward = 1;
            }
        }

        switch (_curRewards.type)
        {
            //假现金
            case (int) EItemType.Bogus:
                GL_PlayerData._instance.Bogus = GL_PlayerData._instance.Bogus + _curRewards.num;
                GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency, new EventParam<EFlyItemType>(EFlyItemType.Bogus));
                //float offset = GL_PlayerData._instance.TotalBogus - GL_PlayerData._instance.Bogus_Convert;
                UI_HintMessage._.ShowMessage($"恭喜！获得{_curRewards.num / 100f}元现金。");
                break;
            //假现金
            case (int) EItemType.Coin:
                GL_PlayerData._instance.Coin = GL_PlayerData._instance.Coin + _curRewards.num;
                GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency, new EventParam<EFlyItemType>(EFlyItemType.Coin));
                //float offset = GL_PlayerData._instance.TotalBogus - GL_PlayerData._instance.Bogus_Convert;
                UI_HintMessage._.ShowMessage($"恭喜！获得{_curRewards.num }金币。");
                break;
        }
      
    }

    private void GetCoin()
    {
        int level = GL_PlayerData._instance.CurLevel;

        YS_NetLogic._instance.UpgradeDouble(level, 3, (rewards =>
        {
            switch ((EItemType)rewards.type)
            {
                case EItemType.Coin:
                    if (rewards.num > 0)
                    {
                        //获取奖励
                        GL_PlayerData._instance.Coin += rewards.num;
                        GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency, new EventParam<EFlyItemType>(EFlyItemType.Coin));
                        UI_HintMessage._.ShowMessage($"恭喜！获得{rewards.num}金币");
                    }
                    break;
            }
        }));

        if (GL_PlayerPrefs.GetInt(EPrefsKey.IsReceiveNewPlayer) == 0)
        {
            GL_PlayerData._instance.GetNewPlayerReward();
        }
    }

    private void ShowAd()
    {
        //switch (GL_PlayerData._instance.IsPlayAdCirculation())
        //{
        //    //广告播放类型 0不播放 1插屏 2激励
        //    case 0:
        //        DDebug.LogError("***** 不播放广告");
        //        break;
        //    case 1:
        //        DDebug.LogError("***** 播放插屏");
        //        GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Interstitial_AllDialog);
        //        break;
        //    case 2:
        //        DDebug.LogError("***** 播放激励");
        //        GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_RightPassivity, go =>
        //        {

        //        });
        //        break;
        //}
    }

    //public void ShowCoin() 
    //{
    //    DDebug.Log("@@@@@@显示获得金币");

    //    SRewardData rewardData = new SRewardData((EItemType)_curRewards.type, ERewardSource.None);
    //    GL_RewardLogic._instance.RewardSettlement(rewardData,_curRewards.num);

    //    Action<int> ac1 = (int value) => { Cb_ShowCoin(value); };
    //    Object[] objects = { rewardData, _curRewards.num, ac1, EItemType.Coin};
    //    // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetReward, objects);
    //}

    //private void Cb_ShowCoin(int type)
    //{
    //   /* if(type == 0)
    //    {
    //        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Success, EItemType.Coin);
    //    }
    //    else */
    //   if (type == 2)
    //   {
    //        //普通领取
    //        GL_PlayerData._instance.Coin += _curRewards.num;

    //        switch ((EItemType)_curRewards.type)
    //        {
    //            case EItemType.Coin:
    //                ShowCoin(_curRewards);
    //                break;
    //        }
    //   }
    //   else if (type == 1)
    //   {
    //        int level = GL_PlayerData._instance.CurLevel;
    //        YS_NetLogic._instance.UpgradeDouble(level, (rewards =>
    //        {
    //            switch ((EItemType)rewards.type)
    //            {
    //                case EItemType.Coin:
    //                    ShowCoin(rewards);
    //                    break;
    //            }
    //        }));
    //   }
    //}

    //private void ShowCoin(Rewards rewards)
    //{
    //    Action action = () =>
    //    {

    //        GL_PlayerData._instance.Coin += rewards.num;
    //        GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency);
    //        // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Success,0);
    //    };

    //    Object[] objects = { rewards.num, action };

    //    // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Coin, objects);
    //}

    //// private bool _showAds;
    //public void ShowCash()
    //{
    //    UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_NetLoading);
    //    // GL_PlayerData._instance.Bogus = (GL_PlayerData._instance.Bogus * 100 +GL_PlayerData._instance._positionConfig.winNum)/100;
    //    DDebug.Log($"@@@@@@显示打开红包{ GL_PlayerData._instance.Bogus}");
    //    // _showAds = _rightCount % 5 == 0;
    //    Action<bool> action = (open) =>
    //    {
    //        if (open)
    //        {
    //            CashReward(open);
    //        }
    //        else
    //        {
    //            GetAdReward();
    //        }

    //    };

    //    // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_OpenRedPack,new object[] {action,true,!_showAds});

    //}

    private void GetAdReward()
    {
        // GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_AnswerRight, CashReward);
    }

    private void CashReward(bool isOpen)
    {
        if (isOpen)
        {
            float count = (float) _curRewards.num / 100;
            Action ac = () =>
            {
                GL_PlayerData._instance.Bogus += _curRewards.num;
                GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency, new EventParam<EFlyItemType>(EFlyItemType.Bogus));
                // GL_GameEvent._instance.SendEvent(EEventID.RefreshMainLimit);
                // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.AnswerRight_Goon);
                // if (GL_PlayerData._instance. _canWithDraw)
                // {
                //     GL_PlayerData._instance._showWithDraw = false;
                //     UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Withdraw);
                // }
            };
                
            // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Success,0);
            // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Cash,new object[]{ac,false ,count});
        }
    }

    

    public static IEnumerator WaitForSecond(float time,Action action=null)
    {
        yield return  new WaitForSeconds(time);
        action?.Invoke();
    }

    protected override void DoFail()
    {
        LevelState = ELevelState.None;
        GL_AudioPlayback._instance.PlayTips(6);
        Net_Rq_Upgrad com = new Net_Rq_Upgrad();
        com.type = 3;
        // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
        GL_ServerCommunication._instance.Send(Cmd.Upgrade, JsonHelper.ToJson(com), delegate(string s)
        {
            if (GL_PlayerData._instance.SystemConfig != null)
            {
                GL_PlayerData._instance.SystemConfig.userLevel += 1;
                GL_PlayerData._instance.UserDayLevel += 1;
            }

            Net_CB_Reward msg = JsonConvert.DeserializeObject<Net_CB_Reward>(s);
            _curRewards = msg.rewards[0];
            if (_curRewards == null)
                return;


            Action<int> ac1 = (int value) =>
            {
                //刷新关卡
                GL_SceneManager._instance.CreateGame();
                _uiIfMain.MoveChoiceGroup(true);
                _uiIfMain.MoveBack();
                //领取奖励
                Cb_ShowCashCoin(value);

                // //刷新里程碑
                // AutoGetSliderReward();

                GL_GuideManager._instance.TriggerGuide(EGuideTriggerType.UIMain);
            };

            Object[] objects = { ac1, _curRewards};
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Fail, objects);
        });
    }

    private void Fail(string json)
    {
        
      
    }

    private Action _action;
    private object data;
    private UI_IF_Main _uiIfMain;

    private int _rewardCount;

    private int _rewardType;

    private int _curlevel;

    /// <summary>
    /// 关卡计数
    /// </summary>
    private int _rightCount =0 ;
    
    //UI选择了答案
    public void UI_Choice(int index ,Action action =null)
    {
        //GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.ChoiceAnswer);
        if (_levelInfo == null)
            
            return;
        _action = action;

        if (_uiIfMain == null)
        {
            _uiIfMain = UIManager.GetInstance().GetUI(SysDefine.UI_Path_Main) as UI_IF_Main;
        }
        
        #region 纯净版

        

        
#if PureVersion
        if (_levelInfo.CorrectAnswer.Contains(index))
        {
            try
            {
                UI_HintMessage._.ShowMessage(_uiIfMain.transform,"回答正确");
            }
            catch 
            {
                _uiIfMain = UIManager.GetInstance().GetUI(SysDefine.UI_Path_Main) as UI_IF_Main;
                UI_HintMessage._.ShowMessage(_uiIfMain.transform,"回答正确");
            }
           
        }
        else
        {
            try
            {
                UI_HintMessage._.ShowMessage(_uiIfMain.transform,"回答错误");
            }
            catch 
            {
                _uiIfMain = UIManager.GetInstance().GetUI(SysDefine.UI_Path_Main) as UI_IF_Main;
                UI_HintMessage._.ShowMessage(_uiIfMain.transform,"回答错误");
            }
           
        }
        GL_PlayerData._instance.RightConfig.questionNum ++;
        CreateLevel(GL_PlayerData._instance.RightConfig.questionNum);
        GL_GameEvent._instance.SendEvent(EEventID.RefreshAnswerMode);
        return;
#endif
        #endregion
        //1.判断回答
        if(_levelInfo.CorrectAnswer.Contains(index))
        {
            _rightCount++;
            _uiIfMain.ShowAnswer(true);
            DDebug.LogError("~~~答对了");
             // 上报获取奖励 
             // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Game_AnswerRight);
             // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Activate_Game);
             //******************  当前答题进度 需确认
             // LevelState = ELevelState.Settle1;
             // StartCoroutine(WaiteSecend());
        }
        else
        {
            _uiIfMain.ShowAnswer(false);
            DDebug.LogError("~~~答错了");
            // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Game_AnswerWrong);
            //选择选择 是否看广告
            // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Fail,data);
            
            
            // DoFail();
            //刷新界面 （连对进度保持不变显示新的题目）
          
        }
       
        // GL_PlayerData._instance.RightConfig.questionNum ++;
        // CreateLevel(GL_PlayerData._instance.RightConfig.questionNum);
        // GL_GameEvent._instance.SendEvent(EEventID.RefreshAnswerMode);
        // 答题进度 提现获取配置 停用 **************
        // GL_PlayerData._instance.GetLevelWithDrawConfig(); 
        //完成答题 刷新题目
        data = null;
    }

    /// <summary>
    /// 获取里程碑奖励
    /// </summary>
    private void AutoGetSliderReward()
    {
        //_uiIfMain._levelSlider.GetAllCanGetReward();
        
        // GL_GameEvent._instance.SendEvent(EEventID.RefreshPosition);
    }
    
}

