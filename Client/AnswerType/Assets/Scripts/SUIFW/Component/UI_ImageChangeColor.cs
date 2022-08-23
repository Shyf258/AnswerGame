using System;
using UnityEngine;
using UnityEngine.UI;

namespace SUIFW
{
    public class UI_ImageChangeColor : MonoBehaviour
    {
        public Sprite _enableSprite;
        public Sprite _disableSprite;

        private Image _img;

        private bool _initialized;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (_initialized) return;

            _initialized = true;
            _img = GetComponent<Image>();
        }

        public void SetColor(bool enable)
        {
            // if (!isActiveAndEnabled) return; // 不知道怎么加了OnEnable还是false
            if (!_initialized) Init();

            if (_img) _img.sprite = enable ? _enableSprite : _disableSprite;
        }
    }
}

