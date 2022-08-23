//  Copyright (c) 2020-present amlovey
//  
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UIElements;

namespace APlus2
{
    [Serializable]
    public class Column : ScriptableObject
    {
        public const int DEFAULT_WIDTH = 120;

        public string key;
        public string header;
        public int width;
        public bool visible;
        public NullableBoolean isDesc;
        public bool showSortIcon;
        public string tooltip;

        private void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }

        public static Column CreateInstance(string key, string header, bool visible = true, string tooltip = "")
        {
            return CreateInstance(key, header, DEFAULT_WIDTH, visible, tooltip);            
        }

        public static Column CreateInstance(string key, string header, int width, bool visible = true, string tooltip = "")
        {
            var instance = ScriptableObject.CreateInstance<Column>();
            instance.header = header;
            instance.width = width;
            instance.visible = visible;
            instance.key = key;
            instance.tooltip = string.IsNullOrEmpty(tooltip) ? header : tooltip;
            return instance;
        }

        public ColumnAction GetColumnAction()
        {
            if (!string.IsNullOrEmpty(this.key) && TableDefinitions.ActionsMap.ContainsKey(this.key.ToLower()))
            {
                var columnAction = TableDefinitions.ActionsMap[this.key.ToLower()];
                columnAction.SetColumn(this);
                return columnAction;
            }

            throw new Exception(string.Format("Key {0} is not exist in map", this.key));
        }
    }

    public abstract class ColumnAction
    {
        protected Column column;

        public void SetColumn(Column column)
        {
            this.column = column;
        }

        public abstract object GetRawData(APAsset asset);
        public abstract VisualElement CreateCellControl();
        public abstract void SetCellData(VisualElement element, object data);
        public abstract List<APAsset> DoSort(List<APAsset> list);
    }

    public class IndexColumnAction : ColumnAction
    {
        public IndexColumnAction() {}

        public override VisualElement CreateCellControl()
        {
            var label = UIHelper.CreateNoMarginPaddingLabel();
            label.style.width = this.column.width;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            return label;
        }

        public override List<APAsset> DoSort(List<APAsset> list)
        {
            return list;
        }

        public override void SetCellData(VisualElement element, object data)
        {
            var label = element as Label;
            if (label != null)
            {
                label.text = data.ToString();
            }
        }

        public override object GetRawData(APAsset asset)
        {
            return 0;
        }
    }

    public class FontIconColumnAction<T> : ColumnAction where T: APAsset
    {
        public Func<T, bool?> dataGetter;

        public FontIconColumnAction(Func<T, bool?> dataGetter)
        {
            this.dataGetter = dataGetter;
        }

        public override VisualElement CreateCellControl()
        {
            var label = UIHelper.CreateNoMarginPaddingLabel();
            label.AddToClassList("ap-icon");
            label.style.width = 24;
            label.style.height = 24;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            label.style.flexShrink = 0;
            label.style.alignSelf = Align.Center;
            return label;
        }

        public override List<APAsset> DoSort(List<APAsset> list)
        {
            var newList = list.Cast<T>();
            switch (this.column.isDesc)
            {
                case NullableBoolean.True:
                    newList = newList.OrderByDescending(this.dataGetter);
                    break;
                case NullableBoolean.False:
                    newList = newList.OrderBy(this.dataGetter);
                    break;
            }

            return newList.Cast<APAsset>().ToList();
        }

        public override object GetRawData(APAsset asset)
        {
            return this.dataGetter(asset as T);
        }

        public override void SetCellData(VisualElement element, object data)
        {
            var label = element as Label;
            var asset = data as T;
            if (label != null && asset != null)
            {
                label.text = UIHelper.GetUnsedMark(this.dataGetter(asset)).value;
            }
        }
    } 

    

    [Serializable]
    public class AssetNameColumnAction<T> : ColumnAction where T : APAsset
    {
        public AssetNameColumnAction() {}

        public override VisualElement CreateCellControl()
        {
            var nameControl = new AssetNameElement(this.column.width);
            return nameControl;
        }

        public override List<APAsset> DoSort(List<APAsset> list)
        {
            var newList = list.Cast<T>();
            switch (column.isDesc)
            {
                case NullableBoolean.True:
                    newList = newList.OrderByDescending(asset => asset.Name);
                    break;
                case NullableBoolean.False:
                    newList = newList.OrderBy(asset => asset.Name);
                    break;
            }

            return newList.Cast<APAsset>().ToList();
        }

        public override object GetRawData(APAsset asset)
        {
            return asset.Name;
        }

        public override void SetCellData(VisualElement element, object data)
        {
            var control = element as AssetNameElement;
            var asset = data as APAsset;
            if (control != null && asset != null)
            {
                control.SetData(asset);
            }
        }
    }

    [Serializable]
    public class LabelColumnAction<T> : ColumnAction where T: APAsset
    {
        public LabelColumnAction(Func<T, string> dataGetter, Func<T, object> selector)
        {
            this.dataGetter = dataGetter;
            this.selector = selector;
        }

        public Func<T, string> dataGetter;
        public Func<T, object> selector;

        public override VisualElement CreateCellControl()
        {
            var label = UIHelper.CreateNoMarginPaddingLabel();
            label.style.width = this.column.width;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            label.style.overflow = Overflow.Hidden;
            return label;
        }

        private string UpdateTextEllipsisIfNeeds(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var valMayWithEllipsis = value;
            var maxLength = Mathf.FloorToInt(this.column.width / 7);
            if (value.Length > maxLength)
            {
                valMayWithEllipsis = value.Substring(0, maxLength - 3) + "...";
            }

            return valMayWithEllipsis;
        }

        public override void SetCellData(VisualElement element, object data)
        {
            var label = element as Label;
            var asset = data as T;
            if (label != null && asset != null)
            {
                var value = this.dataGetter(asset);
                label.text = UpdateTextEllipsisIfNeeds(value);
                label.tooltip = label.text.Equals(value) ? null : value;
            }
        }

        public override List<APAsset> DoSort(List<APAsset> list)
        {
            if (this.selector == null)
            {
                return list;
            }

            var newList = list.Select(item => item as T);
            switch (column.isDesc)
            {
                case NullableBoolean.True:
                    newList = newList.OrderByDescending(this.selector);
                    break;
                case NullableBoolean.False:
                    newList = newList.OrderBy(this.selector);
                    break;
            }

            return newList.Cast<APAsset>().ToList();
        }

        public override object GetRawData(APAsset asset)
        {
            return this.selector(asset as T);
        }
    }
}