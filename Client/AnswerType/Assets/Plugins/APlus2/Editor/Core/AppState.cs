//  Copyright (c) 2020-present amlovey
//  
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;

namespace APlus2
{
    [Serializable]
    public class AppState : ScriptableObject
    {
        public List<AssetCacheItem> items;
        public List<string> keys;
        public string selectedMenuKey;
        public List<object> selections;
        public TableDefinitions tableDef;
        public string searchInputText;

        public List<APAsset> inHierachy;

        public bool menuShow;
        public bool changeHeaderLayerShow;

        public AssetCacheItem GetAssetCacheItem(string assetType)
        {
            foreach (var item in items)
            {
                if (item.assetType == assetType)
                {
                    return item;
                }
            }

            return null;
        }

        public List<APAsset> getCurrentAssetList()
        {
            switch (selectedMenuKey)
            {
                case APlusConstants.IN_HIERACHY_MENU_KEY:
                    return inHierachy;
                default:
                    foreach (var item in items)
                    {
                        if (item.assetType == selectedMenuKey)
                        {
                            return item.assets;
                        }
                    }
                break;
            }

            return null;
        }

        private void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }

        public void SyncAssetsDataFromCache()
        {
            CacheManager.LoadCacheIfNotExist();
            items = CacheManager.cache.cacheItems;
        }
    }
}
