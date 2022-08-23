using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

public partial class UI_IF_Main
{

    private Timer _timer;

    
    
    private List<string> _taskDescription = new List<string>()
    {
        "今日剩余<color=#ff0000>{0}</color>次",
        "领取转圈红包{0}个",
        "看视频领取金币{0}次",
        "通关{0}次",
        "每日登录",
    };
    // private Net_CB_TaskConfig _taskConfig;
    private double _cdTime; //cd时间
    #region 对象

    /// <summary>
    /// 次数提醒
    /// </summary>
    private Text _timesTips;

    /// <summary>
    /// 提现次数限制
    /// </summary>
    private Image _timesSlider;

    /// <summary>
    /// 领取金币按键
    /// </summary>
    private Button _getCoin;

    /// <summary>
    /// 按键图片
    /// </summary>
    private Image _btnImage;

    /// <summary>
    /// 按键时间显示
    /// </summary>
    private Text _timeText;
    private List<GameObject> _taskItemList;
    private Transform _coinPageContent;
    
    #endregion
    
    public void InitTask()
    {
        _timer = new Timer(this,true);
        Transform page = UnityHelper.FindTheChildNode(gameObject, "UI_OBJ_Task");
        _timesTips = UnityHelper.GetTheChildNodeComponetScripts<Text>(page.gameObject, "TimesTips");
        _timesSlider = UnityHelper.GetTheChildNodeComponetScripts<Image>(page.gameObject, "TaskPlan_Fill");
        _timeText = UnityHelper.GetTheChildNodeComponetScripts<Text>(page.gameObject, "UseTime");
        _coinPageContent = UnityHelper.FindTheChildNode(page.gameObject, "Content");
        _getCoin = UnityHelper.GetTheChildNodeComponetScripts<Button>(page.gameObject, "GoTask_Btn");
        _btnImage = _getCoin.GetComponent<Image>();
        RigisterButtonObjectEvent(_getCoin, go =>
        {
            BtnClick();
        });
    }

    
    public void FreshPage()
    {
        // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
        FreshMessage();
        
        FreshGetCoinTimes();
        
       
    } 
    private EDragRedpackState _btnState;
    private Net_CB_VideoRed _videoRed;
    private void FreshGetCoinTimes()
    {
        GL_PlayerData._instance.SendVideoRedpackConfig(EVideoRedpackType.VideoRedpack, () =>
        {
            _videoRed = GL_PlayerData._instance.GetVideoRedpackConfig(EVideoRedpackType.VideoRedpack);
            _timesTips.text = String.Format(_taskDescription[0],_videoRed.videoRedLimit);
            _timesSlider.fillAmount = (float) ( _videoRed.videoRedSize-_videoRed.videoRedLimit) / _videoRed.videoRedSize;
           
            if (_videoRed == null || _videoRed.videoRedLimit == 0)
            {
                _btnState = EDragRedpackState.Exhaust;
                return;
            }

            //检测是否在CD中
            _cdTime = GL_PlayerData._instance.GetVideoRedpackCD(EVideoRedpackType.VideoRedpack);
            if(_cdTime > 0)
            {
                double cur = GL_Time._instance.CalculateSeconds();
                if (cur >= _cdTime)
                {
                    ShowBtnState(false);
                    _btnState = EDragRedpackState.Can;
                }
                else
                {
                    double time = _cdTime - cur;
                    FreshTime(time);
                    ShowBtnState(true);
                    _btnState = EDragRedpackState.CD;
                }
                return;
            }
            else
            {
                ShowBtnState(false);
            }

            _btnState = EDragRedpackState.Can;
            
            // if (_videoRed.intervalTime>0)
            // {
            //     
            //     ShowBtnState(true);
            // }
            // else
            // {
            //     ShowBtnState(false);
            // }
        });
    }

    private GL_Audio audio;
    private void Tips()
    {
        if (audio!=null && audio._isActivate)
        {
            return;
        }
        
        foreach (var VARIABLE in GL_PlayerData._instance.TaskConfig.tasks)
        {
            if (VARIABLE.status==2)
            {
               audio =   GL_AudioPlayback._instance.Play(20);
                break;
            }
        }
    }

