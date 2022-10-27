/***
 * 
 *    Title: "SUIFW" UI框架项目
 *           主题： 通用配置管理器接口   
 *    Description: 
 *           功能： 
 *                基于“键值对”配置文件的通用解析
 *                  
 *    Date: 2017
 *    Version: 0.1版本
 *    Modify Recoder: 
 *    
 *   
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SUIFW
{
	public interface IJsonManager  {

        /// <summary>
        /// 只读属性： 应用设置
        /// 功能： 得到键值对集合数据
        /// </summary>
	    //Dictionary<string, string> AppSetting { get; }

        /// <summary>
        /// 得到配置文件（AppSeting）最大的数量
        /// </summary>
        /// <returns></returns>
	    int GetAppSettingMaxNumber();

        void InitAndAnalysisJson(string jsonPath);

    }

    [Serializable]
    internal class KeyValuesInfo
    {
        //配置信息
        public List<KeyValuesNode> ConfigInfo = null;
    }

    [Serializable]
    internal class KeyValuesNode
    {
        //键
        public string Key = null;
        //值
        public string Value = null;
    }

    [Serializable]
    internal class LanguageDataInfo
    {
        //配置信息
        public List<LanguageData> Info = null;
    }

    [Serializable]
    public class LanguageData
    {
        public int ID;
        /// <summary>
        /// 英文
        /// </summary>
        public string EN = null;
        /// <summary>
		/// 简体中文
		/// </summary>
        public string CN = null;
        /// <summary>
        /// 繁体中文
        /// </summary>
        public string HK = null;
        /// <summary>
		/// 法语
		/// </summary>
        public string FR = null;
        /// <summary>
        /// 德语
        /// </summary>
        public string DE = null;
        /// <summary>
        /// 意大利语
        /// </summary>
        public string IT = null;
        /// <summary>
        /// 日语
        /// </summary>
        public string JA = null;
        /// <summary>
        /// 葡萄牙语
        /// </summary>
        public string PT = null;
        /// <summary>
        /// 西班牙语
        /// </summary>
        public string ES = null;
        /// <summary>
        /// 韩语
        /// </summary>
        public string KO = null;
    }

    public interface IScrollPage
    {
        void OnPageParentOnBeginDrag(PointerEventData eventData);
        void OnPageParentOnEndDrag(PointerEventData eventData);
        void OnPageParentOnDrag(PointerEventData eventData);
    }
}