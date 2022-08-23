//  Copyright (c) 2020-present amlovey
//  
using System;
using UnityEngine.UIElements;

namespace APlus2
{
    public class IconButton : Button
    {
        public Action OnClick;

        public IconButton(Icon icon)
        {
            this.style.backgroundImage = null;
            this.AddToClassList("ap-hoverable");
            this.AddToClassList("ap-icon-button");
            this.text = icon.value;
            this.clickable.clicked += OnClicked;
        }

        private void OnClicked()
        {
            if (OnClick != null)
            {
                OnClick();
            }
        }
    }

    public class TextButton : Button
    {
        public Action OnClick;

        public TextButton(string text)
        {
            this.style.backgroundImage = null;
            this.AddToClassList("ap-hoverable");
            this.AddToClassList("ap-text-button");
            this.text = text;
            this.clickable.clicked += OnClicked;
        }

        private void OnClicked()
        {
            if (OnClick != null)
            {
                OnClick();
            }
        }
    }
}