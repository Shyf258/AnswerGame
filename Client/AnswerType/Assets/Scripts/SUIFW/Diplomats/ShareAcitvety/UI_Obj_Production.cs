using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

public class UI_Obj_Production : UIObjectBase
{

    /// <summary>
    /// 总目标金额显示
    /// </summary>
    private Text _totlaNumber;

    /// <summary>
    /// 存着金额
    /// </summary>
    private Text _deposit;

    /// <summary>
    /// 收入描述
    /// </summary>
    private Text _description;
    
    private string descriptionStr = "本小时预计收入 = {0}元";


    private string _barrageStr = "恭喜玩家<color=#ff0000>{0}</color>在上轮分红中获得<color=#ff0000>{1}</color>元现金";
    [HideInInspector]
    public int _index = 0;

    private Text _barrageText;
    
    private Image _slider;

    private Text _sliderText;
    
    private float _timer;

    private float _timerClick = 5;
    private float _freshTime;


    private Button _tipsBtn;

    private Text _tipsText;

    private string _tipsString = "恭喜您成功邀请{0}位小伙伴本轮收入增幅{1}%";

    private Transform _descriptionPage;
    
    // private Animation _animation;

    private Text _timeText;

  
    private Transform _barrageMove;
    
    private Timer _time;
    // public Text _timerText;
    public override void InitObjectNode()
    {

        // _animation = UnityHelper.GetTheChildNodeComponetScripts<Animation>(gameObject, "BarrageFrame");
        // AnimationClip barrageMove = _animation.GetClip("barrage_Move");
        // AnimationEvent @event = new AnimationEvent();
        // @event.functionName = "ChangeText";
        // barrageMove.AddEvent(@event);

        _barrageMove = UnityHelper.FindTheChildNode(gameObject, "BarrageFrame");
        _totlaNumber = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "TotlaNumber");
        
        _deposit = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Deposit");

        _description = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Description");

