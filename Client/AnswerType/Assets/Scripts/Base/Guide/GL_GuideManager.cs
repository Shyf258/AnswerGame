//2021.12.10    关林
//新手引导框架

using DataModule;
using SUIFW;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Logic.Fly;
using UnityEngine;

public class GL_GuideManager : Singleton<GL_GuideManager>
{
    #region 数据
    private SGuideArchiveInfo _guideArchiveInfo
    {
        set => GL_CoreData._instance._archivedData._guideArchiveInfo= value;
        get => GL_CoreData._instance._archivedData._guideArchiveInfo;
    }
    private Dictionary<int, List<TableGuideData>> _config;

    public Dictionary<int, List<TableGuideData>> Config
    {
        get
        {
            if (_config == null)
            {
                _config = new Dictionary<int, List<TableGuideData>>();

                foreach (var t in DataModuleManager._instance.TableGuideData_Dictionary.Values)
                {
                    if (_config.ContainsKey(t.Group))
                    {
                        _config[t.Group].Add(t);
                    }
                    else
                    {
                        _config.Add(t.Group, new List<TableGuideData> { t });
                    }
                }
            }

            return _config;
        }
    }
    #endregion

    public bool _isGuideing;    //是否正在引导
    private int _curGroudID;   //当前进行的 组ID
    private TableGuideData _curGuideData;   //当前进行的引导数据
    private Action _guideCallback;  //引导回调
    public UI_GuideObject _guide; //引导对象
    
    #region 初始化
    public void Init()
    {
        if (_guideArchiveInfo == null)
            _guideArchiveInfo = new SGuideArchiveInfo();
        if (_guideArchiveInfo._finishGuideDic == null)
            _guideArchiveInfo._finishGuideDic = new Dictionary<int, List<int>>();

        List<int> deleteArchive = new List<int>();  //删除存档

        foreach (var groupID in _guideArchiveInfo._finishGuideDic.Keys)
        {
            var group = Config.Get(groupID);
            if (group == null)
            {
                //1.检测存档, 删除表格中没有的group
                deleteArchive.Add(groupID);
                continue;
            }
            if (_guideArchiveInfo._finishGuideDic[groupID].Count >= group.Count)
            {
                //2.已完成全部引导,减少检测引导的计算量
                Config.Remove(groupID);
            }
            else
            {

                if (group[0].IsForce)
                {
                    //3.检测引导存档，剔除 没有全部完成的强制引导
                    //已完成的数据 少于全部数据,则未全部完成    
                    if (_guideArchiveInfo._finishGuideDic[groupID].Count < group.Count)
                        deleteArchive.Add(groupID);
                }
                else
                {
                    //非强制时, 减少已完成的部分
                    for (int i = 0; i < _guideArchiveInfo._finishGuideDic[groupID].Count; i++)
                    {
                        Config[groupID].RemoveAt(0);
                    }
                }
            }
        }

        //删除
        foreach (var id in deleteArchive)
        {
            _guideArchiveInfo._finishGuideDic.Remove(id);
        }
    }
    #endregion

    #region 引导网络通信
    
    //public Net_CB_GamecoreConfig _guideConifg;
    public void GetConfig()
    {
        GL_PlayerData._instance.SendGamecoreConfig(EGamecoreType.Guide, () =>
        {
        });
    }
    #endregion

    #region 检测
    /// <summary>
    /// 触发引导
    /// </summary>
    public bool TriggerGuide(EGuideTriggerType type, Action action = null)
    {
#if PureVersion
        return false;
#endif
        
        if (AppSetting.IsCloseGuide)
            return false;

        if (action != null)
            _guideCallback = new Action(action);
        int level = GL_PlayerData._instance.CurLevel;
        bool isTrigger = false;
        foreach (var groudID in Config.Keys)
        {
            //检测每组， 第一个
            var guide = Config[groudID][0];
            //等级
            if (level < guide.UnlockLevel)
                continue;
            //类型
            if (type != (EGuideTriggerType)guide.TriggerType)
                continue;
            //前置是否完成
            if (!IsFinishGuide(guide.PreGroup))
                continue;

            isTrigger = true;
            _curGroudID = groudID;
            _curGuideData = guide;
            break;
        }

        if (isTrigger)
        {
            DDebug.LogError("~~~成功查找到引导 : " + _curGroudID + ". table:" + _curGuideData.ID);
            DoExecute();
        }

        return isTrigger;

    }

