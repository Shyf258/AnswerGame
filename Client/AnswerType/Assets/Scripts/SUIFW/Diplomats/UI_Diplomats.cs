using System;
using System.Collections.Generic;
using Logic.System.NetWork;
using UnityEngine;
using SUIFW;
using UnityEngine.UI;
using Object = System.Object;

public class UI_Diplomats : Singleton<UI_Diplomats>
{

    public void Init()
    {
        UIManager.GetInstance().Init();
        UIMaskMgr.GetInstance().Init();

        GL_GameEvent._instance.RegisterEvent(EEventID.GameState, DoGameState);
    }

    ~UI_Diplomats()
    {
        GL_GameEvent._instance.UnregisterEvent(EEventID.GameState, DoGameState);
    }

    //接收游戏状态
    private void DoGameState(EventParam param)
    {
        var p = (EventParam<EGameState>)param;
        //DDebug.LogError("UI_GameState~~~~~~~~~~~~" + p._param.ToString());
        switch (p._param)
        {
            case EGameState.None:
            case EGameState.GameMain:
                CloseAllUI();
                ShowUI(SysDefine.UI_Path_Main);
                //if(!GL_CoreData._instance.AbTest)
                    ShowUI(SysDefine.UI_Path_DragRedpack);


                if (GL_Game._instance._isStartGame)
                {
                    //刚进入游戏, 关卡列表直接到位
                    //优先级:  1. 新手礼包 2.签到 3.主界面上的UI奖励(通行证赛季结算等)
                    MainPageEventSort1();
                    GL_Game._instance._isStartGame = false;
                }
                else
                {
                    MainPageEventSort2();
                }
                break;
            //case EGameState.Playing:
            //    ShowUI(SysDefine.UI_Path_Game);
            //    break;
            case EGameState.Loading:
                ShowUI(SysDefine.UI_Path_Loading);
                break;
            case EGameState.Splash:
                ShowUI(SysDefine.UI_Path_Splash);
                break;
            //case EGameState.Settlement:
            //    CloseAllUI();
            //    //是否第一次通关
            //    //if (!GL_SceneManager._instance.HasRewardInCurLevel())
            //    //{
            //    //    ShowUI(SysDefine.UI_PATH_SETTLENOREWARD);
            //    //}
            //    //else
            //    //{
            //    //    ShowUI(SysDefine.UI_PATH_SETTLEMENT_CHAPTER);
            //    //    //BeginSettlement();


            //    //}
            //    break; 
            case EGameState.PureVersion:
                // ShowUI(SysDefine.UI_Path_MainPage);
                break;         
            default:
                break;
            
        }
    }

    public void MainPage()
    {
        //if (GL_Game._instance._isStartGame)
        //{
        //    //刚进入游戏, 关卡列表直接到位
        //    //优先级:  1. 新手礼包 2.签到 3.主界面上的UI奖励(通行证赛季结算等)
        //    MainPageEventSort1();
        //}
        //else
        {
            //返回主界面, 
            //优先级: 1.评价 2.活动目标收集 3.活动奖励发放 4. 通行证,四叶草动画 5. 主界面上的UI奖励(通行证赛季结算等)
            // 6. 路点开启 7.关卡宝箱开启 8.玩家气泡 9.引导(队伍引导 第二关引导)

        }
    }


    /// <summary>
    /// 创建UI
    /// </summary>
    /// <param name="path">Path.</param>
    public void ShowUI(string path, object initdata = null)
    {
       
        UIManager.GetInstance().ShowUIForms(path, initdata);
        // DDebug.LogError("~~~显示界面: " + path);
    }

    /// <summary>
    /// 关闭UI
    /// </summary>
    /// <param name="path">Path.</param>
    public void CloseUI(string path)
    {
        UIManager.GetInstance().CloseUIForms(path);
    }

    public void RefreshUI(string path)
    {
    }

    public SUIFW.UIManager GetUIManager()
    {
        return SUIFW.UIManager.GetInstance();
    }

    public void CloseAllNormalUI()
    {
        UIManager.GetInstance().CloseAllUI_NormalForms();
    }

    public void CloseAllUI()
    {
        UIManager.GetInstance().CloseAllUI();
    }

    public void CloseAllUIContainFix()
    {
        UIManager.GetInstance().CloseAllUIContainFix();
    }

