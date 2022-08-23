//2019.07.23    关林
//场景管理器

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataModule;
using UnityEngine.SocialPlatforms;
using System;
using System.Linq;

public class GL_SceneManager : Singleton<GL_SceneManager>
{
    #region 表数据

    #endregion

    public GL_LevelAudio _levelAudio;
    private GL_GameMode _curGameMode; //当前关卡
    public GL_GameMode_Answer CurGameMode => _curGameMode as GL_GameMode_Answer;

    public void Init()
    {
        ParseTable();

        _levelAudio = new GL_LevelAudio();
        _levelAudio.Init();
        //_levelAudio.PlayAudio(GL_PlayerData._instance.CurLevel);
        _curGameMode = new GL_GameMode_Answer();
        _curGameMode.Init();

        CreateGame();
    }

    #region 关卡解锁信息

    //解析表格
    private void ParseTable()
    {
    }

    #endregion

    #region 刷新关卡

    //创建关卡
    public void CreateGame()
    {
        //_curGameMode.Init();
        int levelIndex = GL_PlayerData._instance.CurLevel;
        // levelIndex *= 98;
        CreateGame(levelIndex, EGameModeType.Answer);
    }

    public void ClearLevel()
    {
        Gl_EffectManager._instance.RemoveAllEffect();

        //if (_curGameMode != null)
        //{
        //    _curGameMode.Clear();
        //    _curGameMode.gameObject.SetActive(false);
        //    //删除旧关卡
        //    GameObject.Destroy(_curGameMode.gameObject);
        //    _curGameMode = null;
        //}

    }
    
    public void CreateGame(int levelIndex, EGameModeType gameType)
    {
        ClearLevel();

        switch (gameType)
        {
            case EGameModeType.Answer:
                CreateAnswer(levelIndex);
                break;
            //case EGameModeType.HexaPuzzle:
            //    GL_PlayerData._instance.GameMode = GameMode.Normal;
            //    CreateHexaPuzzle(levelIndex);
            //    //判断是否满足提现要求
                
            //    break;
            //case  EGameModeType.ChipsMode:
            //    GL_PlayerData._instance.GameMode = GameMode.Chips;
            //    CreateHexaPuzzle(levelIndex);
            //    break;
            default:
                break;
        }
    }

    private void CreateAnswer(int levelIndex)
    {
        //创建新关卡
        //if (levelIndex > _conjLevelList.Count)
        //    levelIndex = _conjLevelList.Count;

        //var _levelInfo = _conjLevelList[levelIndex];

        CurGameMode.CreateLevel(levelIndex);
        //CurGameMode.RefreshGame(levelIndex);

        CreateGameModeSuccess();
    }


    //创建玩法关卡成功
    private void CreateGameModeSuccess()
    {
        //UI_Diplomats._instance.RefreshGame();
    }

    #endregion

    #region 接口

    public void LevelReplay()
    {
        if(CurGameMode!= null && CurGameMode.LevelState == GL_GameMode.ELevelState.Playing)
        {
            //GL_TileRegion._instance.Replay();
        }
    }
    public void LevelHint()
    {
        if (CurGameMode != null && CurGameMode.LevelState == GL_GameMode.ELevelState.Playing)
        {
            //GL_TileRegion._instance.ShowTips();
        }
    }
    public void LevelNext()
    {
        if (CurGameMode != null && CurGameMode.LevelState == GL_GameMode.ELevelState.Playing)
        {
            CurGameMode.LevelState= GL_GameMode.ELevelState.Settle2;
        }
    }
    #endregion

    #region 更新
    public void DoUpdate(float dt)
    {
        //_answerMode?.DoUpdate(dt);
    }

    
    public void DoFixedUpdate(float dt)
    {
    }


    #endregion
}

public class SLevelInfo
{
    public int _id = 0;
    public int _level = 0;
}

[Serializable]
public class SLevelArchiveInfo
{
    public int _unlockLevelIndex = 1; //已解锁的关卡序号
    public List<SLevelUnlockInfo> _levelUnlockInfos = new List<SLevelUnlockInfo>();
}

[Serializable]
public class SLevelUnlockInfo
{
    public int _levelIndex;      //关卡序号  

    public bool _isPassed;       //是否通关   

    public int _passCount;        //通关次数

    public int _continueFailedCount; // 连续失败次数

    public int _enterCount;    //进入次数

    public SLevelUnlockInfo(int levelIndex)
    {
        _levelIndex = levelIndex;
        _isPassed = false;
        _passCount = 0;
        _continueFailedCount = 0;
    }
}