    public bool ShowTask()
    {
        bool show =false;
        int taskcount =   GetCount();
        show = taskcount > 0;
        _tipsTaskText.text = taskcount.ToString();
        return show;
    }
    
    private int GetCount()
    {
        int count = 0;
        try
        {
            foreach (var VARIABLE in GL_PlayerData._instance.TaskConfig.tasks)
            {
                if (VARIABLE.status==2)
                {
                    count++;
                }
            }
        }
        catch 
        {
            
        }

        return count;
    }
    

    private void BtnClick()
    {
        // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_PureAnswer);
        // return;

        if (_btnState != EDragRedpackState.Can)
        {
            UI_HintMessage._.ShowMessage("冷却中，请稍等");
            return;
        }

        if (  _btnState == EDragRedpackState.Exhaust)
        {
            UI_HintMessage._.ShowMessage("今日次数不足");
        }
        
        GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_TaskPageGetCoin, (set) =>
        {
            if(set)
            {
                GL_PlayerData._instance.SendGetVideoRedpack(EVideoRedpackType.VideoRedpack, true, (msg) =>
                {
                    Action action = () =>
                    {
                        FreshGetCoinTimes();
                        GL_PlayerData._instance.GetTaskConfig();
                    };
                    object[] datas = { msg.rewards, action };
                    UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetResult, datas);
                });
            }
        });
        
    }


    private void FreshMessage()
    {
        // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
        GL_PlayerData._instance.GetTaskConfig(() =>
        {
            tasks = Array();
            GL_Tools.RefreshItem(tasks,_coinPageContent,taskItem,_taskItemList);
            Tips();
        });
       
    }



    private void FreshTime(double count)
    {
        int time = Convert.ToInt32(count);
        _timer.StartCountdown(time % 60, (time % 3600)/60, time/ 3600, (() =>
        { 
            _timer.Stop();
            ShowBtnState(false);
        }),_timeText);
    }

    private List<Net_CB_TaskArr> tasks;
    private List<Net_CB_TaskArr> Array()
    {
        
    List<Net_CB_TaskArr> tasks_1= new List<Net_CB_TaskArr>();
    List<Net_CB_TaskArr> tasks_2= new List<Net_CB_TaskArr>();
    List<Net_CB_TaskArr> tasks_3= new List<Net_CB_TaskArr>();


    foreach (var value in GL_PlayerData._instance.TaskConfig.tasks)
    {
        switch (value.status)
        {
            case 1: //已领取
                tasks_3.Add(value);
                break;
            case 2://未领取
                tasks_1.Add(value);
                break;
            case 3://已完成
                tasks_2.Add(value);
                break;
        }
    }
    AddList(tasks_1,tasks_2);
    AddList(tasks_1,tasks_3);

    return tasks_1;
    // _indexGet = 0;
    // for (int i = _coinPageContent.childCount-1; i <=0; i--)
    // {
    //     switch ( _coinPageContent.GetChild(i).GetComponent<FF_TaskObj>()._netCbTaskArr.status)
    //     {
    //         case 1:
    //             _coinPageContent.GetChild(i).SetSiblingIndex( _coinPageContent.childCount-1);
    //             break;
    //         case 2:
    //             _coinPageContent.GetChild(i).SetSiblingIndex(0);
    //             _indexGet++;
    //             break;
    //         case 3:
    //             _coinPageContent.GetChild(i).SetSiblingIndex(_indexGet);
    //             break;
    //     }
    // }
    }

    private void AddList(IList a,IList b )
    {
        foreach (var item in b)
        {
            a.Add(item);
        }
    }

    /// <summary>
    /// 切换按键点击状态
    /// </summary>
    /// <param name="disable">不可点击</param>
    private void ShowBtnState(bool disable)
    {
        if (disable)
        {
            _btnImage.sprite = listSprite[1];
            _timeText.SetActive(true);
        }
        else
        {
            _btnState = EDragRedpackState.Can;
            _btnImage.sprite = listSprite[0];
            _timeText.SetActive(false);
        }
    }





}