    //检测第一个引导是否可触发
    public bool CheckFirstGuide()
    {
        //1.检测存档中, 第一个引导组是否完成
        if(IsFinishGuide(1))
            return false;
        //2.检测引导config的状态
        var conifg = GL_PlayerData._instance.GetGamecoreConfig(EGamecoreType.Guide);
        if (conifg == null)
            return false;

        if (conifg.progress >= conifg.acceptTimes)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //检测是否要获取config
    public void CheckGuideConfig()
    {
        //如果第一个已完成, 则不检测,读取本地的存档就可以了 
        if (IsFinishGuide(1))
            return;
        GetConfig();
    }
    //public bool IsReady
    //{
    //    get
    //    {
    //        return IsFinishGuide(1) || _guideConifg != null;
    //    }
    //}

    public TableGuideData CurGuideData => _curGuideData;


    public void TriggerGuide11()
    {
        if (CurGuideData == null || CurGuideData.ID != 11)
            return;

        OnCallback();
    }
    //public bool IsCheckReturnMain()
    //{
    //    bool result = false;
    //    if(CurGuideData != null && (EGuideTriggerType)CurGuideData.TriggerType == EGuideTriggerType.UIMain)
    //    {
    //        result = true;
    //    }

    //    return result;
    //}
    #endregion

    #region 引导执行
    private void DoExecute()
    {
        // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Guide_Execute, "_" + _curGuideData.ID);
        
        _isGuideing = true;

        #region 引导前事件触发
        // 执行引导前方法
        if (_curGuideData.PreFunc != null && _curGuideData.PreFunc.Count > 0)
        {
            foreach (var s in _curGuideData.PreFunc.Where(s => !string.IsNullOrEmpty(s)))
            {
                try
                {
                    var method = typeof(GL_GuideManager).GetMethod(s, BindingFlags.NonPublic | BindingFlags.Instance);
                    var obj = (object)this;
                    method?.Invoke(obj, null);
                }
                catch
                {
                }
            }
        }

        #endregion

        #region 引导类型检测
        switch ((EGuideType)_curGuideData.GuideType)
        {
            case EGuideType.ClickObject:
                ExecuteClickObject();
                break;
            case EGuideType.Dialog:
                break;
            case EGuideType.Operate:
                break;
            default:
                break;
        }
        #endregion
    }

    private void ExecuteClickObject()
    {
        if (string.IsNullOrWhiteSpace(_curGuideData.ObjectName))
        {
            //没有物体则直接完成
            FinishGuide();
            return;
        }

        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Guide, _curGuideData);
    }

    #endregion

    #region 引导完成判断
    public void CheckUIClose(string uiName)
    {
        if (_curGuideData == null)
            return;
    
        
        if ((EGuideType)_curGuideData.GuideType != EGuideType.CheckUIClose)
            return;

        if(_curGuideData.ObjectName.Equals(uiName))
        {
            FinishGuide();
        }
    }
    #endregion

    #region 完成引导



    public void FinishGuide()
    {
        // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Guide_Finish , "_" + _curGuideData.ID);
        
        _isGuideing = false;
        UI_Diplomats._instance.CloseGuide();

        #region 存档
        if(!_guideArchiveInfo._finishGuideDic.ContainsKey(_curGroudID))
            _guideArchiveInfo._finishGuideDic.Add(_curGroudID, new List<int>());
        if(!_guideArchiveInfo._finishGuideDic[_curGroudID].Contains(_curGuideData.ID))
        {
            _guideArchiveInfo._finishGuideDic[_curGroudID].Add(_curGuideData.ID);
            GL_CoreData._instance.SaveData();
        }
        

        //本地数据清理
        if (Config.ContainsKey(_curGroudID))
            Config[_curGroudID].Remove(_curGuideData);
        #endregion

        #region 找下一步引导
        if (Config == null || Config.Count == 0 || !Config.ContainsKey(_curGroudID))
            return;
        bool isNext = true;
        if (Config[_curGroudID].Count == 0)
        {
            Config.Remove(_curGroudID);
            isNext = false;
        }

        if(isNext && !Config[_curGroudID][0].ForceNext)
        {
            //下一步不强制引导
            //等待下一个判断
            isNext = false;
        }

        #region 完成阶段的事件
        if (_curGuideData.AfterFunc != null && _curGuideData.AfterFunc.Count > 0)
        {
            foreach (var s in _curGuideData.AfterFunc.Where(s => !string.IsNullOrEmpty(s)))
            {
                try
                {
                    var method = typeof(GL_GuideManager).GetMethod(s, BindingFlags.NonPublic | BindingFlags.Instance);
                    var obj = (object)this;
                    method?.Invoke(obj, null);
                }
                catch
                {
                    // ignored
                }
            }
        }
        #endregion

        if (isNext)
        {
            //_curGroudID = _curGroudID;
            _curGuideData = Config[_curGroudID][0];
            DoExecute();
        }

        #endregion
    }


