//2022.8.1  关林
//新手签到

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataModule;
using SUIFW.Diplomats.Common;
using SUIFW;

public class GL_NewbieSign : Singleton<GL_NewbieSign>
{
    private SNewbieSignArchiveInfo _archiveInfo
    {
        set => GL_CoreData._instance._archivedData._newbieSignArchiveInfo = value;
        get => GL_CoreData._instance._archivedData._newbieSignArchiveInfo;
    }

    //引导数据
    public Net_CB_GamecoreConfig _gamecoreConfig;
    //表格数据
    public TableNewbieSignData _curTableData;
    //每天数据
    public List<SNewbieSignDayInfo> _newbieSignDayInfoList;
    //当前广告CD
    public double _curCooldown => _archiveInfo._adCooldown;

    public int _curDate => _archiveInfo._curDay;
    
    private ENewbieSignState _state;

    public void Init()
    {
        if (_archiveInfo == null)
            _archiveInfo = new SNewbieSignArchiveInfo();

        GL_PlayerData._instance.SendGamecoreConfig(EGamecoreType.NewbieSign, () =>
        {
            _gamecoreConfig = GL_PlayerData._instance.GetGamecoreConfig(EGamecoreType.NewbieSign);
            if (_gamecoreConfig != null)
            {
                if (_gamecoreConfig.progress >= _gamecoreConfig.acceptTimes)
                {
                    NewbieSignState = ENewbieSignState.Complete;
                }
            }
            //RefreshData();
        });

        RefreshData();
    }

    private void RefreshData()
    {
                NewbieSignState = (ENewbieSignState)_archiveInfo._state;

        //刷新每天的状态
        if (NewbieSignState == ENewbieSignState.Model1 
            || NewbieSignState == ENewbieSignState.Model7
            || NewbieSignState == ENewbieSignState.Model360)
        {
            _newbieSignDayInfoList = new List<SNewbieSignDayInfo>();
            for (int i = 0; i < _curTableData.TotalDays; i++)
            {
                SNewbieSignDayInfo info = new SNewbieSignDayInfo();
                info._total = _curTableData.FirstDay +i * _curTableData.AddNumber;
                if (_archiveInfo._everyDayInfo==null)
                {
                    _archiveInfo._everyDayInfo = new List<int>();
                }
                if (_archiveInfo._everyDayInfo.Count > i)
                {
                    info._index = _archiveInfo._everyDayInfo[i];
                }
                else
                {
                    info._index = 0;
                    _archiveInfo._everyDayInfo.Add(0);
                }

                //计算状态
                if(info._index >= info._total)
                {
                    info._state = ESignDayState.Complete;
                }
                else
                {
                    //计算天数
                    if(i > _archiveInfo._curDay)
                    {
                        //未到
                        info._state = ESignDayState.Cannot;
                    }
                    else
                    {
                        info._state = ESignDayState.Can;
                        //可签
                        
                        
                    }
                }
                _newbieSignDayInfoList.Add(info);
            }
            for (int i = 0; i < _newbieSignDayInfoList.Count; i++)
            {
                if (i <= _archiveInfo._curDay && !_curTableData.IsPlayAD)
                {
                    DoSign(i);
                }
            }
        }
    }

