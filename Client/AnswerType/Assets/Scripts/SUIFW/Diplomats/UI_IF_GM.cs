using System;
using System.Collections.Generic;
using UnityEngine;
using SUIFW;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;

public class UI_IF_GM : BaseUIForm
{
    //test
    public Button btnPassAll;
    public Button _btnTest;
    public Button _btnCancel;
    public GameObject _testOpenRoot;


    public override void Init()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.Fixed;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Light;

        #region 控制界面
        _btnTest.onClick.AddListener(() =>
        {
            Refresh(false);
            _testOpenRoot.gameObject.SetActive(true);
            _btnTest.gameObject.SetActive(false);
        });
        _btnCancel.onClick.AddListener(() =>
        {
            _testOpenRoot.gameObject.SetActive(false);
            _btnTest.gameObject.SetActive(true);
        });
        #endregion

        #region 进入关卡
        InputField levelInput = UnityHelper.GetTheChildNodeComponetScripts<InputField>(gameObject, "inputIndexList");

        RigisterButtonObjectEvent("btnSetIndexList", (go) =>
        {
            int level = 1;
            int.TryParse(levelInput.text, out level);
            GL_Game._instance.DoLevelEnter(level);
        });
        #endregion



#if Test123
int i = 100;

#endif

    }


    void SetCanvas()
    {
        var canvas = this.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1;
        }
    }


    public override void onUpdate()
    {

    }


    public override void Refresh(bool recall)
    {
        //_textLevelDataIndex.text = MC_FruitManager._instance.LevelDataIndex.ToString();
    }


    public override void RefreshUIColor()
    {
    }

    public override void RefreshLanguage()
    {
    }

    public override void OnHide()
    {
    }
}