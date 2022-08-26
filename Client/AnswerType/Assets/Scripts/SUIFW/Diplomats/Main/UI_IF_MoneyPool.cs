#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_IF_MoneyPool
// 创 建 者：Yangsong
// 创建时间：2022年01月24日 星期一 14:27
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using DG.Tweening;
using SUIFW.Diplomats.Common;
using SUIFW.Helps;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace SUIFW.Diplomats.Main
{
    /// <summary>
    /// 奖金池
    /// </summary>
    public class UI_IF_MoneyPool : BaseUIForm
    {
        #region UIField

        private string _idleAnim = "an_vipidle_01";

        private Button _btnwithdraw;

        private Image _image;

        private ImgUV _imgUV;

        private float _startY;

        private Transform _effect;

        private Text _percentage;

        private float _maskMin = -60f;
        private float _maskMax = 260f;

        private Transform _effectTransform;


        private float _isCoolDown = 0;   //是否广告冷却

        private Transform _withDrawImage;
        private UI_NumberImage _numberImage;

        private Action _action;
        #endregion

        #region Override

        public override void onUpdate()
        {
            if (_isCoolDown > 0)
            {
                float value = _isCoolDown - Time.time;
                if(value < 0)
                {
                    _isCoolDown = 0;
                    RefreshIcon();
                    return;
                }
                _numberImage.SetNumber((int)value);
            }
        }

        public override void Init()
        {
            _isFullScreen = false;
            CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
            CurrentUIType.UIForms_Type = UIFormType.PopUp;
            CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

            RigisterButtonObjectEvent("BtnClose",(go => OnBtnClose()));
            _btnwithdraw = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "btn_withdraw");
            RigisterButtonObjectEvent(_btnwithdraw, (go => { OnBtnWithdraw(); }));
            _image = UnityHelper.GetTheChildNodeComponetScripts<Image>(_btnwithdraw.gameObject, "img");
            _imgUV = UnityHelper.GetTheChildNodeComponetScripts<ImgUV>(_image.gameObject, "mask");
            _effect = UnityHelper.FindTheChildNode(_btnwithdraw.gameObject, "Effect");
            _percentage = UnityHelper.GetTheChildNodeComponetScripts<Text>(_btnwithdraw.gameObject, "Percentage");
            _percentage.text = "0%";
            _startPos = _imgUV.transform.localPosition;

            _withDrawImage = UnityHelper.FindTheChildNode(_btnwithdraw.gameObject, "xianjinchi_09");
            _numberImage = _btnwithdraw.GetComponentInChildren<UI_NumberImage>();
            _numberImage .Init(false);

            _effectTransform = UnityHelper.FindTheChildNode(gameObject, "lightEffect");
        }

        public override void InitData(object data)
        {
            base.InitData(data);
            var datas = data as Object[];
            if (datas == null) 
                return;
            if (datas[0] is Action action)
            {
                _action = action;
            }
        }
        
        public override void PoppedRefresh()
        {
            _animator.Play(_idleAnim);
        }

        public override void RefreshLanguage()
        {

        }

        public override void Refresh(bool recall)
        {
            _effect.gameObject.SetActive(false);
            RefreshData();
            
             // GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Banner_GoldenPig);
        }

        public override void OnHide()
        {
           
            // GL_AD_Interface._instance.CloseBannerAd();
        }

        #endregion

        #region Event
        
        public void OnBtnWithdraw()
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.MoneyPoolGrow);
            if(_isCoolDown > 0)
            {
                UI_HintMessage._.ShowMessage("冷却中，可观看其他视频充能");
                return;
            }

            if (!_isCanWithdraw)
            {
                UI_HintMessage._.ShowMessage("提现次数不足");
                return;
            }
            
            if (!GL_PlayerData._instance.IsLoginWeChat() && _isArpuEnough)
            {
                //DDebug.Log("@@@@@@@@没有登录微信");
                //登陆微信
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WeChatLogin);
            }
            else 
            {
                //GL_SDK._instance.PopUp();
                GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_WithDrawPool, (set) => 
                {
                    if (set)
                    {
                        _isCoolDown = Time.time + 9.9f;
                        
                
                        if (_isArpuEnough)
                        {
                            //DDebug.Log("@@@@@@@@进入提现");
                            WithDraw();
                        }
                        else
                        {
                            RefreshData();
                        }
                    }
                },GL_ConstData.SceneID_MoneyPool);
            }
        }

        private void WithDraw()
        {
            //提现广告播放成功
            Net_WithDraw draw = new Net_WithDraw();
            draw.withDrawType = 4;
            draw.withDrawId = _curWithdraw.id;
            draw.type = 2;
            GL_ServerCommunication._instance.Send(Cmd.WithDraw, JsonUtility.ToJson(draw), CB_WithDraw);
        }

        private void CB_WithDraw(string param)
        {
            UI_HintMessage._.ShowMessage("提现成功");
            _curWithdraw.withDrawLimit = 0;
            RefreshUI();
            // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Arpu_Withdraw);

            RefreshData();
            
        }

        private void OnBtnClose()
        {
            CloseUIForm();
            if (_action!=null)
            {
                _action?.Invoke();
                _action = null;
            }
        }
        
        #endregion

        #region CustomField



        #endregion

        #region Logic

        /// <summary> 当前arpu值 </summary>
        private int _curArpu;

        private Net_CB_WithDraw _curWithdraw;

        /// <summary> 是否还有提现次数 </summary>
        private bool _isCanWithdraw;

        /// <summary> 当前arpu是否足够 </summary>
        private bool _isArpuEnough;

        /// <summary> 当前增加的arpu </summary>
        private int _curOffset;

        /// <summary> 波浪开始位置 </summary>
        private Vector3 _startPos;

        /// <summary> 当前需要的总共ARPU </summary>
        private int _curNeedTotalArpu;

        private void RefreshData()
        {
			RefreshIcon();
            GL_PlayerData._instance.SendWithDrawConfig( EWithDrawType.MoneyPool, ()=>
            {
                RefreshUI();
            });
        }

        private void RefreshUI()
        {
            var config = GL_PlayerData._instance.GetWithDrawConfig(EWithDrawType.MoneyPool);
            if (config == null)
                return;

            _isCanWithdraw = false;
            _isArpuEnough = false;


            foreach (var draw in config.couponWithDraws)
            {
                if (draw.withDrawLimit > 0)
                {
                    _curWithdraw = draw;

                    _curArpu = config.arpu;
                    // DDebug.Log("@@@@@@@发送提现配置刷新UI: " + _curArpu);
            
                    AddProgress();
            
                    if (_curArpu >= _curWithdraw.arpu)
                    {
                        // DDebug.Log("@@@@@@@@@@@@可以提现");
                        _isArpuEnough = true;
                        _effect.gameObject.SetActive(true);
                        _isCoolDown = 0;
                    }
                    
                    _isCanWithdraw = true;
                    break;
                }
            }
            RefreshIcon();
        }
        
        private void AddProgress()
        {
            // DDebug.Log("@@@@@@@@累积加个" + _curOffset);

            float value = (float)_curArpu / _curWithdraw.arpu;
            if (value > 1)
                value = 1;
            _imgUV.transform.localPosition = new Vector3(0, Mathf.Lerp(_maskMin, _maskMax, value));
            
            value *= 10000;
            value = Mathf.Floor(value);
            value /= 100f;
            
            _percentage.text = value.ToString() + "%";

            // _uiIfMain.AddProgress( value.ToString());
        }
        
        // private void AddProgress()
        // {
        //     DDebug.Log("@@@@@@@@累积加个" + _curOffset);
        //
        //     float value =GL_PlayerData._instance.RefreshCondition();
        //    
        //     _imgUV.transform.localPosition = new Vector3(0, Mathf.Lerp(_maskMin, _maskMax, value*0.01f));
        //   
        //     DDebug.LogError("****** 当前财神累计进度" + value);
        //     _percentage.text = value + "%";
        // }


        private void RefreshIcon()
        {
            bool show = _isCoolDown <= 0;
            _effectTransform.SetActive(show);
            _withDrawImage.gameObject.SetActive(show);
            _numberImage.gameObject.SetActive(!show);
        }

        
        #endregion

    }
}