    #region 阶段
    private void CheckAllSign()
    {
        bool result = true;
        foreach (var nb in _newbieSignDayInfoList)
        {
            if(nb._state != ESignDayState.Complete)
            {
                result = false;
                break;
            }
        }

        if(result)
        {
            switch (_newbieSignDayInfoList.Count)
            {
                case 7:
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetReward7);
                    break;
                case 14:
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType. GetReward14);
                    break;
                case 360:
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetReward360);
                    break;
            }
            //弹出自动提现的逻辑
            GL_PlayerData._instance.SendGamecoreAccept(EGamecoreType.NewbieSign, 0, (msg) => {
                NewbieSignState = ENewbieSignState.Complete;
                //打开结算UI
                object[] datas = { msg.rewards, null,false,true};
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetResult, datas);
            });
        }
    }
    #endregion

    #region 接口
    public void NewDay()
    {
        _archiveInfo._curDay += 1;
        _archiveInfo._playadNumber = 0;
        _archiveInfo._adCooldown = 0;
        GL_CoreData._instance.SaveData();
        RefreshData();
    }
    public void SelectState(ENewbieSignState state)
    {
        NewbieSignState = state;
    }

    //检测引导
    public bool CheckSecondGuide()
    {
        bool result = false;
        
        if (_gamecoreConfig != null && _gamecoreConfig.progress < _gamecoreConfig.acceptTimes)
            result = true;

        return result;
    }
    //主页是否显示图标
    public bool IsShowIcon()
    {
        if (NewbieSignState == ENewbieSignState.Complete || NewbieSignState == ENewbieSignState.None)
            return false;

        return true;
    }

    //签到
    public void DoSign(int day)
    {

        switch (_newbieSignDayInfoList.Count)
        {
            case 7:
                GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetRewardPlayAd7);
                break;
            case 14:
                GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetRewardPlayAd14);
                break;
            case 360:
                // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetReward360);
                break;
        }
        
        
        if (day > _newbieSignDayInfoList.Count)
        {
            UI_HintMessage._.ShowMessage("超过了签到总天数");
            return;
        }
            
        if (day > _archiveInfo._curDay)
        {
            UI_HintMessage._.ShowMessage("超过了签到天数");
            return;
        }
            

        var data = _newbieSignDayInfoList[day];
        data._index += 1;
        if(data._index>= data._total)
        {
            data._index = data._total;
            data._state = ESignDayState.Complete;
            //完成了一个,则检测所有签到是否完成
            CheckAllSign();
        }

        _archiveInfo._playadNumber += 1;
        _archiveInfo._everyDayInfo[day] = data._index;

        //计算CD
        if (_curTableData.IsPlayAD)
        {
            int value = _archiveInfo._playadNumber * 10;
            DateTime dt = DateTime.Now.AddSeconds(value);
            _archiveInfo._adCooldown = GL_Time._instance.CalculateSeconds(dt);
        }
        GL_CoreData._instance.SaveData();

        
    }
    #endregion

    public ENewbieSignState NewbieSignState
    {
        get 
        {
            return _state;
        }
        private set
        {
            if (value != _state)
            {
                _state = value;
                _archiveInfo._state = (int)value;
                switch (value)
                {
                    case ENewbieSignState.None:
                        break;
                    case ENewbieSignState.WaitingGet:
                        break;
                    case ENewbieSignState.Model1:
                    case ENewbieSignState.Model7:
                    case ENewbieSignState.Model360:
                        //选择了对应模式
                        _curTableData = GameDataTable.GetTableNewbieSignData((int)value);
                        RefreshData();
                        break;
                    case ENewbieSignState.Complete:
                        UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_NewbieSign);
                        break;
                    default:
                        break;
                }

               
                GL_CoreData._instance.SaveData();
                GL_GameEvent._instance.SendEvent(EEventID.RefreshNewbieSignUI, new EventParam<ENewbieSignState>(value));
            }
        }
    }
}

//每天的签到状态
public enum ESignDayState
{
    Cannot, //不可签到
    Can,    //可签到
    Complete,   //可完成
}

public enum ENewbieSignState
{
    None = 0,   //未激活
    Model1,     //模式1
    Model7,     //模式2
    Model360,   //模式3
    Complete,   //完成
    WaitingGet, //等待领取
    WaitSelect, //等待选择
}

//签到每天信息
public class SNewbieSignDayInfo
{
    public ESignDayState _state;
    public int _index;  //当前签到次数
    public int _total;  //当天总次数
}

[Serializable]
public class SNewbieSignArchiveInfo
{
    public int _state;   //当前阶段
    public int _curDay;         //当前第几天
    public List<int> _everyDayInfo ; //每天的签到数据

    public int _playadNumber;   //当前播放广告次数
    public double _adCooldown;  //广告冷却
}
