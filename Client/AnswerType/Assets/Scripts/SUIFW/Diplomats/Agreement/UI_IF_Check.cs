using System;
using System.Collections;
using System.Collections.Generic;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class UI_IF_Check : BaseUIForm
{
    #region 对象

    //验证
    private Transform _checkFrame;
    private InputField _inputFieldName;
    private InputField _inputFieldId;
    private Button _checkIn;
    
    //失败
    private Transform _failFrame;
    private Button _btnCheck;
    private Button _btnOut;
    
    
    //提示
    private Transform _tipsFrame;
    
    
    //完成
    private Transform _finishFrame;
    private Button _btnCloseFinish;
    
    //未成年
    private Transform _nonageFrame;

    private Button _btnCloseNonage;
    
    #endregion

    private Transform _page;
    
    private CheckMessage _checkMessage;
    
    public override void Init()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.Normal;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;


        //验证
        _checkFrame = UnityHelper.FindTheChildNode(gameObject, "Check_Frame");
        _inputFieldName = UnityHelper.GetTheChildNodeComponetScripts<InputField>(_checkFrame.gameObject, "Input_Name");
        _inputFieldId = UnityHelper.GetTheChildNodeComponetScripts<InputField>(_checkFrame.gameObject, "Input_ID");
        _checkIn = UnityHelper.GetTheChildNodeComponetScripts<Button>(_checkFrame.gameObject, "CheckIn");
        RigisterButtonObjectEvent(_checkIn, gp =>
        {
            CheckPlayer();
        });
        
        //失败
        _failFrame = UnityHelper.FindTheChildNode(gameObject, "Fail_Frame");
        _btnCheck = UnityHelper.GetTheChildNodeComponetScripts<Button>(_failFrame.gameObject, "BtnCheck");
        RigisterButtonObjectEvent(_btnCheck,go=>{FreshPage(CheckState.UnCheck);});
        _btnOut = UnityHelper.GetTheChildNodeComponetScripts<Button>(_failFrame.gameObject, "BtnOut");
        RigisterButtonObjectEvent(_btnOut, go =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
        //提示
        _tipsFrame = UnityHelper.FindTheChildNode(gameObject, "Tips_Frame");
        
        
        //完成
        _finishFrame = UnityHelper.FindTheChildNode(gameObject, "Finish_Frame");
        _btnCloseFinish = UnityHelper.GetTheChildNodeComponetScripts<Button>(_finishFrame.gameObject, "BtnClose");
        RigisterButtonObjectEvent(_btnCloseFinish, go => { CloseUIForm();});
        
        //未成年
        _nonageFrame = UnityHelper.FindTheChildNode(gameObject, "Nonage_Frame");
        _btnCloseNonage = UnityHelper.GetTheChildNodeComponetScripts<Button>(_nonageFrame.gameObject, "BtnClose");
        RigisterButtonObjectEvent(_btnCloseNonage, go => { CloseUIForm();});
    }


 
    private Action _action;
    public override void InitData(object data)
    {
        base.InitData(data);
        var datas = data as Object[];
        if (datas.Length>0 && datas[0]!=null)
        {
            _checkMessage = datas[0] as CheckMessage;

            _action = _checkMessage._action;

            FreshPage(_checkMessage.State);
        }
    }

    public override void RefreshLanguage()
    {
       
    }

    public override void Refresh(bool recall)
    {
       
    }

    private void FreshPage(CheckState state)
    {
        if (_page!= null)
        {
            _page.SetActive(false);
        }

        switch (state)
        {
            case  CheckState.UnCheck:
                _page =  _checkFrame;
                break;
            case  CheckState.Fail:
                _page =  _failFrame;
                break;
            case  CheckState.Tips:
                _page =  _tipsFrame;
                break;
            case  CheckState.Finish:
                _page = _finishFrame;
                break;
            case  CheckState.Nonage:
                _page = _nonageFrame;
                GL_CoreData._instance.Nonage = true;
                GL_CoreData._instance.Anti = true;
                break;
        }

        
        _page.SetActive(true);
    }

    public override void onUpdate()
    {
       
    }

    public override void OnHide()
    {
        base.OnHide();
        _action?.Invoke();
        _action = null;
    }

    #region 验证身份信息

     private void CheckPlayer()
    {
        if (_inputFieldName.text.Length <=1)
        {
            // UI_HintMessage._.ShowMessage("请正确输入名字");
            FreshPage(CheckState.Fail);
            return;
        }

        if (_inputFieldId.text.Length!=18)
        {
            // UI_HintMessage._.ShowMessage("请输入正确身份证号码");
            FreshPage(CheckState.Fail);
            return;
        }

        // if (CheckCidInfo(_inputFieldId.text))
        // {
        //     UI_HintMessage._.ShowMessage("验证通过");
        //     CloseUIForm();
        // }
        // else
        // {
        //     UI_HintMessage._.ShowMessage("验证未通过，请正确输入信息");
        // }
        DDebug.LogError(CheckCidInfo(_inputFieldId.text));
        
    }

    // private string name = "张伟";
    // private string ID = "110120199012211472";
    private bool Check()
    {
        bool player;
        // if (_inputFieldName.text != name && _inputFieldId.text!= ID)
        {
            UI_HintMessage._.ShowMessage("请正确信息");
            return false;
        }
        player = true;
        return player;
    }

    private string CheckCidInfo(string cid)
    {
        string[] aCity = new string[]{null,null,null,null,null,null,null,null,null,null,null,"北京","天津","河北","山西","内蒙古",null,null,null,null,null,"辽宁","吉林","黑龙江",null,null,null,null,null,null,null,"上海","江苏","浙江","安微","福建","江西","山东",null,null,null,"河南","湖北","湖南","广东","广西","海南",null,null,null,"重庆","四川","贵州","云南","西藏",null,null,null,null,null,null,"陕西","甘肃","青海","宁夏","新疆",null,null,null,null,null,"台湾",null,null,null,null,null,null,null,null,null,"香港","澳门",null,null,null,null,null,null,null,null,"国外"};
        double iSum=0;
        string info="";
        System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex(@"^\d{17}(\d|x)$");
        System.Text.RegularExpressions.Match mc = rg.Match(cid);
        if(!mc.Success)
        {
            return "";
        }
        cid = cid.ToLower();
        cid = cid.Replace("x","a");
        if(aCity[int.Parse(cid.Substring(0,2))]==null)
        {
            // return "非法地区";
            // UI_HintMessage._.ShowMessage("输入请正确信息");
            FreshPage(CheckState.Fail);
            return "非法地区";
        }
        try
        {
            DateTime.Parse(cid.Substring(6,4)+"-"+cid.Substring(10,2)+"-"+cid.Substring(12,2));
        }
        catch
        {
            // UI_HintMessage._.ShowMessage("输入请正确信息");
            FreshPage(CheckState.Fail);
            return "非法生日";
        }
        for(int i=17;i>=0;i--)
        {
            iSum +=(System.Math.Pow(2,i)%11)*int.Parse(cid[17-i].ToString(),System.Globalization.NumberStyles.HexNumber);

        }

        if (iSum % 11 != 1)
        {
            // UI_HintMessage._.ShowMessage("输入请正确信息");
            FreshPage(CheckState.Fail);
            return("非法证号");
        }


        //GL_PlayerPrefs.SetInt(EPrefsKey.IsFCM, 1);
        //GL_CoreData._instance.Check = false;
        // CloseUIForm();
        int year = Convert.ToInt32(cid.Substring(6, 4));
        int month = Convert.ToInt32(cid.Substring(10,2));
        int date = Convert.ToInt32(cid.Substring(12,2));

        if (DateTime.Now.Year -year > 18 )
        {
            DDebug.LogError("成年");
            FreshPage(CheckState.Finish);
        }
        else if (DateTime.Now.Year -year == 18)
        {
            DDebug.LogError("不一定成年");
            if (DateTime.Now.Month > month )
            {
                DDebug.LogError("成年");
                FreshPage(CheckState.Finish);
            }
            else if (DateTime.Now.Month == month)
            {
                DDebug.LogError("比较日期");
                if (DateTime.Now.Day >= date)
                {
                    DDebug.LogError("成年");
                    FreshPage(CheckState.Finish);
                }
                else
                {
                    DDebug.LogError("未成年");
                    FreshPage(CheckState.Nonage);
                }
            }
            else
            {
                DDebug.LogError("未成年");
                FreshPage(CheckState.Nonage);
            }
        }
        else
        {
            DDebug.LogError("未成年");
            FreshPage(CheckState.Nonage);
        }
        
        return(aCity[int.Parse(cid.Substring(0,2))]
               +","+cid.Substring(6,4)+"-"+cid.Substring(10,2)
               +"-"+cid.Substring(12,2)+","+
               (int.Parse(cid.Substring(16,1))%2==1?"男":"女"));

    }

    #endregion
    
 
  
    
}
public class CheckMessage
{
    public CheckState State;
    public Action _action;
}
