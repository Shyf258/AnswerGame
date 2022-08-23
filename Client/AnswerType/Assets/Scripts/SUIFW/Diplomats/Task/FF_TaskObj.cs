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

public class FF_TaskObj : UI_BaseItem
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
    private Image _planBar;
    /// <summary>
    /// 任务描述
    /// </summary>
    private Text _taskDescription;
    /// <summary>
    /// 任务图标
    /// </summary>
    private Image _taskIcon;
    /// <summary>
    /// 任务目标数量
    /// </summary>
    private Text _taskCount;

    private Image _btnImage;
    private Text _btnText;
    //视频图标
    private Image _popupPlayImage;

    private List<string> _taskDescriptionStr = new List<string>()
    {
        "今日剩余{0}次",
        "领取转圈红包{0}个",
        "看视频领取金币{0}次",
        "通关{0}次",
        "每日登录",
    };
    
    private string _taskMessage = "累计答对<color=#cf0400>{0}</color>道题";
    #endregion

    [HideInInspector]
    public Net_CB_TaskArr _netCbTaskArr;
    
    private GL_DownloadFile _downloadFile;  //下载文件
    public override void Refresh<T>(T data, int dataIndex)
    {
        // DDebug.LogError("***** 刷新任务对象" + dataIndex);
        base.Refresh(data, dataIndex);
        if (data is Net_CB_TaskArr netData)
        {
            // taskId = transform.GetSiblingIndex();
            // DDebug.LogError("***** ID" + taskId);
            // DDebug.LogError("***** 刷新任务对象" + GL_Game._instance._taskConfig._taskArrs.Count);
            _netCbTaskArr = netData;

            if (netData.type == 13)
            {
            
                GL_Tools.GetComponent<UI_GuideObject>(_taskBtn.gameObject);
            
            }
            else
            {
                UI_GuideObject obj = _taskBtn.gameObject.GetComponent<UI_GuideObject>();
            
                if (dataIndex != 0)
                {
                    if (obj != null)
                    {
                        obj.RemoveCanvas();
                        DestroyImmediate(obj);
                    }
                }
            }
            taskId = _netCbTaskArr.id;
        }
        ChangeMessage();
    }
    //
    public override void Init()
    {
        base.Init();
        _taskBtn = UnityHelper.FindTheChildNode(gameObject, "TaskGet_Btn");
        _planCount = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "TaskPlan_Count");
        _planBar = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "TaskPlan_Fill");
        _taskDescription = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "TaskDescription");
        _taskIcon = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "Task_Icon");
        _taskCount = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Task_Count");

        _btnImage = _taskBtn.GetComponent<Image>();
        _btnText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_taskBtn.gameObject, "Btn_Text");
        _popupPlayImage = UnityHelper.GetTheChildNodeComponetScripts<Image>(_taskBtn.gameObject, "PopupPlay");


        _taskBtn.GetComponent<Button>().onClick.AddListener(Click);
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

    private BtnState _btnState;
    /// <summary>
    /// 更新显示信息
    /// </summary>
    public void ChangeMessage()
    {
        if (_popupPlayImage != null)
            _popupPlayImage.gameObject.SetActive(false);
        // Net_CB_TaskArr config = _netCbTaskArr;
        EItemType itemType = EItemType.Coin;
        //任务进度条
        _planBar.fillAmount = (float)_netCbTaskArr.progress / _netCbTaskArr.condition;
        string passCount = "<color=#cf0400>{0}</color>/{1}";
        if (_netCbTaskArr.progress>=_netCbTaskArr.condition)
        {
            //任务文本
            _planCount.text = "MAX";
        }
        else
        {
            //任务文本
            _planCount.text = string.Format(passCount, _netCbTaskArr.progress, _netCbTaskArr.condition); 
        }
       
        //获取奖励数量
        _number = _netCbTaskArr.winRewards[0].num;
        //获取奖励类型
        _type = _netCbTaskArr.winRewards[0].type;
        _taskType = _netCbTaskArr.type;
        // float number = _netCbTaskArr.winNum;
      
       
        _taskCount.text = String.Format("{0}元",(_number/100f));

        int index=5;
        switch (_netCbTaskArr.type)
        {
            case 26:
                index = 3;
                break;
            case 27:
                index = 1;
                break;
            case 28:
                index = 2;
                break;
            case 29 :
                index = 4;
                break;
            
        }

        if (index==5)
        {
            _taskDescription.SetActive(false);
        }
        else
        {
            //任务描述
            _taskDescription.text = String.Format(_taskDescriptionStr[index], _netCbTaskArr.condition);
        }
       
        itemType = (EItemType)_type;
        SRewardData data = new SRewardData(itemType);
        // _taskIcon.sprite = data._rewardSprite; //更新 图片显示
        switch (_netCbTaskArr.status)
        {
            case 1:
                _btnState = BtnState.HasGet;
                _btnImage.sprite = iconBtn[0];
                _btnText.text = "已领取";
                // transform.SetAsLastSibling();
                break;
            case 2:
                _btnState = BtnState.Finished;
                _btnImage.sprite = iconBtn[1];
                _btnText.text = "领奖励";
                //if ((TaskType)_netCbTaskArr.type != TaskType.NewPlayer && itemType == EItemType.CashCoupon && _number >= GL_ConstData.TaskRewardAdNumber)
                //{
                //    if (_popupPlayImage != null)
                //        _popupPlayImage.gameObject.SetActive(true);
                //    LayoutRebuilder.ForceRebuildLayoutImmediate(_btnText.transform.parent.transform as RectTransform);
                //}
                // transform.SetAsFirstSibling();
                break;
            case 3:
                _btnState = BtnState.UnFinish;
                _btnImage.sprite = iconBtn[2];
                _btnText.text = "去完成";
                break;
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
                UI_HintMessage._.ShowMessage(/*transform,*/ DataModuleManager._instance.TableDailyTaskData_Dictionary[20].Description);
                // _uiIfTask.ClosePage();
                break;
            case BtnState.UnFinish: //去完成
                
                // GL_Game._instance._taskConfig.ExeTask((TaskType) _netCbTaskArr.type);
                 break;
            case BtnState.Finished: //领取

                GL_SDK._instance.Vibrate(300,1);
                // if ((TaskType)_netCbTaskArr.type != TaskType.NewPlayer
                //     && _netCbTaskArr.winRewardType == 7 && _number >= GL_ConstData.TaskRewardAdNumber
                //     && !GL_GuideManager._instance._isGuideing)
                // {
                //     GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_GetLevelReward, go =>
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
                Action action = () => { RefreshReward(); };
                //Action actionGet = () => { };
                Net_CB_VideoRed _videoRedpackConfig = new Net_CB_VideoRed();
                for (int i = 0; i < _netCbTaskArr.winRewards.Count; i++)
                {
                    switch (_netCbTaskArr.winRewards[i].type)
                    {
                     case 1://金币
                         _videoRedpackConfig.mostCoupon = _netCbTaskArr.winRewards[i].num;
                         break;
                     case 3://假现金
                         _videoRedpackConfig.mostBougs = _netCbTaskArr.winRewards[i].num;
                         break;
                    }
                }
                _videoRedpackConfig.bougs = 0;
                object[] objects = { ERewardSource.TaskReward, _videoRedpackConfig.mostCoupon, _videoRedpackConfig.mostBougs, _videoRedpackConfig.bougs, action };   
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetNormal, objects);
              
                break;
        }
    }

    private void RefreshReward()
    {
        _btnImage.sprite = iconBtn[0];
        _btnText.text = "已领取";
        _netCbTaskArr.status = 1;
        _btnState = BtnState.HasGet;
        transform.SetSiblingIndex( transform.parent.childCount-1);
        switch (_taskType)
        {
            case 26:
                GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.TaskPassFinish);
                break;
            case 27:
                GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.VideoCoin);
                break;
            case 28:
                GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.TaskVideo);
                break;
            case 29 :
                GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.TaskDaily);
                break;
        }
        //回报
         GL_Game._instance._taskConfig.GetTaskRewardReport(0,(int)_number, taskId,  _taskType,_type,true);

        
    }
    
    //是否是下载任务
    private bool IsDownloadTask()
    {
        if (_netCbTaskArr == null)
            return false;

        if ((TaskType)_netCbTaskArr.type == TaskType.DownLoadIce
            || (TaskType)_netCbTaskArr.type == TaskType.DownLoadIdom
            ||(TaskType)_netCbTaskArr.type == TaskType.DownLoadLifeWin)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
}