        _slider = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "Slider");

        _sliderText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_slider.gameObject, "SliderText");
        
        _barrageText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Text_1");

        // _timerText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Time_text");

        _timeText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "TimeText");
        
        _tipsBtn = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "TipsBtn");
        _tipsText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_tipsBtn.gameObject, "Text");
        _time = new Timer(this,true);
        RigisterButtonObjectEvent("RecordBtn", go =>
        {
            // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
            //打开记录
            // GL_PlayerData._instance.GetProduceWithDrawHis((() =>
            // {
            //     GL_PlayerData._instance.GetProduceRanking((() =>
            //     {
            //         // UIManager.GetInstance().CloseUIForms(SysDefine.UI_Path_NetLoading);
            //         UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Salary);
            //     }));
            // }));
            GL_PlayerData._instance.GetProduceRanking((() =>
            {
                // UIManager.GetInstance().CloseUIForms(SysDefine.UI_Path_NetLoading);
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Salary);
            }));
        });

        RigisterButtonObjectEvent("ShareBtn", go =>
        {
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
            
            //分享
          GL_PlayerData._instance.GetInviteConfig((() =>
          {
              UIManager.GetInstance().CloseUIForms(SysDefine.UI_Path_NetLoading);
              UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_ShareResult);
          }));
        });
        // _animation = UnityHelper.GetTheChildNodeComponetScripts<Animation>(gameObject, "an_liangcang");
        RigisterButtonObjectEvent("GrowMoney", go =>
        {
            // _animation.Play("an_liangcang");
            //点击领取奖励
            InvokeRepeating("GrowProducePlan",0.7f,0);
        });
        
        
        RigisterButtonObjectEvent("WithDraw", go =>
        {
            WithDraw();
        });
        
        RigisterButtonObjectEvent(_tipsBtn, go =>
        {
            _tipsBtn.SetActive(false);
        });

        _descriptionPage = UnityHelper.FindTheChildNode(gameObject, "UI_IF_Description");
        RigisterButtonObjectEvent("BtnClose", go =>
        {
            _descriptionPage.SetActive(false);
        });
        RigisterButtonObjectEvent("DescriptionBtn", go =>
        {
            _descriptionPage.SetActive(true);
        });
    }

    private int time;
    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        _freshTime += Time.deltaTime;
        _timerClick += Time.deltaTime;
        Showtime();
        
        // if (_timer>=8f)
        // {
        //     _timer = 0;
        //
        //     // _animation.Play("an_liangcang");
        // }

        // if (_freshTime>3000f)
        // {
        //     
        //     GL_PlayerData._instance.GetProduceConfig(() =>
        //     {
        //         FreshText();
        //     });
        //     GL_PlayerData._instance.GetProduceBarrage((() =>
        //     {
        //         _index = 0;
        //         ChangeBarrage();
        //     }));
        // }
    }

    public override void Refresh()
    {
        base.Refresh();
        GL_PlayerData._instance.GetProduceBarrage((() =>
        {
            _timer = 0;
            _index = 0;
            Movebarrage();
        }));
        FreshText();
        _timer = 0;
        _freshTime = 0;

        if ( GL_PlayerData._instance.NetCbProduceConfig.inviteNum>=1)
        {
            _tipsBtn.SetActive(true);
            int multiple = GL_PlayerData._instance.NetCbProduceConfig.inviteNum * 50/*
             >= 100
                ? 100
                : GL_PlayerData._instance.NetCbProduceConfig.inviteNum * 50*/
                ;
            _tipsText.text = String.Format(_tipsString,GL_PlayerData._instance.NetCbProduceConfig.inviteNum,multiple);
        }
    }


    /// <summary>
    /// 弹幕移动
    /// </summary>
    private void Movebarrage()
    {
        _barrageText.transform.localPosition = new Vector3(1000,0,0);
        ChangeBarrage();
        _barrageText.transform.DOLocalMove(new Vector3(-1500, 0, 0), 8, true).SetAs(TweenParams.Params.OnComplete(() =>
        {
            Movebarrage();
        }));
    }

    private bool _click = false;
    private void Showtime()
    {
        time = 5 - (int) _timerClick >= 0 ? 5 - (int) _timerClick : 0;
        if (time<=0)
        {
            _timeText.gameObject.SetActive(false);
            _click = true;
        }
        else
        {
            _timeText.text = "倒计时：" + time;
            _timeText.gameObject.SetActive(true);
            _click = false;
        }
                                  
    }

    public void ExitPage()
    {
        _descriptionPage.SetActive(false);
    }

    /// <summary>
    /// 刷新显示信息
    /// </summary>
    public void FreshText()
    {
        _totlaNumber.text = GL_PlayerData._instance.NetCbProduceConfig.totalNum / 100f + "元";

        _deposit.text = GL_PlayerData._instance.NetCbProduceConfig.money / 100f + "元";
        
        _description.text  = String.Format(descriptionStr,GL_PlayerData._instance.NetCbProduceConfig.expectMoney/100f);

        _slider.fillAmount = GL_PlayerData._instance.NetCbProduceConfig.inviteNum / 4f;

        _sliderText.text ="增幅"+ (_slider.fillAmount * 100).ToString("0") + "%";
        // FreshTime();
    }

    


    
    public void ChangeBarrage()
    {
        _index++;

        try
        {
            if (GL_PlayerData._instance._Barrage.withDraws.Count<1)
            {
                _barrageText.text = "排行榜暂未录入数据，整点后生成排行";
                return;
            }
        }
        catch
        {
            _barrageText.text = "排行榜暂未录入数据，整点后生成排行";
            return;
        }
        
        if (_index==GL_PlayerData._instance._Barrage.withDraws.Count)
        {
            _index = 0;
        }

        string name = GL_PlayerData._instance._Barrage.withDraws[_index].nickName == null  
            ?  "游客"  
            : GL_PlayerData._instance._Barrage.withDraws[_index].nickName;
        
        _barrageText.text = String.Format(_barrageStr,
            name,
            GL_PlayerData._instance._Barrage.withDraws[_index].withDrawNum / 100f);
    }


    /// <summary>
    /// 增加生产进度，点击粮仓
    /// </summary>
    private void GrowProducePlan()
    {

        if (!_click)
        {
            UI_HintMessage._.ShowMessage("冷却中，请稍等片刻");
            return;
        }

        _timerClick = 0;
        if (GL_PlayerData._instance.NetCbProduceConfig.viewLimit>0)
        {
            StartCoroutine(FreshGet());
        }
        else
        {
            string time = DateTime.Now.Hour + 1 >= 24 ? "明日" :( (DateTime.Now.Hour + 1).ToString()+"点后");
            UI_HintMessage._.ShowMessage($"请{time}再来");
        }
       
        CancelInvoke();
    }
    

    IEnumerator FreshGet()
    {
        yield return new WaitForSeconds(0.5f);
        GetMoney();
    }
    
    private void GetMoney()
    {
        //GL_SDK._instance.PopUp();
        GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_ProductionGrow, (set) =>
        {
            if (set)
            {
                UI_HintMessage._.ShowMessage("恭喜您的总收入增加了，\n观看越多收入越多哦，加油！");
                GL_PlayerData._instance.GetProduceConfig(() =>
                {
                    FreshText();
                });
            }
        },GL_ConstData.SceneID_Production);
        
        CancelInvoke();
    }


    /// <summary>
    /// 点击提现
    /// </summary>
    private void WithDraw()
    {
        if (GL_PlayerData._instance.NetCbProduceConfig.money<30)
        {
            UI_HintMessage._.ShowMessage("满足0.3元才可以提现哦，请继续努力吧");
            return;
        }
        
        GL_PlayerData._instance.ProduceWithdraw(() =>
        {
            GL_PlayerData._instance.GetProduceConfig(() =>
            {
                FreshText();
            });
        });
      
    }

}
