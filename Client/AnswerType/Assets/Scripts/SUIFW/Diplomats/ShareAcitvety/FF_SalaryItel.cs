using System;
using System.Collections;
using System.Collections.Generic;
using SUIFW;
using UnityEngine;
using UnityEngine.UI;

public class FF_SalaryItel : UI_BaseItem
{
    private Text _descriptionText;

    private string _textSTR = "本小时预支工资<color=#fe3000>{0}元</color>，开支时间:\n{1}";

    private string timeStr = "{0}年{1}月{2}日，{3}：{4}";
    
    private string _rankSTR = "用户名：{0},今日贡献值<color=#fe3000>{1}元</color>，暂列第{2}名。"/* +
                              "姓名：{0},本小时预支工资<color=#fe3000>{1}元</color>，开支时间:\n{2},排名{3}"*/;
    
    private WithDrawHis _withDrawData;

    private WithDrawRank _withDrawRank;

    private RecordType _record;

    /// <summary>
    /// 排行图标
    /// </summary>
    private Image _rankIcon;

    /// <summary>
    /// 排名
    /// </summary>
    private Text _rankText;

    public List<Sprite> _iconList;
    private int index;

    private Text _addition;
    private string additionStr = "+{0}%奖金";
    public override void Init()
    {
        if (_isInit)
            return;

        _isInit = true;
        
        base.Init();

        _descriptionText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Text");
        _rankIcon = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "RankIcon");
        _rankText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "rankText");
        _addition = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Addition");
    }
    
    public override void Refresh<T>(T data, int dataIndex)
    {
        base.Refresh(data, dataIndex);
        _withDrawData = null;
        if (data is WithDrawHis netData)
        {
            _withDrawData = netData;
            _record = RecordType.Payslip;
            // _childIndex = dataIndex;
            // InitData();
        }
        else if (data is WithDrawRank rankData)
        {
            _withDrawRank = rankData;
            _record = RecordType.Rank;
        }

        InitData();
    }

    private void InitData()
    {
        Init();
        if (_record == RecordType.Payslip)
        {
         // DateTime time = GL_Tools.GetTime(_withDrawData.withDrawTime,true);
         //
         // string recordTime = string.Format(timeStr, time.Year, time.Month, time.Day, time.Hour, time.Minute.ToString("00"));
         //
         // _text.text = string.Format(_textSTR,
         //     // _withDrawData.nickName,
         //     _withDrawData.withDrawNum / 100f,
         //     // WithDrawSuccess(_withDrawData.status),
         //     recordTime);
            
        }
        else
        {
            // DateTime time = GL_Tools.GetTime( _withDrawRank.withDrawTime,true);

            // string recordTime = string.Format(timeStr, time.Year, time.Month, time.Day, time.Hour, time.Minute.ToString("00"));
            index = GL_PlayerData._instance.MinIndex(GL_PlayerData._instance.NetCbProduceRanking.withDraws,
                _withDrawRank);
            string name = _withDrawRank.nickName == null  ?  "游客"  :  _withDrawRank.nickName;
            _descriptionText.text = string.Format(_rankSTR, 
                name,
                _withDrawRank.withDrawNum / 100f,
                // WithDrawSuccess(_withDrawRank.status),
                // recordTime,
                
                // transform.GetSiblingIndex()+1
                index
                );

            FreshRank();

            double add = _withDrawRank.rate;
            _addition.text = string.Format(additionStr, add.ToString("0.0"));
        }
    }

    private void FreshRank()
    {
        _rankText.SetActive(false);
        switch (index)
        {
            case 1 :
                _rankIcon.sprite = _iconList[0];
                _rankText.SetActive(false);
                break;
            case 2 :
                _rankIcon.sprite = _iconList[1];
                break;
            case 3 :
                _rankIcon.sprite = _iconList[2];
                break;
            default:
                _rankText.SetActive(true);
                _rankIcon.sprite = _iconList[3];
                _rankText.text = index.ToString();
                break;
        }
        _rankIcon.SetNativeSize();
    }

    // private void ShowMessage(string name,string status, float money ,  string time)
    // {
    //     _text.text = string.Format(_textSTR, name, money, status,time);
    //     // _text.text = string.Format(_rankSTR, name, money, status,time,);
    // }

    private string WithDrawSuccess(int status)
    {
        string statusStr;
        if (status == 2)
        {
            //已到账
            statusStr = "已到账";
        }
        else
        {
            //审核中
            statusStr = "已到账";
        }

        return statusStr;
    }

}

enum RecordType
{
    Rank,
    Payslip
}