    //展示Loading
    //public void ShowLoadingAD()
    //{
    //    ShowUI(SysDefine.UI_PATH_LOADINGAD);
    //}

    ////关闭Loading
    //public void CloseLoadingAD()
    //{
    //    CloseUI(SysDefine.UI_PATH_LOADINGAD);
    //}


    public void RefreshGame()
    {
        // UI_IF_Game game = UIManager.GetInstance().GetUI(SysDefine.UI_Path_Game) as UI_IF_Game;
        // if (game != null && game.IsShow())
        // {
        //     game.Refresh(true);
        // }
    }

    private bool _showPig = true;
    public bool _isMainPageOver = true;
    //刚启动游戏,  主界面事件排序
    public void MainPageEventSort1()
    {
        //if (GL_CoreData._instance._isFirstGame)
        //{
        //    //ShowUI(SysDefine.UI_Path_GuideTip);
        //    GL_CoreData._instance._isFirstGame = false;
        //    return;
        //}
            
        //return;
        _isMainPageOver = true;
        Action onHide = () => { MainPageEventSort1(); };

        // //1.引导
        // if (GL_GuideManager._instance.CheckFirstGuide())
        // {
        //     if(GL_GuideManager._instance.TriggerGuide(EGuideTriggerType.Server, onHide))
        //     {
        //         DDebug.Log("MainPageSort " + "新手引导1");
        //         return;
        //     }
        // }
        //
        // if (GL_NewbieSign._instance.CheckSecondGuide())
        // {
        //     if(GL_GuideManager._instance.TriggerGuide(EGuideTriggerType.NewSign, onHide))
        //     {
        //         DDebug.Log("MainPageSort " + "新手引导1");
        //         return;
        //     }
        // }

        //主页引导
        if (GL_GuideManager._instance.TriggerGuide(EGuideTriggerType.UIMain, onHide))
        {
            DDebug.Log("MainPageSort " + "新手引导-主页");
            return;
        }

        if (GL_CoreData._instance._isFirstGame)
        {
            GL_CoreData._instance._isFirstGame = false;
            
            return; 
            
        }

        if(_showPig && !GL_CoreData._instance.AbTest)
        {
#if PureVersion
            return;
#endif
            _showPig = false;
            // ShowUI(SysDefine.UI_Path_NewSignInPage, onHide);
            // YS_NetLogic._instance.GoldenpigConfig((config =>
            // {
            //     Object[] objects = { config,onHide };
            //     ShowUI(SysDefine.UI_IF_Goldenpig,objects);
            // } ));
            Object[] objects = { onHide };
            ShowUI(SysDefine.UI_IF_MoneyPool,objects);
            return;
        }
        

        _isMainPageOver = false;

        // if (GL_PlayerData._instance.IsLoginWeChat())
        // {
        //     GL_PlayerData._instance.GetProduceConfig((() =>
        //     {
        //         ShowUI(SysDefine.UI_Path_Production);
        //     }));
        // }

        //if (!GL_CoreData._instance.AbTest)
        {
            if (GL_Game._instance._signInConfig.Clockin()  )
            {
                ShowUI(SysDefine.UI_Path_NewSignInPage);
            }
        }
    }

    private void MainPageEventSort2()
    {
        //return;
        _isMainPageOver = true;
        Action onHide = () => { MainPageEventSort2(); };


        //主页引导
        if (GL_GuideManager._instance.TriggerGuide(EGuideTriggerType.UIMain, onHide))
        {
            DDebug.Log("MainPageSort " + "新手引导-进入关卡引导");
            return;
        }
        _isMainPageOver = false;
    }
    public void CloseGuide()
    {
        var top = UIManager.GetInstance().GetUI(SysDefine.UI_Path_Guide) as UI_IF_Guide;
        if (top != null && top.IsShow())
        {
            CloseUI(SysDefine.UI_Path_Guide);
        }
    }

    //loading界面的 loading条显隐
    public void LoadingShowSlider(bool isSlider, bool isLogin, string loginStr)
    {
        var uiLoading = UIManager.GetInstance().GetUI(SysDefine.UI_Path_Loading) as UI_IF_Loading;
        if(uiLoading != null)
            uiLoading.ShowSlider(isSlider, isLogin, loginStr);
    }
}