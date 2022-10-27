using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DataModule;
using Excel.Log;
using Spine;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

public class FF_LevelObj : UI_BaseItem
{
    #region 参数

    /// <summary>
    /// 当前对象的任务ID
    /// </summary>
    public int taskId;
    /// <summary>
    /// 奖励数量
    /// </summary>
    private float _number;
    /// <summary>
    /// 获取奖励的类型
    /// </summary>
    private int _type;
    /// <summary>
    /// 任务类型
    /// </summary>
    private int _taskType;
    /// <summary>
    /// 按键图片显示
    /// </summary>
    public List<Sprite> iconBtn;
    /// <summary>
    /// 任务描述文本
    /// </summary>
    private int _description;


    private string _levelDescription = "累计完成{0}关";
    
    private List<string> _tipsDescription = new List<string>()
    {
        "领取成功",
        "请先完成任务"
    };


    private UI_IF_Main _main;
    
    #endregion


    #region 游戏对象

    /// <summary>
    /// 领取奖励按键
    /// </summary>
    private Transform _taskBtn;
    /// <summary>
    /// 任务进度显示文本
    /// </summary>
    private Text _planCount;
    /// <summary>
    /// 任务进度条
    /// </summary>
    private Slider _planBar;
    /// <summary>
    /// 任务描述
    /// </summary>
    private Text _taskDescription;
    // /// <summary>
    // /// 任务图标
    // /// </summary>
    // private Image _taskIcon;
    /// <summary>
    /// 任务目标数量
    /// </summary>
    private Text _taskCount;

    private Transform _tab;
    
    private Image _btnImage;
    private Text _btnText;
    //视频图标
    // private Image _popupPlayImage;
    // private Transform _effect;
    #endregion

    
    /// <summary>
    /// 任务配置信息
    /// </summary>
    [HideInInspector]
    public Net_CB_MilestoneTaskInfo _milestoneTask;
    
