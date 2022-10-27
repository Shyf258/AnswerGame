using SUIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataModule;
using System.Linq;
using UnityEngine.UI;

public class UI_IF_Guide : BaseUIForm
{
    private Transform _mask;

    private TableGuideData _tableData;
    private UI_GuideObject _chickObject;  //点击对象

    private Transform _fingerTrans;  //手指图标

    private Transform _dialogTrans;
    private Text _dialogText;
    public override void Init()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.Normal;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate;

        _mask = UnityHelper.FindTheChildNode(gameObject, "mask");
        _fingerTrans = UnityHelper.FindTheChildNode(gameObject, "Finger");

        _dialogTrans = UnityHelper.FindTheChildNode(gameObject, "Dialog");
        _dialogText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_dialogTrans.gameObject, "Text");

        RigisterButtonObjectEvent("mask", (go) => OnMask());
    }

    public override void InitData(object data)
    {
        _tableData = data as TableGuideData;

    }

    public override void onUpdate()
    {

    }

    public override void Refresh(bool recall)
    {
        _mask.SetActive(_tableData.IsShowMask);
        _chickObject = null;
        switch ((EGuideType)_tableData.GuideType)
        {
            case EGuideType.ClickObject:
                ShowClickObject();
                break;
            case EGuideType.Dialog:
                break;
            case EGuideType.Operate:
                break;
            default:
                break;
        }

        //显示对话框内容
        if(_tableData.IsShowDialog)
        {
            _dialogTrans.SetActive(true);
            _dialogText.text = _tableData.Dialog;
            float positionY = 0;
            //计算对话框位置
            if(_chickObject != null)
            {
                positionY = _chickObject.transform.localPosition.y + _tableData.DialogOffset;
            }

            _dialogTrans.transform.localPosition = Vector3.up * positionY;
        }
        else
        {
            _dialogTrans.SetActive(false);
        }
        
    }

    private void ShowClickObject()
    {
        var guideObjs = FindObjectsOfType<UI_GuideObject>();
        _chickObject = guideObjs.FirstOrDefault(t => t.name == _tableData.ObjectName);

        if (_chickObject != null)
        {

            //这里计算手指偏移
            _fingerTrans.gameObject.SetActive(_tableData.IsShowFinger);
            //_clickImg.gameObject.SetActive(!_noFinger);
            if (_tableData.IsShowFinger)
            {
                _fingerTrans.position = _chickObject.GetUiPos();
                // DDebug.LogError("*****按键坐标："+_chickObject.GetUiPos());
                // DDebug.LogError("*****手指坐标："+_fingerTrans.localPosition);
                if (_tableData.FingerOffset != null && _tableData.FingerOffset.Count > 1)
                    _fingerTrans.localPosition += new Vector3(_tableData.FingerOffset[0], _tableData.FingerOffset[1]);
            }
            if (!_chickObject.SetGuideButton())
                GL_GuideManager._instance.FinishGuide();
        }
        else
        {
            GL_GuideManager._instance.FinishGuide();
        }
    }

    private void OnMask()
    {
        if(_tableData.IsClickOtherClose)
        {
            GL_GuideManager._instance.FinishGuide();
            if (_tableData.IsClickOtherClose)
            {
                GL_GuideManager._instance._guide.OnBtnGuideListen();
            }
        }
    }

    public override void RefreshLanguage()
    {
    }
}
