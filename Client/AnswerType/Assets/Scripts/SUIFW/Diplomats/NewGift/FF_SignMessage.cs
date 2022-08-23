using System;
using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using SUIFW;
using UnityEngine;
using UnityEngine.UI;

public class FF_SignMessage : UI_BaseItem
{
    private Transform _plan;

    private Image _fill;

    private Text _planText;
    
    private Transform _finish;

    private Text _date;

    private Text _condition;

    // private int _nowAd;
    //
    // private int _maxAd;
    private ESignDayState _state;
    private List<string> _stringList = new List<string>()
    {
        "第{0}天",
        "看{0}次视频广告",
        "<color=#ff0000>{0}</color>/{1}",
        "登陆"
    };
    
    public override void Init()
    {
        if (_isInit)
            return;

        _isInit = true;
        base.Init();

        _plan = UnityHelper.FindTheChildNode(gameObject, "Plan");
        _fill = UnityHelper.GetTheChildNodeComponetScripts<Image>(_plan.gameObject, "Fill");
        _planText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_plan.gameObject, "PlanText");
        _finish = UnityHelper.FindTheChildNode(gameObject, "Finish");
        _date = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Date");
        _condition = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Condition");
    }

    public override void Refresh<T>(T data, int dataIndex)
    {
        base.Refresh(data, dataIndex);

        //设置值 _nowAd ，_maxAd

        if (data is SNewbieSignDayInfo netData)
        {
            if (netData._state == ESignDayState.Complete)
            {
                _state = netData._state;
            }
            FreshMessage(netData._index, netData._total);
            if (_state == ESignDayState.Complete)
            {
                FinishState(true);
            }
            else
            {
                
            }
        }
        

       
        
    }


    public void FreshMessage(int nowAd , int maxAd)
    {
        _date.text = string.Format(_stringList[0], transform.GetSiblingIndex() + 1);
        if (GL_NewbieSign._instance._curTableData.IsPlayAD)
            _condition.text = string.Format(_stringList[1], maxAd);
        else
            _condition.text = _stringList[3];

        
        //更新完成状态
        bool finish = nowAd >= maxAd ;
        FinishState(finish);
        if (!finish)
        {
            _fill.fillAmount = (float) nowAd / maxAd;
            _planText.text = string.Format(_stringList[2],nowAd,maxAd);
        }
    }

    /// <summary>
    /// 完成状态
    /// </summary>
    /// <param name="unlock"></param>
    private void FinishState(bool finish)
    {
        _plan.SetActive(!finish);
        _finish.SetActive(finish);
    }

}



