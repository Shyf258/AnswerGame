#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_Obj_ActivitySign
// 创 建 者：Yangsong
// 创建时间：2022年07月29日 星期五 18:45
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using DataModule;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Main
{
    /// <summary>
    /// 活动签到对象
    /// </summary>
    public class UI_Obj_Activity : UIObjectBase
    {
        #region UIField

        /// <summary> 活动描述 </summary>
        private Text _txtActivity;
        
        /// <summary> 活动子描述 </summary>
        private Text _txtSub;

        /// <summary> 图标 </summary>
        private Image _imgIcon;

        private Text _text;

        private Text _textCd;

        private UI_Button _btn;


        #endregion

        #region Override

        public override void InitObjectNode()
        {
            _txtActivity = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtActivity");
            _txtSub = UnityHelper.GetTheChildNodeComponetScripts<Text>(_txtActivity.gameObject, "_txtSub");
            _imgIcon = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "_imgIcon");
            _btn = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(gameObject, "_btnGet");
            _text = UnityHelper.GetTheChildNodeComponetScripts<Text>(_btn.gameObject, "_text");
            _textCd = UnityHelper.GetTheChildNodeComponetScripts<Text>(_btn.gameObject, "_textCd");
            RigisterButtonObjectEvent(_btn,(go => OnBtnGet()));
        }

        public void RefreshNode(TableActivityData data)
        {
            _curData = data;
            _txtActivity.text = data.Description;
            _txtSub.text = data.SubDescription;
            var icon = GL_SpriteAtlasPool._instance.GetSprite("WelfareIcon", data.IconName);
            _imgIcon.sprite = icon;
            if (data.ID == 1)
            {
                if (!_isCdEnd)
                {
                    SetState(true);
                    StartCd(_intervalTime);
                }
                else
                {
                    SetState(false);
                }
            }
            else
            {
                SetState(false);
            }
        }

        #endregion

        #region Event

        private void OnBtnGet()
        {
            if (_curData == null)
                return;

            switch ((EnumActivity)_curData.ID)
            {
                case EnumActivity.WatchVideo:
                    OnWatchVideo();
                    break;
                case EnumActivity.RedPacketGroup:
                    OnRedPacketGroup();
                    break;
                case EnumActivity.AnswerQuest:
                    OnAnswerQuest();
                    break;
            }
        }

        //看视频
        private void OnWatchVideo()
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.ActivityVideoClick);
            GL_PlayerData._instance.SendGamecoreConfig(EGamecoreType.ActivityVideo, (() =>
            {
                var config = GL_PlayerData._instance.GetGamecoreConfig(EGamecoreType.ActivityVideo);
                if (config.dayAcceptTimes <= 0)
                {
                    UI_HintMessage._.ShowMessage("次数已经用完，请明日再来");
                }
                else
                {
                    // if (_nextTime<=0)
                    // {
                    //     _nextTime = Time.time;
                    // }
                    // else
                    // {
                    //     float offsetTime = Time.time - _nextTime;
                    //     if (offsetTime > config.intervalTime)
                    //     {
                    //         _nextTime = Time.time;
                    //     }
                    //     else
                    //     {
                    //         int second = (int)(config.intervalTime - offsetTime);
                    //         UI_HintMessage._.ShowMessage($"请在{second}秒后在进行领取");
                    //         return;
                    //     }
                    // }

                    if (!_isCdEnd)
                    {
                        UI_HintMessage._.ShowMessage($"冷却中，请稍后");
                        return;
                    }
                    
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.ActivityVideoPlayStart);
                    GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_ActivityVideo,(success =>
                    {
                        if (success)
                        {
                            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.ActivityVideoPlayEnd);
                            GL_PlayerData._instance.SendGamecoreAccept(EGamecoreType.ActivityVideo,0,(accept =>
                            {
                                Action action = () =>
                                {
                                    config.dayAcceptTimes -= 1;
                                };
                                object[] datas = { accept.rewards, action,false,true}; //是否激活mainUp
                                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetResult,datas);
                            }));
                            
                            //cd
                            _intervalTime = config.intervalTime;
                            _isCdEnd = false;
                            StartCd(config.intervalTime);
                        }
                    }));
                }
            }));
        }

        //红包群
        private void OnRedPacketGroup()
        {
            
        }

        //答题
        private void OnAnswerQuest()
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.ActivityAnswerClick);
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_PureAnswer);
        }

        #endregion

        #region CustomField

        private float _nextTime;

        private TableActivityData _curData;

        public Sprite[] BtnIcon;
        
        private Timer _timer;
        private DateTime _dataTime;

        private int _intervalTime;

        private bool _isCdEnd = true;

        #endregion

        #region Logic

        /// <summary>
        /// 设置按钮状态
        /// </summary>
        /// <param name="isCd"></param>
        private void SetState(bool isCd)
        {
            if (!isCd)
            {
                _btn.image.sprite = BtnIcon[0];
                _text.SetActive(true);
                _textCd.SetActive(false);
            }
            else
            {
                _btn.image.sprite = BtnIcon[1];
                _text.SetActive(false);
                _textCd.SetActive(true);
            }
            
        }
        
        //刷新计时
        private void StartCd(int time)
        {
            SetState(true);
            _dataTime = DateTime.Now;
            _dataTime = _dataTime.AddSeconds(time);
            if (_timer == null)
            {
                _timer = new Timer(null, true); //全局时间
            }
                
            _timer.StartCountdown(_dataTime,(() =>
            {
                _timer?.Stop();
                _isCdEnd = true;
                SetState(false);
            }),_textCd);
        }

        #endregion
    }
}