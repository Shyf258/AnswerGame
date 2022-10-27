using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SUIFW
{
    public class LanguageManagerByJson : IJsonManager
    {
        //保存（键值对）应用设置集合
        private static Dictionary<int, LanguageData> _AppSetting;

        /// <summary>
        /// 只读属性： 得到应用设置（键值对集合）
        /// </summary>
        public Dictionary<int, LanguageData> AppSetting
        {
            get { return _AppSetting; }
        }

        public LanguageManagerByJson(string jsonPath)
        {
            _AppSetting = new Dictionary<int, LanguageData>();
            //初始化解析Json 数据，加载到（_AppSetting）集合。
            InitAndAnalysisJson(jsonPath);
        }


        public int GetAppSettingMaxNumber()
        {
            if (_AppSetting != null && _AppSetting.Count >= 1)
            {
                return _AppSetting.Count;
            }
            else
            {
                return 0;
            }
        }

        public void InitAndAnalysisJson(string jsonPath)
        {
            TextAsset configInfo = null;
            LanguageDataInfo data = null;

            //参数检查
            if (string.IsNullOrEmpty(jsonPath)) return;
            //解析Json 配置文件
            try
            {
                configInfo = Resources.Load<TextAsset>(jsonPath);
                data = JsonUtility.FromJson<LanguageDataInfo>(configInfo.text);

            }
            catch
            {
                throw new JsonAnlysisException(GetType() + "/InitAndAnalysisJson()/Json Analysis Exception ! Parameter jsonPath=" + jsonPath);
            }
            //数据加载到AppSetting 集合中

            foreach (var nodeInfo in data.Info)
            {
                LanguageData d = new LanguageData();
                d.EN = nodeInfo.EN;
                d.CN = nodeInfo.CN;
                d.HK = nodeInfo.HK;
                d.FR = nodeInfo.FR;
                d.DE = nodeInfo.DE;
                d.IT = nodeInfo.IT;
                d.JA = nodeInfo.JA;
                d.PT = nodeInfo.PT;
                d.ES = nodeInfo.ES;
                d.KO = nodeInfo.KO;
                d.ID = nodeInfo.ID;
                _AppSetting.Add(nodeInfo.ID, d);
            }

        }
    }
}