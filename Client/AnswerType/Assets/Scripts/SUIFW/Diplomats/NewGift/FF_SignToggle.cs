using System;
using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using SUIFW;
using UnityEngine;
using UnityEngine.UI;

public class FF_SignToggle : UI_BaseItem
{
    private Image _finish;

    private Toggle _toggle;

    private Text _date;

    private UI_IF_NewbieSign _uiIfNewbieSign;

    private int _index;
    
    [HideInInspector]
    public SignMessageState _messageState;
    public override void Init()
    {
        if (_isInit)
            return;

        _isInit = true;
        base.Init();
        
        _toggle = GetComponent<Toggle>();
        _finish = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "Finish");
        _date = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Date");
       
        _uiIfNewbieSign = UIManager.GetInstance().GetUI(SysDefine.UI_Path_NewbieSign) as UI_IF_NewbieSign;
        
        _toggle.onValueChanged.AddListener(go =>
        {
            if (go)
            {
                _uiIfNewbieSign.MovePage(_index);
                _uiIfNewbieSign.DayCount = _index ;
                
            }
        });
        
        
        
        
        AddToggleGroup();
        
    }

    public override void Refresh<T>(T data, int dataIndex)
    {
        base.Refresh(data, dataIndex);

        if (data is SNewbieSignDayInfo netData)
        {
            ChangeState(netData._state == ESignDayState.Complete);
            _index = transform.GetSiblingIndex();
            _date.text = $"{_index+1}天";
        }
    }

    public void ChangeState(bool finish)
    {
        _finish.SetActive(finish);
    }

    private void AddToggleGroup()
    {
        _toggle.group = transform.parent.GetComponent<ToggleGroup>();
    }

}
