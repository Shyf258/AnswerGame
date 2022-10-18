using SUIFW;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UI_IF_DragRedpack : BaseUIForm
{
    private UI_DragRedpack _drag;
    private Image _cdImage;
    private Transform _clickTransform;
    private Animator _animRedpack;

    private Rect _border;   //拖拽边界
    private Net_CB_VideoRed _videoRedpackConfig;
    private double _cdTime; //cd时间
    private double _totalTime; //CD总时间,临时写法
    private EDragRedpackState _state; 


    public override void Init()
    {
        _isFullScreen = false;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForms_Type = UIFormType.Topside;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;

        _cdImage = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "Home_icon_hongbao2");
        _clickTransform = UnityHelper.FindTheChildNode(gameObject, "an_click_01");

        Transform tf = UnityHelper.FindTheChildNode(gameObject, "an_hongbao_02");
        _animRedpack = tf.GetComponent<Animator>();
        _drag = GetComponentInChildren<UI_DragRedpack>();
        _drag.Init(this);

        //因为UI已经右上作为锚点, 所以所有坐标都是负数
        //_border = new Rect(_drag.transform.localPosition.x, -150f,
        //    (Screen.width + _drag.transform.localPosition.x * 2f) * -1f,
        //    (Screen.height - 150f * 2f) * -1f);
        _border = new Rect(_drag.transform.localPosition.x, -150f,
            -((UIManager.GetInstance().TraCanvasTransfrom as RectTransform).rect.width + _drag.transform.localPosition.x * 2),
            -((UIManager.GetInstance().TraCanvasTransfrom as RectTransform).rect.height - 300));
    }

    public override void onUpdate()
    {
        if(_dragIndex > 0)
        {
            _drag.transform.localPosition = Vector3.Lerp(_drag.transform.localPosition, _target, 0.4f);
        }

        if(State == EDragRedpackState.CD)
        {
            double time = _cdTime - GL_Time._instance.CalculateSeconds();
            float value = (float)(time / _totalTime);

            if (value <= 0)
            {
                RefreshState();
            }
            else
            {
                _cdImage.fillAmount = 1 - value;
            }

        }
    }

    public override void Refresh(bool recall)
    {
        RefreshState();
       
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void RefreshLanguage()
    {
    }

    private void RefreshState()
    {
        _videoRedpackConfig = GL_PlayerData._instance.GetVideoRedpackConfig(EVideoRedpackType.DragRedpack);
        if (_videoRedpackConfig == null || _videoRedpackConfig.videoRedLimit == 0)
        {
            State = EDragRedpackState.Exhaust;
            return;
        }

        //检测是否在CD中
        _cdTime = GL_PlayerData._instance.GetVideoRedpackCD(EVideoRedpackType.DragRedpack);
        if(_cdTime > 0)
        {
            double cur = GL_Time._instance.CalculateSeconds();
            if (cur >= _cdTime)
            {
                State = EDragRedpackState.Can;
            }
            else
            {
                _totalTime = _cdTime - cur;
                _cdImage.fillAmount = 0;
                State = EDragRedpackState.CD;
            }
            return;
        }

        State = EDragRedpackState.Can;
    }


    //按下红包
    private void OnClickButon()
    {
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.TurnIcon);
        
        //DDebug.LogError("~~~拖拽红包 按下按键");
        if(State == EDragRedpackState.Can)
        {
            Action action = () =>
            {
                RefreshState();
                GL_PlayerData._instance.GetTaskConfig();
            };

            int count;

            if (_videoRedpackConfig.mostBougs>0)
            {
                count = _videoRedpackConfig.bougs;
            }
            else
            {
                count = _videoRedpackConfig.coupon;
            }
            
            object[] objects = { ERewardSource.DragRedpack, _videoRedpackConfig.mostCoupon, _videoRedpackConfig.mostBougs, count, action };
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetNormal, objects);

            GL_AudioPlayback._instance.Play(21);
        }
        else if(State == EDragRedpackState.CD)
        {

        }
        else if (State == EDragRedpackState.Exhaust)
        {
            SUIFW.Diplomats.Common.UI_HintMessage._.ShowMessage("今日次数已用完，\r\n请明日在来领取");
        }
    }

    
    private Vector3 _offset; //偏移值
    private int _dragIndex;   //拖拽计数
    private Vector3 _target;    //目标
    public void DoMouseDown()
    {
        //按下时计算, 鼠标点击与图标偏移值
        Vector3 pos = Input.mousePosition;
        Vector3 mouse = GL_Tools.MouseToUGUIPosition(pos, _drag.transform.parent as RectTransform);
        _offset = _drag.transform.localPosition - mouse;
        //DDebug.LogError("~~~OnMouseDown: _drag.transform.position :" + _drag.transform.position);
    }
    public void DoMouseDrag()
    {
        //DDebug.LogError("OnMouseDrag");
        _dragIndex++;

        Vector3 pos = Input.mousePosition;
        Vector3 mouse = GL_Tools.MouseToUGUIPosition(pos, _drag.transform.parent as RectTransform);

        _target = mouse + _offset;
    }
    public void DoMouseUp()
    {
        if(_dragIndex > 0)
        {
            //DDebug.LogError("DoMouseUp");
            DragOver();
            _dragIndex = 0;
        }

    }
    public void DoMouseUpAsButton()
    {
        if(_dragIndex < 20)
        {
            //按下时间太长, 则不算一次点击
            OnClickButon();
        }
        else
        {
            
        }
        DragOver();
        _dragIndex = 0;
    }



    //拖拽结束
    private void DragOver()
    {
        //DDebug.LogError("~~~拖拽红包 拖拽结束");
        
        //拖拽计算边界
        Vector3 cur = _drag.transform.localPosition;
        if(cur.x >= _border.center.x)
        {
            cur.x = _border.x;
        }
        else
        {
            cur.x = _border.width + _border.x;
        }


        if (cur.y > _border.y)
        {
            cur.y = _border.y;
        }
        else if (cur.y < _border.height)
        {
            cur.y = _border.height + _border.y;
        }

        //开启移动
        StopAllCoroutines();
        StartCoroutine(DoMove(cur));
    }

    private IEnumerator DoMove(Vector3 target)
    {
        Vector3 start = _drag.transform.localPosition;
        float timer = 0;
        float time = 0.2f;
        do
        {
            timer += Time.deltaTime;
            _drag.transform.localPosition = Vector3.Lerp(start, target, timer / time);
            yield return null;
        } while (timer < time);
        _drag.transform.localPosition = target;
    }


    public EDragRedpackState State
    {
        get => _state;
        set
        {
            _state = value;
            if(value == EDragRedpackState.CD)
            {
                double cur = GL_Time._instance.CalculateSeconds();
                if(cur >= _cdTime)
                {
                    State = EDragRedpackState.Can;
                }
                else
                {
                    _totalTime = _cdTime - cur;
                    _cdImage.fillAmount = 0;
                }
                _clickTransform.SetActive(false);
                _animRedpack.Play("Idle");
            }
            else if (value == EDragRedpackState.Can)
            {
                _cdImage.fillAmount = 1;
                _clickTransform.SetActive(true);
                _animRedpack.Play("Sway");
            }
            else if (value == EDragRedpackState.Exhaust)
            {
                _cdImage.fillAmount = 0;
                _clickTransform.SetActive(false);
                _animRedpack.Play("Idle");
            }
        }
    }

   

}

public enum EDragRedpackState
{
    Can,    //可点击
    CD,     //CD中
    Exhaust, //没有次数
}