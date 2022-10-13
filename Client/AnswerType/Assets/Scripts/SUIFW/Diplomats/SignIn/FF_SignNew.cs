using System.Collections;
using System.Collections.Generic;
using Logic.System.NetWork;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

public class FF_SignNew : UI_BaseItem
{

    #region 对象

    /// <summary>
    /// 提示文本
    /// </summary>
    private List<string> _tipsText = new List<string>()
    {
        "恭喜领取成功",
        "请您按照顺序领取",
        "您还没有满足条件",
        "已领取"
    };

    /// <summary>
    /// 按键背景图 绿色未完成 黄色已完成未领取 灰色已领取
    /// </summary>
    public List<Sprite> _iconBG;

    private string _dateText = "连续{0}天";

    /// <summary>
    /// 签到天数显示
    /// </summary>
    private Text _date;

    /// <summary>
    /// 签到按键背景框（领取状态）
    /// </summary>
    private Image _signBg;

    // /// <summary>
    // /// 当前签到进度
    // /// </summary>
    // private Text _signPlan;

    /// <summary>
    /// 奖励数量显示
    /// </summary>
    private Text _rewardText;

    private string _rewardDescription  ;
    // /// <summary>
    // /// 奖励物品图标显示
    // /// </summary>
    // private Image _rewardIcon;

    /// <summary>
    /// 签到按键
    /// </summary>
    private Button _signBtn;


    /// <summary>
    /// 签到信息配置
    /// </summary>
    private SignConfig _signConfig;
    
    /// <summary>
    /// 已领取标签
    /// </summary>
    private Transform _sginMask;


    // private Image _bar;
    
    
    #endregion

    public override void Init()
    {
        base.Init();
        _date = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "DayCount");
        _signBg = transform.GetComponent<Image>();
        _signBtn = transform.GetComponent<Button>();
        _signBtn.onClick.AddListener(Onclick);
        // _signPlan = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "SignPlan");
        // _rewardIcon = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "Icon");
        _rewardText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "RewardCount");
        _sginMask = UnityHelper.FindTheChildNode(gameObject, "SignInMark");
        // _bar = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "Bar");
    }


    public override void Refresh<T>(T data, int dataIndex)
    {
        base.Refresh(data, dataIndex);
        if (data is SignConfig signConfig)
        {

            // ShowMessage();
        }

    }

    public void ShowMessage(SignConfig signConfig)
    {

        _signConfig = signConfig;
        // //奖励物品
        // _rewardIcon.sprite = _signConfig._sRewardData._rewardSprite;

        //签到进度
        // _signPlan.text = _signConfig._planNow + "/" + _signConfig._planTaget;
        // _bar.fillAmount = (float) _signConfig._planNow / _signConfig._planTaget;
        //领取状态 更改图片显示

        if (signConfig._planState == 1) //已领取
        {
            _sginMask.SetActive(true);
        }
        else
        { 
            _sginMask.SetActive(false);
        }

        if (signConfig._nowSign)
        {
            _signBg.sprite = _iconBG[0];
        }
        else
        {
            _signBg.sprite = _iconBG[1];
        }
        
        _rewardDescription = "{0}元";
        _rewardText.text = string.Format(_rewardDescription, _signConfig._rewardNumber); 
        //更改连续打卡要求天数
        _date.text = string.Format(_dateText, _signConfig._planTaget);


    }


    private void Onclick()
    {

        // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDraw_SignClockIn);
        if (_signConfig._planState==1)
        {
            UI_HintMessage._.ShowMessage(/*transform.parent,*/"当前奖励已领完");
            return;
        }

        if (_signConfig._planNow < _signConfig._planTaget)
        {
            UI_HintMessage._.ShowMessage(/*transform.parent,*/"当前打卡天数不足");
            return;
        }

        //CB_WithDraw(null);
        //return;
        //判断是否微信登陆.
        if (!GL_PlayerData._instance.IsLoginWeChat())
        {
            //登陆微信
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WeChatLogin);
        }
        else
        {
            //提现广告
            GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_WithDrawSign, (set) =>
            {
                if (set)
                {
                    //提现广告播放成功
                    Net_WithDraw draw = new Net_WithDraw();
                    // draw.withDrawType = (int) EWithDrawType.Clockin;
                    draw.withDrawId = _signConfig.ID;
                    draw.type = 2;
                    draw.withDrawType = 6;
                    GL_ServerCommunication._instance.Send(Cmd.WithDraw, JsonUtility.ToJson(draw), CB_WithDraw);
                }
                else
                {
                    UI_HintMessage._.ShowMessage("播放广告失败，请重新尝试");
                }
            });
        
        }
    }


    private void CB_WithDraw(string param)
    {
        GL_PlayerData._instance.Net_CB_WithDrawResult(param);
        UIManager.GetInstance().CloseUIForms(SysDefine.UI_Path_NewSignInPage);
        float money = _signConfig._rewardNumber ;
        EWithDrawType _eWithDrawType = EWithDrawType.Clockin;
        var obj = new object[]
        {
            money,
            _eWithDrawType,
            GL_PlayerData._instance._netCbWithDraw.money
        };
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
        GL_PlayerData._instance.SendWithDrawConfig(EWithDrawType.Clockin, () =>
        { 
            UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_NetLoading);
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WithdrawSuccess, obj);
        });
       
    }

}

public enum SignState
{
    UnComplete = 0, //未完成 绿色
    UnGet,  //未领取 黄色
    Finish, //已领取 灰色
}
