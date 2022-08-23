using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SUIFW
{
    public class UI_TextChangeColor : MonoBehaviour
    {
        public STextColorInfo _enableInfo;
        public STextColorInfo _disableInfo;

        private Text _text;
        private List<Outline> _outlines;
        private List<Shadow> _shadows;

        private bool _initialized;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (_initialized) return;

            _text = GetComponent<Text>();
            _outlines = GetComponents<Outline>().ToList();
            _shadows = GetComponents<Shadow>().ToList();
            _initialized = true;
        }

        public void SetColor(bool enable)
        {
            var info = enable ? _enableInfo : _disableInfo;
            if (info == null) return;
            if (!_initialized) Init();

            /* 顺序不能变, Outline继承自Shadow */
            _text.color = info.textColor;
            _shadows.ForEach(s => s.effectColor = info.shadowColor);
            _outlines.ForEach(o => o.effectColor = info.outlineColor);
        }

        [System.Serializable]
        public class STextColorInfo
        {
            public Color textColor;
            public Color outlineColor;
            public Color shadowColor;
        }
    }
}



