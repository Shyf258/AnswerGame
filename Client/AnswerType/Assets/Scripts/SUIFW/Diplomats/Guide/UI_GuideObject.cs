using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GuideObject : MonoBehaviour
{
    private bool _haveCanvas;
    private Canvas _canvas;
    private Canvas[] _canvasList;
    private List<ParticleSystemRenderer> _effectList;
    private GraphicRaycaster _raycaster;
    private Button _btn;
    private Toggle _toggle;

    public bool SetGuideButton()
    {
        GL_GuideManager._instance._guide = this;
        _btn = GetComponent<Button>();
        _toggle = GetComponent<Toggle>();
        if (_btn == null && _toggle == null)
        {
            return false;
        }

        SetCanvas();

        //_btn.onClick.RemoveListener(OnBtnGuideListen);
        if (_btn != null)
            _btn.onClick.AddListener(OnBtnGuideListen);
        if (_toggle != null)
            _toggle.onValueChanged.AddListener(go =>
            {
                if (go)
                    OnBtnGuideListen();
            });

        return true;
    }

    #region Canvas
    private void SetCanvas()
    {
        _canvas = gameObject.GetComponent<Canvas>();
        if (_canvas == null)
        {
            _canvas = gameObject.AddComponent<Canvas>();
            _haveCanvas = false;
        }
        else
        {
            _haveCanvas = true;
        }
        _canvas.overrideSorting = true;
        _canvas.sortingOrder = 1;
        _raycaster = GL_Tools.GetComponent<GraphicRaycaster>(gameObject);

        Invoke(nameof(SetOrder), 0.05f);

    }

    private void SetOrder()
    {
        _canvasList = gameObject.GetComponentsInChildren<Canvas>();
        foreach (var c in _canvasList)
        {
            c.sortingOrder += (c.sortingOrder % 10) + 1000;
        }
        _effectList = new List<ParticleSystemRenderer>();
        FindP(transform);
    }

    public void FindP(Transform transs)
    {
        foreach (Transform t in transs)
        {
            //if (!t.gameObject.activeInHierarchy)
            {
                var p = t.GetComponent<ParticleSystemRenderer>();
                if (p != null)
                {
                    p.sortingOrder += 1100;
                    _effectList.Add(p);
                }


            }
            FindP(t);
        }
    }
    public void RemoveCanvas()
    {
        CancelInvoke(nameof(SetOrder));
        if (_raycaster != null)
            DestroyImmediate(_raycaster);

        //foreach (var c in _canvasList)
        //{
        //    c.sortingOrder -= 1100;
        //}
        //foreach (var e in _effectList)
        //{
        //    e.sortingOrder -= 1100;
        //}

        if (!_haveCanvas)
            DestroyImmediate(_canvas);
    }
    private void SetRaycast()
    {

    }
    #endregion


    public void OnBtnGuideListen()
    {
        if (_btn != null)
            _btn.onClick.RemoveListener(OnBtnGuideListen);
        if (_toggle != null)
        {
            _toggle.onValueChanged.RemoveListener(go =>
            {
                OnBtnGuideListen();
            });
        }
        //延迟触发
        //if (_delayTimer > 0)
        //{
        //    _delayCountDown = _delayTimer;
        //}
        //else
        {
            OnClickSuccess();
        }
    }

    private void OnClickSuccess()
    {
        RemoveCanvas();

        GL_GuideManager._instance.FinishGuide();
        GL_AudioPlayback._instance.Play(2);
    }

    public Vector3 GetUiPos(float z = 0)
    {
        return transform.position;
    }
}