    /// <summary>
    /// 是否完成引导组
    /// </summary>
    public bool IsFinishGuide(int groupID)
    {
        //当前数据中 没有该组, 则完成
        return !Config.ContainsKey(groupID);
    }
    #endregion

    #region 引导前事件
    private void OnShowGuideTip()
    {
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GuideTip);
    }

    //清除前一个引导
    private void OnRemoveLastGroud()
    {
        int targetGroud = _curGroudID - 1;
        if(targetGroud > 1 && Config.ContainsKey(targetGroud))
        {
            var data = Config[targetGroud];
            if(data.Count > 0)
            {
                if (!_guideArchiveInfo._finishGuideDic.ContainsKey(targetGroud))
                    _guideArchiveInfo._finishGuideDic.Add(targetGroud, new List<int>());
                if (!_guideArchiveInfo._finishGuideDic[targetGroud].Contains(data[0].ID))
                {
                    _guideArchiveInfo._finishGuideDic[targetGroud].Add(data[0].ID);
                    GL_CoreData._instance.SaveData();
                }

                //本地数据清理
                if (Config.ContainsKey(targetGroud))
                {
                    Config[targetGroud].Remove(data[0]);
                    if(Config[targetGroud].Count == 0)
                    {
                        Config.Remove(targetGroud);
                    }
                }
                    
            }

        }

    }
    private void OnCallback()
    {
        if (_guideCallback == null)
            return;

        //防止引导触发引导后,  引导回调被误删除
        int temp = _guideCallback.GetHashCode();
        _guideCallback?.Invoke();
        if(temp == _guideCallback.GetHashCode())
            _guideCallback = null;
    }

    //打开红包界面
    private void OnShowOpenRedPack()
    {
        FinishGuide();
        OnShowLimitNewGift();
        
        // Action<bool> ac1 = (bool set) => { CB_OnShowOpenRedPack(set); };
        // var conifg = GL_PlayerData._instance.GetGamecoreConfig(EGamecoreType.Guide);
        // object[] objects = { ac1, false, true, ERewardSource.Guide, conifg.rewards };
        // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_SurpriseRedpack, objects);
    }
    private void CB_OnShowOpenRedPack(bool set)
    {
        GL_PlayerData._instance.SendGamecoreAccept(EGamecoreType.Guide, 0,(msg)=> 
        {
            Action ac1 = () => { OnShowLimitNewGift(); };
            object[] datas = { msg.rewards, ac1 };
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetResult, datas);
        });
        //Net_RequesetCommon com = new Net_RequesetCommon();
        //GL_ServerCommunication._instance.Send(Cmd.Guide, JsonUtility.ToJson(com), (string p)=>
        //    {
        //        Logic.System.NetWork.YS_NetLogic._instance.SearchTask();
        //    }
        //    );
    }

    //打开限时新人礼
    private void OnShowLimitNewGift()
    {
        object[] datas = { _guideCallback };
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_LimitNewGift, datas);
    }


    private void OnShowNewbieSign()
    {
        GL_NewbieSign._instance.SelectState(ENewbieSignState.WaitingGet);
        object[] datas = { _guideCallback };
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NewbieSign, datas);
    }

    private void OnChangeWithdraw()
    {
        SUIFW.Diplomats.Main.MyWithdraw.UI_IF_NewWithdraw wd = UIManager.GetInstance().GetUI(SysDefine.UI_Path_NewWithdraw) 
            as SUIFW.Diplomats.Main.MyWithdraw.UI_IF_NewWithdraw;
        if (wd == null)
            return;
        wd.DoChangeScrollRect();

    }

    #endregion
}


[System.Serializable]
public class SGuideArchiveInfo
{
    public Dictionary<int, List<int>> _finishGuideDic; // 已完成的引导

    public SGuideArchiveInfo()
    {
        _finishGuideDic = new Dictionary<int, List<int>>();
    }
}
