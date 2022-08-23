//2018.09.19    关林
//数据表 解析


using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataModule
{
    public class DataModuleManager
    {
        //是否加载, 只加载一次
        private bool _isLoad = false;

        /// <summary>
        /// 加载所有json数据模板
        /// </summary>        
        public void LoadAllDataModule()
        {
            if (_isLoad) return;
            _isLoad = true;
            Type dmm = typeof(DataModuleManager);
            foreach (var fi in dmm.GetProperties())
            {
                string[] sArray = fi.Name.Split('_');
                if (sArray.Length < 2 || sArray[0] == string.Empty)
                    continue;
                Type dataClass = Type.GetType("DataModule." + sArray[0]);
                if (dataClass == null)
                {
                    Debug.LogError(sArray[0] + " Can not Find!\n 检查：是否使用了名空间DataModule，_Dictionary前缀是否为类名");
                } 

                var method = dataClass.GetMethod("loadAllData");
                fi.SetValue(this, method.Invoke(null, null), null);
            }

            //ABtest 特殊处理
//#if Version_B
//        LoadTableABTest();
//#endif

            try
            {
                Dictionary<int, TableAnswerInfoData>  dic = JsonHelper.FromJson<Dictionary<int, TableAnswerInfoData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableAnswerInfo" +"_"+ AppSetting.BuildApp.ToString()).text);

                TableAnswerInfoData_Dictionary = dic;
                
            }
            catch 
            {
                  
            }
            
         
        }

        public Dictionary<int, TableGlobalVariableData> TableGlobalVariableData_Dictionary { get; set; }
        public Dictionary<int, TableAudioData> TableAudioData_Dictionary { get; set; }
        public Dictionary<int, TableEffectData> TableEffectData_Dictionary { get; set; }
        public Dictionary<int, TableNetworkRequestData> TableNetworkRequestData_Dictionary { get; set; }
        public Dictionary<int, TableNetworkURLData> TableNetworkURLData_Dictionary { get; set; }
        public Dictionary<string, TableTextData> TableTextData_Dictionary { get; set; }
        //public Dictionary<int, TableLevelOrderData> TableLevelOrderData_Dictionary { get; set; }
        public Dictionary<int, TableAnswerInfoData> TableAnswerInfoData_Dictionary { get; set; }
        public Dictionary<int, TableItemData> TableItemData_Dictionary { get; set; }
        public Dictionary<int, TableOfficialInfoData> TableOfficialInfoData_Dictionary { get; set; }
        public Dictionary<int, TableRankIconData> TableRankIconData_Dictionary { get; set; }
        public Dictionary<int, TableDailyTaskData> TableDailyTaskData_Dictionary { get; set; }
        public Dictionary<int, TableGuideData> TableGuideData_Dictionary { get; set; }
        public Dictionary<int, TableBuildAppData> TableBuildAppData_Dictionary { get; set; }
        public Dictionary<int, TableEcpmData> TableEcpmData_Dictionary { get; set; }
        public Dictionary<int, TableActivityData> TableActivityData_Dictionary { get; set; }
        public Dictionary<int, TableActivitySignData> TableActivitySignData_Dictionary { get; set; }
        public Dictionary<int, TableNewbieSignData> TableNewbieSignData_Dictionary { get; set; }
        public Dictionary<int, TableIdiomInfoData> TableIdiomInfoData_Dictionary { get; set; }
        public Dictionary<int, TableAncientPoetryData> TableAncientPoetryData_Dictionary { get; set; }

        #region 单例

        private static DataModuleManager instance = null;

        private DataModuleManager()
        {
        }

        public static DataModuleManager _instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataModuleManager();
                    instance.LoadAllDataModule();
                }

                return instance;
            }
        }

        #endregion
    }
}
