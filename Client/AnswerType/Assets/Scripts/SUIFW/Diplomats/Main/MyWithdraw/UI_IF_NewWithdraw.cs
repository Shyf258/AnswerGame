#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_Obj_MyWithdraw
// 创 建 者：Yangsong
// 创建时间：2022年04月07日 星期四 15:26
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using SUIFW.Diplomats.Common;
using SUIFW.Diplomats.Main.MyWithdraw;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace SUIFW.Diplomats.Main.MyWithdraw
{
    /// <summary>
    /// 切换页我得提现
    /// </summary>
    public class UI_IF_NewWithdraw : BaseUIForm
    {
        #region UIField

        private ScrollRect _scrollRect;
        
        //玩家节点
        private Image _playerIcon;
        private Text _txtNickName;
        private Text _txtLevel;
        
        //假现金节点
        private Text _txtRedNum;
        private Transform _grpRed;
        private Text _txtRedTime; //假现金计时
        private Transform _tfRedTime;
        private Transform getRedNode;
        private UI_Button _btnGetRed;
        private Timer _redTimer;
        private DateTime _redDataTime;
        private UI_Button btnRedWithdraw;
        
        //金币节点
        private Text _txtGoldNum;
        private Text _txtGoldMoney;
        private Transform _grpGold;
        private Text _txtGoldTime; //金币计时
        private Transform _tfGoldTime;
        private Transform getGoldNode;
        private UI_Button _btnGetGold;
        private Timer _goldTimer;
        private DateTime _goldDataTime;
        private UI_Button btnGoldWithdraw;

        
        #region 提现增幅

        private Button _growRed;

        private Text _growRedText;
        
        private Button _growCoin;

        private Text _growCoinText;

        private string _growStr = "<color=#ff0000><size=52>{0}%</size></color>提现增幅";

        #endregion
        
        #endregion

        #region Override

        public override void RefreshLanguage()
        {
            
        }

        public override void Refresh(bool recall)
        {
            RefreshRed();
            RefreshGold();
            RefreshPlayer();
            RefreshRedCd();
            RefreshGoldCd();
            RefreshWithDrawGrow();
            _scrollRect.verticalNormalizedPosition = 1;
        }

        private void InitButtonViewABTest()
        {
            // if (!GL_CoreData._instance.AbTest)
            // {
            //     getRedNode.transform.localPosition = new Vector3(getRedNode.transform.localPosition.x, -253);
            //     btnRedWithdraw.transform.localPosition = new Vector3(btnRedWithdraw.transform.localPosition.x, -429);
            //     getGoldNode.transform.localPosition = new Vector3(getGoldNode.transform.localPosition.x, -330);
            //     btnGoldWithdraw.transform.localPosition = new Vector3(btnGoldWithdraw.transform.localPosition.x, -508);
            // }
        }
        private void TriggerGuide()
        {
            //GL_GuideManager._instance.TriggerGuide(EGuideTriggerType.UIWithdraw);
        }
        public void DoChangeScrollRect()
        {
            _scrollRect.verticalNormalizedPosition = 0.5f;
        }

        private void RefreshRed()
        {
            GL_PlayerData._instance.SendWithDrawConfig(EWithDrawType.CashWithDraw, () =>
            {
                Init(EnumMyWithdraw.Red);
            });
        }

        //刷新假现金计时
        private void RefreshRedCd()
        {
            _tfRedTime.SetActive(false);
            _btnGetRed.SetActive(true);
            
            var config = GL_PlayerData._instance.GetVideoRedpackConfig(EVideoRedpackType.WithdrawRedpack);
            if (config == null || config.videoRedLimit == 0)
            {
                return;
            }

            var cdTime = GL_PlayerData._instance.GetVideoRedpackCD(EVideoRedpackType.WithdrawRedpack);
            double cur = GL_Time._instance.CalculateSeconds();
            if (cur < cdTime)
            {
                _tfRedTime.SetActive(true);
                _btnGetRed.SetActive(false);
                
                var offset = cdTime - cur;
                _redDataTime = DateTime.Now;
                _redDataTime = _redDataTime.AddSeconds(offset);
                if (_redTimer == null)
                {
                    _redTimer = new Timer(this);
                }
                _redTimer.StartCountdown(_redDataTime,(() =>
                {
                    _redTimer?.Stop();
                    _tfRedTime.SetActive(false);
                    _btnGetRed.SetActive(true);
                }),_txtRedTime);
            }
        }

        private void RefreshGold()
        {
            GL_PlayerData._instance.SendWithDrawConfig(EWithDrawType.DailyWithDraw, () =>
            {
                Init(EnumMyWithdraw.Gold);

                //因为需要排序, 所以延迟一会检测
                if(GL_PlayerData._instance.IsEnoughCoin())
                    Invoke(nameof(TriggerGuide), 0.05f);
            });
        }
        
        //刷新金币计时
        private void RefreshGoldCd()
        {
            _tfGoldTime.SetActive(false);
            _btnGetGold.SetActive(true);
            
            var config = GL_PlayerData._instance.GetVideoRedpackConfig(EVideoRedpackType.WithdrawCoin);
            if (config == null || config.videoRedLimit == 0)
            {
                return;
            }

            var cdTime = GL_PlayerData._instance.GetVideoRedpackCD(EVideoRedpackType.WithdrawCoin);
            double cur = GL_Time._instance.CalculateSeconds();
            if (cur < cdTime)
            {
                _tfGoldTime.SetActive(true);
                _btnGetGold.SetActive(false);
                
                var offset = cdTime - cur;
                _goldDataTime = DateTime.Now;
                _goldDataTime = _goldDataTime.AddSeconds(offset);
                if (_goldTimer == null)
                {
                    _goldTimer = new Timer(this);
                }
                _goldTimer.StartCountdown(_goldDataTime,(() =>
                {
                    _goldTimer?.Stop();
                    _tfGoldTime.SetActive(false);
                    _btnGetGold.SetActive(true);
                }),_txtGoldTime);
            }
        }

        public override void onUpdate()
        {
            
        }

        public override void Init()
        {
            _isOpenMainUp = false;
            
            _scrollRect = UnityHelper.GetTheChildNodeComponetScripts<ScrollRect>(gameObject, "Scroll View");
            //玩家
            var playerNode = UnityHelper.GetTheChildNodeComponetScripts<Transform>(gameObject, "PlayerNode");
            _playerIcon = UnityHelper.GetTheChildNodeComponetScripts<Image>(playerNode.gameObject, "PlayIcon");
            _txtNickName = UnityHelper.GetTheChildNodeComponetScripts<Text>(playerNode.gameObject, "_txtNickName");
            _txtLevel = UnityHelper.GetTheChildNodeComponetScripts<Text>(playerNode.gameObject, "_txtLevel");
            var _btnSet = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(playerNode.gameObject, "_btnSet");
            _btnSet.onClick.AddListener((() => { OnBtnSet(); }));

            //假现金
            var redNode = UnityHelper.GetTheChildNodeComponetScripts<Transform>(gameObject, "RedNode");
            _txtRedNum = UnityHelper.GetTheChildNodeComponetScripts<Text>(redNode.gameObject, "_txtRedNum");
            _grpRed = UnityHelper.GetTheChildNodeComponetScripts<Transform>(redNode.gameObject, "_grpRed");
            btnRedWithdraw = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(redNode.gameObject, "_btnRedWithdraw");
            btnRedWithdraw.onClick.AddListener(OnBtnRedWithdraw);
            getRedNode = UnityHelper.GetTheChildNodeComponetScripts<Transform>(gameObject, "GetRedNode");
            _btnGetRed = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(getRedNode.gameObject, "_btnGetRed");
            _btnGetRed.onClick.AddListener(OnBtnGetRed);
            _tfRedTime = UnityHelper.GetTheChildNodeComponetScripts<Transform>(getRedNode.gameObject, "RedTime");
            _txtRedTime = UnityHelper.GetTheChildNodeComponetScripts<Text>(getRedNode.gameObject, "_txtRedTime");
            
            _growRed = UnityHelper.GetTheChildNodeComponetScripts<Button>(redNode.gameObject, "OpenGrow");
            _growRedText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_growRed.gameObject, "Text");
            RigisterButtonObjectEvent(_growRed, go =>
            {
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_DayGrow);
            });

            
            //金币
            var goldNode = UnityHelper.GetTheChildNodeComponetScripts<Transform>(gameObject, "GoldNode");
            _txtGoldMoney = UnityHelper.GetTheChildNodeComponetScripts<Text>(goldNode.gameObject, "_txtGoldMoney");
            _txtGoldNum = UnityHelper.GetTheChildNodeComponetScripts<Text>(goldNode.gameObject, "_txtGoldNum");
            _grpGold = UnityHelper.GetTheChildNodeComponetScripts<Transform>(goldNode.gameObject, "_grpGold");
            btnGoldWithdraw = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(goldNode.gameObject, "_btnGoldWithdraw");
            btnGoldWithdraw.onClick.AddListener(OnBtnGoldWithdraw);
            getGoldNode = UnityHelper.GetTheChildNodeComponetScripts<Transform>(gameObject, "GetGoldNode");
            _btnGetGold = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(getGoldNode.gameObject, "_btnGetGold");
            _btnGetGold.onClick.AddListener(OnBtnGetGold);
            _tfGoldTime = UnityHelper.GetTheChildNodeComponetScripts<Transform>(getGoldNode.gameObject, "GoldTime");
            _txtGoldTime = UnityHelper.GetTheChildNodeComponetScripts<Text>(getGoldNode.gameObject, "_txtGoldTime");

            _growCoin = UnityHelper.GetTheChildNodeComponetScripts<Button>(goldNode.gameObject, "OpenGrow");
            _growCoinText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_growCoin.gameObject, "Text");
            RigisterButtonObjectEvent(_growCoin, go =>
            {
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_DayGrow);
            });
            
            InitButtonViewABTest();
        }

        #endregion

        #region Event

        private void OnBtnSet()
        {
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Setting);
        }

        /// <summary>
        /// 红包记录
        /// </summary>
        private void OnBtnRedRecord()
        {
            UI_Diplomats._instance.ShowUI(SysDefine.UI_IF_MyWithdrawRecord);
        }

        /// <summary>
        /// 元宝记录
        /// </summary>
        private void OnBtnGoldRecord()
        {
            UI_Diplomats._instance.ShowUI(SysDefine.UI_IF_MyWithdrawRecord);
        }

        /// <summary>
        /// 红包提现
        /// </summary>
        private void OnBtnRedWithdraw()
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDrawRed);

            //选择默认第一条提现
            if (_curRedWithdrawData == null)
            {
                var withdrawItem = _goldItems[0].GetComponent<UI_Btn_NewWithdrawItem>();
                _curRedWithdrawData = withdrawItem.GetWithdrawData();
            }

            if (_curRedWithdrawData == null)
            {
                return;
            }

            RedWithdraw();
        }

        /// <summary>
        /// 看广告得假现金
        /// </summary>
        private void OnBtnGetRed()
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetRedCoin);
            GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_WithDrawCoin, (success) =>
            {
                if (success)
                {
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetRedCoinFinish);
                    GL_PlayerData._instance.SendGetVideoRedpack(EVideoRedpackType.WithdrawRedpack,true,(msg =>
                    {
                        Action action = () =>
                        {
                            RefreshGold();
                            RefreshRed();
                            RefreshRedCd();
                            RefreshWithDrawGrow();
                        };
                        object[] datas = { msg.rewards, action,true};
                        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetResult,datas);
                    }));
                }
            });
        }
        
        /// <summary>
        /// 元宝提现
        /// </summary>
        private void OnBtnGoldWithdraw()
        {
            
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDrawCoin);

            //选择默认第一条提现
            if (_curGoldWithdrawData == null)
            {
                var withdrawItem = _goldItems[0].GetComponent<UI_Btn_NewWithdrawItem>();
                _curGoldWithdrawData = withdrawItem.GetWithdrawData();
            }

            if (_curGoldWithdrawData == null)
            {
                return;
            }

            GoldWithdraw();
        }

        /// <summary>
        /// 看广告获得金币
        /// </summary>
        private void OnBtnGetGold()
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetCoinCoin);
            GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_WithDrawCoin, (success) =>
            {
                if (success)
                {
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetCoinCoinFinish);
                    GL_PlayerData._instance.SendGetVideoRedpack(EVideoRedpackType.WithdrawCoin,true,(msg =>
                    {
                        Action action = () =>
                        {
                            RefreshGold();
                            RefreshRed();
                            RefreshGoldCd();
                            RefreshWithDrawGrow();
                        };
                        object[] datas = { msg.rewards, action,true};
                        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetResult,datas);
                    }));
                }
            });
        }

        #endregion

        #region CustomField
        
        private string _itemPrefabPath = "SUIFW/Prefab/Main/MyWithdraw/UI_Btn_NewWithdrawItem";
        
        //官职不足时
        private List<string> _tipsList = new List<string>()
        {
            "官职未到达，请继续努力",
            "需要当日观看视频{0}次！",
            "请提现上一个额度！",
            "金币数不足，请继续努力",
            "关卡未到达，请继续努力",
            "需要金币{0}",
            "现金数不足，请继续努力",
            "提现次数不足"
        };

      
        
        private MyWithdrawData _curRedWithdrawData;
        private MyWithdrawData _curGoldWithdrawData;
        
        private Dictionary<int, UI_Btn_NewWithdrawItem> _redItems = new Dictionary<int, UI_Btn_NewWithdrawItem>();       
        private Dictionary<int, UI_Btn_NewWithdrawItem> _goldItems = new Dictionary<int, UI_Btn_NewWithdrawItem>(); 

        #endregion

        #region Logic

        public void Init(EnumMyWithdraw enumMyWithdraw)
        {
            switch (enumMyWithdraw)
            {
                case EnumMyWithdraw.Red:
                    _curRedWithdrawData = null;
                    _txtRedNum.text = GL_PlayerData._instance.Bogus_Convert.ToString();
                    CreateRed();
                    break;
                case EnumMyWithdraw.Gold:
                    _curGoldWithdrawData = null;
                    _txtGoldNum.text = GL_PlayerData._instance.Coin.ToString();
                    _txtGoldMoney.text = GL_PlayerData._instance.Coin_RMB + "元";
                    CreateGold();
                    break;
            }
        }
        
        public void SetRedSelectState()
        {
            if (_curRedWithdrawData == null)
            {
                return;
            }
            
            int targetIndex = _curRedWithdrawData.Index;
            int index = 0;
            _redItems.Values.ToList().ForEach((item =>
            {
                if (item.IsSelected())
                {
                    item.Selectable(false);
                }

                if (targetIndex == index)
                {
                    item.Selectable(true);
                }
                
                index++;
            }));
        }
        
        public void SetGoldSelectState()
        {
            if (_curGoldWithdrawData == null)
            {
                return;
            }
            
            int targetIndex = _curGoldWithdrawData.Index;
            int index = 0;
            _goldItems.Values.ToList().ForEach((item =>
            {
                if (item.IsSelected())
                {
                    item.Selectable(false);
                }

                if (targetIndex == index)
                {
                    item.Selectable(true);
                }
                
                index++;
            }));
        }

        #region 创建

        /// <summary>
        /// 创建假现金
        /// </summary>
        private void CreateRed()
        {
            var config = GL_PlayerData._instance.GetWithDrawConfig(EWithDrawType.CashWithDraw);
            var list = config.couponWithDraws;
            if (list.Count <= 0)
                return;
            for (int i = 0; i < list.Count; i++)
            {
                UI_Btn_NewWithdrawItem item;
                if (!_redItems.ContainsKey(i))
                {
                    var prefab = GL_LoadAssetMgr._instance.Load<UI_Btn_NewWithdrawItem>(_itemPrefabPath);
                    item = Instantiate(prefab, _grpRed, false);
                    item.InitObjectNode();
                    _redItems[i] = item;
                }
                else
                {
                    item = _redItems[i];
                }
                
                MyWithdrawData data = new MyWithdrawData();
                data.EnumMyWithdraw = EnumMyWithdraw.Red;
                data.Index = i;
                data.Action = () =>
                {
                    _curRedWithdrawData = data;
                    SetRedSelectState();
                };
                data.WithDraw = list[i];
                data.IsCanWithdraw = IsRedCanWithdraw(data, false);
                if (data.Index == 0)
                {
                    _curRedWithdrawData = data;
                    SetRedSelectState();
                }
                item.Init(this,data);
            }
        }

        /// <summary>
        /// 创建元宝
        /// </summary>
        private void CreateGold()
        {
            var config = GL_PlayerData._instance.GetWithDrawConfig(EWithDrawType.DailyWithDraw);
            var list = config.couponWithDraws;
            if (list.Count <= 0)
                return;
            for (int i = 0; i < list.Count; i++)
            {
                UI_Btn_NewWithdrawItem item;
                if (!_goldItems.ContainsKey(i))
                {
                    var prefab = GL_LoadAssetMgr._instance.Load<UI_Btn_NewWithdrawItem>(_itemPrefabPath);
                    item = Instantiate(prefab, _grpGold, false);
                    item.InitObjectNode();
                    _goldItems[i] = item;
                }
                else
                {
                    item = _goldItems[i];
                }
                
                MyWithdrawData data = new MyWithdrawData();
                data.EnumMyWithdraw = EnumMyWithdraw.Gold;
                data.Index = i;
                data.Action = () =>
                {
                    _curGoldWithdrawData = data;
                    SetGoldSelectState();
                };
                data.WithDraw = list[i];
                data.IsCanWithdraw = IsGoldCanWithdraw(data, false);
                if (data.Index == 0)
                {
                    _curGoldWithdrawData = data;
                    SetGoldSelectState();
                }
                item.Init(this,data);
            }
        }

        #endregion

        #region 提现

        private bool IsRedCanWithdraw(MyWithdrawData withdrawData,bool isHint)
        {
            if (GL_PlayerData._instance.Bogus < withdrawData.WithDraw.money)
            {
                if (isHint) UI_HintMessage._.ShowMessage(_tipsList[6]);
                return false;
            }
            if (withdrawData.WithDraw.money>=30000)
            {
                if (withdrawData.WithDraw.money>=35000)
                {
                    if (isHint) UI_HintMessage._.ShowMessage("请先提取上一额度");
                }
                else
                {
                    if (GL_PlayerData._instance.UserDayLevel>80)
                    {
                        if (isHint) UI_HintMessage._.ShowMessage("提现300元需要连续登录7天，且每天答对80道题目");
                    }
                    else
                    {
                        if (isHint) UI_HintMessage._.ShowMessage("每日答对80道题目即可提现300元");
                    }
                   
                }
                return false;
            }
            //2.广告数量不满足时
            int num = withdrawData.WithDraw.viewAdTimes - GL_PlayerData._instance.SystemConfig.viewAds;
            
            if (num > 0)
            {
                string tips = string.Format(_tipsList[1], num);
                if (isHint) UI_HintMessage._.ShowMessage(tips);
                return false;
            }

            return true;
        }

        private void RedWithdraw()
        {
            if (!IsRedCanWithdraw(_curRedWithdrawData,true))
            {
                return;
            }

            //判断是否微信登陆.
            if (!GL_PlayerData._instance.IsLoginWeChat())
            {
                //登陆微信
                Action show = () =>
                {
                    RefreshPlayer();
                    RefreshWithDrawGrow();  
                };
                
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WeChatLogin,show);
            }
            else
            {
                PlayAD(_curRedWithdrawData);
            }
        }

        public bool IsGoldCanWithdraw(MyWithdrawData withDraw,bool isHint)
        {
            //当前不需要提现上一个额度
            // if (IsNeedWithdrawToPre(index))
            // {
            //     //不是第一个可提现的
            //     if (isHint) UI_HintMessage._.ShowMessage(_tipsList[2]);
            //     return false;
            // }

            //提现次数
            if (withDraw.WithDraw.withDrawLimit == 0)
            {
                if (isHint) UI_HintMessage._.ShowMessage(_tipsList[7]);
                return false;
            }
            
            //0.金币
            if (GL_PlayerData._instance.Coin < withDraw.WithDraw.coupon)
            {
                if (isHint) UI_HintMessage._.ShowMessage(_tipsList[3]);
                return false;
            }

            //1.检测关卡等级
            if (GL_PlayerData._instance.CurLevel - 1 < withDraw.WithDraw.level)
            {
                if (isHint) UI_HintMessage._.ShowMessage(_tipsList[4]);
                return false;
            }
            //2.广告数量不满足时
            int num = withDraw.WithDraw.viewAdTimes - GL_PlayerData._instance.SystemConfig.viewAds;
            num = 0;
            if (num > 0)
            {
                string tips = string.Format(_tipsList[1], num);
                if (isHint) UI_HintMessage._.ShowMessage(tips);
                return false;
            }

            return true;
        }

        private void GoldWithdraw()
        {

            switch (_curGoldWithdrawData.WithDraw.money)
            {
                case 38:
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDrawLow);
                    break;
                case 68:
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDrawMedium);
                    break;
                case 88:
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDrawHigh);
                    break;
            }
            
            if (!IsGoldCanWithdraw(_curGoldWithdrawData, true))
            {
                return;
            }

            //判断是否微信登陆.
            if (!GL_PlayerData._instance.IsLoginWeChat())
            {
                //登陆微信
                Action show = () =>
                {
                    RefreshPlayer();
                    RefreshWithDrawGrow();
                };
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WeChatLogin,show);
                // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Setting);
            }
            else
            {
               PlayAD(_curGoldWithdrawData);
            }
        }

        private void PlayAD(MyWithdrawData withdrawData)
        {
            //提现广告播放成功
            Net_WithDraw draw = new Net_WithDraw();
            draw.withDrawId = withdrawData.WithDraw.id;
            GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_WithDrawCoin, (set) =>
            {
                if (set)
                {
                    if (withdrawData.EnumMyWithdraw == EnumMyWithdraw.Red)
                    {
                        draw.withDrawType = 3;
                        draw.type = 2;
                        GL_ServerCommunication._instance.Send(Cmd.WithDraw, JsonUtility.ToJson(draw), CB_RedWithDraw);

                    }
                    else if (withdrawData.EnumMyWithdraw == EnumMyWithdraw.Gold)
                    {
                        draw.withDrawType = 7;
                        draw.type = 2;
                        GL_ServerCommunication._instance.Send(Cmd.WithDraw, JsonUtility.ToJson(draw), CB_GoldWithDraw);
                    }
                }
            });
        }
        
        //提现回调
        private void CB_RedWithDraw(string param)
        {
            GL_PlayerData._instance.Net_CB_WithDrawResult(param);
            float money = _curRedWithdrawData.WithDraw.money * 0.01f;
            EWithDrawType _eWithDrawType = EWithDrawType.CashWithDraw;
            var obj = new object[]
            {
                money,
                _eWithDrawType,
                GL_PlayerData._instance._netCbWithDraw.money
            };
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WithdrawSuccess, obj);
            GL_PlayerData._instance.Bogus -= _curRedWithdrawData.WithDraw.money;
            RefreshRed();
            RefreshWithDrawGrow();
            GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency);
        }
        
        private void CB_GoldWithDraw(string param)
        {
            
            switch (_curGoldWithdrawData.WithDraw.money)
            {
                case 38:
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDrawLowSuccess);
                    break;
                case 68:
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDrawMediumSuccess);
                    break;
                case 88:
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDrawHighSuccess);
                    break;
            }
            GL_PlayerData._instance.Net_CB_WithDrawResult(param);
            float money = _curGoldWithdrawData.WithDraw.money * 0.01f;
            EWithDrawType _eWithDrawType = EWithDrawType.DailyWithDraw;
            var obj = new object[]
            {
                money,
                _eWithDrawType,
                GL_PlayerData._instance._netCbWithDraw.money
            };
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WithdrawSuccess, obj);
            GL_PlayerData._instance.Coin -= _curGoldWithdrawData.WithDraw.coupon;
            RefreshGold();
            RefreshWithDrawGrow();
            GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency);
        }

        /// <summary>
        /// 是否需要提现上一个额度
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsNeedWithdrawToPre(int index)
        {
            var withDrawList = GL_PlayerData._instance.GetWithDrawConfig(EWithDrawType.DailyWithDraw);
            bool isWithdraw = false;
            List<Net_CB_WithDraw> withDraws = withDrawList.couponWithDraws;
            int curIndex = index;
            if (curIndex > 0)
            {
                int preIndex = curIndex - 1;
                if (withDraws[preIndex].withDrawLimit > 0)
                {
                    isWithdraw = true;
                }
            }
            
            return isWithdraw;
        }

        #endregion
        
        /// <summary>
        /// 刷新玩家信息
        /// </summary>
        private void RefreshPlayer()
        {
            GL_PlayerData._instance.GetWeChatIcon((t) =>
            {
                _playerIcon.sprite = t;
                _txtNickName.text = GL_PlayerData._instance.WeChatName;
                _txtLevel.text = "Lv." + GL_PlayerData._instance.CurLevel;
            });
        }
        
        #region 提现增幅刷新显示

        private void RefreshWithDrawGrow()
        {
            MethodExeTool.Invoke(() => { ShowWithDrawGrow(); }, 0.5f);
        }

        private void ShowWithDrawGrow()
        {
            if (GL_PlayerData._instance._WithDrawGrowConfig==null)
            {
                _growCoin.SetActive(false);
                _growRed.SetActive(false);
            }
            else
            {
                _growCoin.SetActive(true);
                _growRed.SetActive(true);
                string description = String.Format(_growStr,(GL_PlayerData._instance._WithDrawGrowConfig.growth));
                _growCoinText.text = description;
                _growRedText.text = description;
            }
        }

        #endregion
        
        
        
        #endregion
        
    }
}

public class MyWithdrawData
{
    public EnumMyWithdraw EnumMyWithdraw;
    public int Index;
    public Action Action;
    /// <summary> 是否能提现 </summary>
    public bool IsCanWithdraw;
    public Net_CB_WithDraw WithDraw = new Net_CB_WithDraw();
}

public enum EnumMyWithdraw
{
    Red,
    Gold
}