    // private GL_DownloadFile _downloadFile;  //下载文件
    public override void Refresh<T>(T data, int dataIndex)
    {
        // DDebug.LogError("***** 刷新任务对象" + dataIndex);
        base.Refresh(data, dataIndex);
        if (data is Net_CB_MilestoneTaskInfo netData)
        {
            // taskId = transform.GetSiblingIndex();
            // DDebug.LogError("***** ID" + taskId);
            // DDebug.LogError("***** 刷新任务对象" + GL_Game._instance._taskConfig._taskArrs.Count);
            _milestoneTask = netData;
            
            
            taskId = _milestoneTask.id;
        }
        ChangeMessage();
    }
    //
    public override void Init()
    {
        base.Init();
        _taskBtn = UnityHelper.FindTheChildNode(gameObject, "TaskGet_Btn");
        _planCount = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "TaskPlan_Count");
        _planBar = UnityHelper.GetTheChildNodeComponetScripts<Slider>(gameObject, "TaskPlan_Bar");
        _taskDescription = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "TaskDescription");
        // _taskIcon = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "Task_Icon");
        _taskCount = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Task_Count");

        _btnImage = _taskBtn.GetComponent<Image>();
        _btnText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_taskBtn.gameObject, "Btn_Text");
        // _popupPlayImage = UnityHelper.GetTheChildNodeComponetScripts<Image>(_taskBtn.gameObject, "PopupPlay");

        // _effect = UnityHelper.FindTheChildNode(gameObject, "Effect");

        _taskBtn.GetComponent<Button>().onClick.AddListener(Click);

        _tab = UnityHelper.FindTheChildNode(gameObject, "Tab");
        _main = UIManager.GetInstance().GetUI(SysDefine.UI_Path_Main) as UI_IF_Main;
    }


    /// <summary>
    /// 更新文本描述
    /// </summary>
    /// <param name="a">旧字符</param>
    /// <param name="b">新字符</param>
    /// <param name="c">需要更新的字段</param>
    /// <returns></returns>
    private string changeMessage(string a, string b, string c)
    {
        StringBuilder str = new StringBuilder(c);
        str.Replace(a, b);
        return str.ToString();
    }

    [HideInInspector]
    public BtnState _btnState;
    /// <summary>
    /// 更新显示信息
    /// </summary>
    public void ChangeMessage()
    {
        // if (_popupPlayImage != null)
        //     _popupPlayImage.gameObject.SetActive(false);
        // Net_CB_TaskArr config = _netCbTaskArr;
        EItemType itemType = EItemType.Coin;
        //任务进度条
        _planBar.value = (float)(GL_PlayerData._instance.CurLevel-1)/ _milestoneTask.level;
        //任务文本
        _planCount.text = (GL_PlayerData._instance.CurLevel-1) + "/" + _milestoneTask.level;
        //获取奖励数量
         _number = _milestoneTask.winRewards[0].num;
        //获取奖励类型
        _type = _milestoneTask.winRewards[0].type;

        if (_milestoneTask.winRewards.Count>1 )
        {
            ShowTab(true);
        }
        else
        {
            ShowTab(false);
        }
        
        // _taskType = _netCbTaskArr.type;
        // float number = _netCbTaskArr.winNum;
      
        //任务数量描述，已停用
        // _taskCount.text = DataModuleManager._instance.TableDailyTaskData_Dictionary[_netCbTaskArr.type].RewardCount;
        // if (_netCbTaskArr.winRewardType == 3)
        // {
        //     _taskCount.text = String.Format(_taskCount.text, (int)_number * 0.01f);
        // }
        // else
        // {
        //     _taskCount.text = String.Format(_taskCount.text, (int)_number);
        // }

        //任务数量描述
        switch (_type)
        {
            //金币奖励
            case 1:
                _taskCount.text = string.Format("X{0}元", _number);
                break;
            //现金奖励
            case 3:
                string count = (_number * 0.01f).ToString("0.00");
                _taskCount.text = string.Format("X{0}元",count);
                break;
            //提现券奖励
            case 7:
                _taskCount.text = string.Format("X{0}元", _number);
                
                break;
        }
        // _taskCount.text = String.Format(_taskCount.text, (int)_number);
        
        //任务描述
        _taskDescription.text = string.Format(_levelDescription, _milestoneTask.level);
        // _taskDescription.text = String.Format(_taskDescription.text, _netCbTaskArr.condition);

        itemType = (EItemType)_type;
        SRewardData data = new SRewardData(itemType);
        
        // _taskIcon.sprite = data._rewardSprite; //更新 图片显示
        
        // _effect.gameObject.SetActive(false);
        switch (_milestoneTask.status)
        {
            case 1:
                _btnState = BtnState.HasGet;
                _btnImage.sprite = iconBtn[0];
                _btnText.text = "已领取";
                break;
            case 2:

                if ((GL_PlayerData._instance.CurLevel-1) >= _milestoneTask.level)
                {
                    _btnState = BtnState.Finished;
                    _btnImage.sprite = iconBtn[1];
                    _btnText.text = "领取";
                    // if (_popupPlayImage != null)
                    //     _popupPlayImage.gameObject.SetActive(true);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(_btnText.transform.parent.transform as RectTransform); //适应位置
                    // _effect.gameObject.SetActive(true);
                }
                else
                {
                    _btnState = BtnState.UnFinish;
                    _btnImage.sprite = iconBtn[2];
                    _btnText.text = "未完成";
                }
                
                break;
                // if ((TaskType)_netCbTaskArr.type != TaskType.NewPlayer && itemType == EItemType.CashCoupon && _netCbTaskArr.winNum >= GL_ConstData.TaskRewardAdNumber)
                // {
                //     if (_popupPlayImage != null)
                //         _popupPlayImage.gameObject.SetActive(true);
                //     LayoutRebuilder.ForceRebuildLayoutImmediate(_btnText.transform.parent.transform as RectTransform);
                // }
                // _effect.gameObject.SetActive(true);
              
            // case 3:
            //     _btnState = BtnState.UnFinish;
            //     _btnImage.sprite = iconBtn[2];
            //     if(!IsDownloadTask())
            //     {
            //         _btnText.text = "待领取";
            //     }
            //     else
            //     {
            //         //下载任务
            //         if(_downloadFile == null)
            //             _btnText.text = "下载";
            //         else
            //             _btnText.text = "下载中";
            //     }
            //
            //     break;
        }
    }



    /// <summary>
    /// 按键点击
    /// </summary>
    private void Click()
    {
        GL_AudioPlayback._instance.Play(2);
        // UIManager.GetInstance().CloseUIForms(strUIFromName);
        switch (_btnState)
        {

            case BtnState.HasGet:  //已领取
                // UI_HintMessage._.ShowMessage(transform, DataModuleManager._instance.TableDailyTaskData_Dictionary[20].Description);
                // _uiIfTask.ClosePage();
                break;
            case BtnState.UnFinish: //去完成
                UI_HintMessage._.ShowMessage(/*transform,*/ _tipsDescription[1]);
                // _uiIfTask.ClosePage();
                // Task_Func._instance.ExeTask((ETaskType)_netCbTaskArr.type); 
                // GL_Game._instance._taskConfig.ExeTask((TaskType) _netCbTaskArr.type);
                // UI_HintMessage._.ShowMessage(transform, DataModuleManager._instance.TableDailyTaskData_Dictionary[15].Description);
                // _uiIfTask.ClosePage();
                 // if(IsDownloadTask()) 
                 // {
                 //    //下载任务特殊处理
                 //    if(_downloadFile == null && !string.IsNullOrEmpty(_netCbTaskArr.downLoadUrl))
                 //    {
                 //        string url = _netCbTaskArr.downLoadUrl;
                 //        int index = url.LastIndexOf("/");
                 //        var fileName = url.Substring(index + 1, url.Length - index - 1);
                 //        _downloadFile = new GL_DownloadFile(url, fileName, (s) =>
                 //        {
                 //            DDebug.LogError("~~~task下载任务: 完成");
                 //            //下载完了
                 //            Net_TaskDownloadComplete msg = new Net_TaskDownloadComplete();
                 //            msg.taskId = _netCbTaskArr.id;
                 //            GL_ServerCommunication._instance.Send(Cmd.TaskDownloadComplete, JsonUtility.ToJson(msg), (ss) =>
                 //            {
                 //                DDebug.LogError("~~~task下载任务: 通知完成");
                 //
                 //                if(GL_Game._instance.GameState == EGameState.GameMain)
                 //                {
                 //                    //下载接口发送成功
                 //                    if (gameObject.activeInHierarchy)
                 //                    {
                 //                        GL_PlayerData._instance.GetTaskConfig();
                 //                    }
                 //                    else
                 //                    {
                 //                        DDebug.LogError("~~~task下载任务: 刷新主页任务红点");
                 //                        //任务界面显示中, 则刷新任务列表
                 //                        Logic.System.NetWork.YS_NetLogic._instance.RefreshTask();
                 //                    }
                 //                }
                 //            });
                 //
                 //            //通知sdk 开始安装应用
                 //            string path = Application.persistentDataPath + "/" + fileName;
                 //            GL_SDK._instance.InstallApk(path);
                 //        });
                 //       
                 //        ChangeMessage();
                 //    } 
                 // }
                 // else 
                 // {
                 //    UI_HintMessage._.ShowMessage(transform, DataModuleManager._instance.TableDailyTaskData_Dictionary[21].Description);
                
                 //    // _uiIfTask.ClosePage();
                 //    // Task_Func._instance.ExeTask((ETaskType)_netCbTaskArr.type); 
                 //    GL_Game._instance._taskConfig.ExeTask((TaskType) _netCbTaskArr.type);
                 // } 
                 break;
            case BtnState.Finished: //领取
                RefreshReward();
                // GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_GetLevelReward, go =>
                // {
                //     if (go)
                //     {
                //         RefreshReward();
                //     }
                // });
                
                // if ((TaskType)_netCbTaskArr.type != TaskType.NewPlayer && 
                //     _netCbTaskArr.winRewardType == 7  
                //     && _netCbTaskArr.winNum >= GL_ConstData.TaskRewardAdNumber
                //     && !GL_GuideManager._instance._isGuideing)
                // {
                //     GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_DailyTask, go =>
                //     {
                //         if (go)
                //         {
                //             RefreshReward();
                //         }
                //     });
                // }
                // else
                // {
                //     RefreshReward();
                // }
                break;
        }
    }

    private void ShowTab(bool show)
    {
        _tab.SetActive(show);
    }

    private void RefreshReward(int pagetype = 2)
    {
        _btnImage.sprite = iconBtn[0];
         // if (_popupPlayImage != null)
         //     _popupPlayImage.gameObject.SetActive(false);
         // LayoutRebuilder.ForceRebuildLayoutImmediate(_btnText.transform.parent.transform as RectTransform); //适应位置
         _btnText.text = "已领取";
         _milestoneTask.status = 1;
       
         transform.SetSiblingIndex(transform.parent.childCount);
        
         // if (GL_Game._instance._taskConfig._taskPlan.progress < GL_Game._instance._taskConfig._taskPlan.condition)
         // {
         //     GL_Game._instance._taskConfig._taskPlan.progress++;
         // }
         // if ((TaskType)_netCbTaskArr.type == TaskType.NewPlayer)
         // {
         //     transform.gameObject.SetActive(false);
         // }
         // else
         // {
         //     if (GL_Game._instance._taskConfig._taskPlan.progress < GL_Game._instance._taskConfig._taskPlan.condition)
         //     {
         //         GL_Game._instance._taskConfig._taskPlan.progress++;
         //     }
         //     transform.SetSiblingIndex(GL_Game._instance._taskConfig._taskArrs.Count);
         // }
         _btnState = BtnState.HasGet;
        
         //***************************  里程碑任务领取埋点
         //回报
         _taskType = 0;

        if (_milestoneTask.winRewards.Count > 1)
        {
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
            GL_AD_Logic._instance.Vibrate(300, 1);

        }
        else
        {
            GL_Game._instance._taskConfig.GetTaskRewardReport(pagetype, (int)_number, taskId, _type, _taskType, false);
        }

    }
    
    //是否是下载任务
    // private bool IsDownloadTask()
    // {
    //     if (_netCbTaskArr == null)
    //         return false;
    //
    //     if ((TaskType)_netCbTaskArr.type == TaskType.DownLoadIce
    //         || (TaskType)_netCbTaskArr.type == TaskType.DownLoadIdom
    //         ||(TaskType)_netCbTaskArr.type == TaskType.DownLoadLifeWin)
    //     {
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }
    
}




