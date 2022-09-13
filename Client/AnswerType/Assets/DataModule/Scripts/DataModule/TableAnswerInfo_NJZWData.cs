// FileName: TableAnswerInfo_NJZWData.cs
//
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using DataModule;


namespace DataModule
{
	public class TableAnswerInfo_NJZWData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// 序号
		/// </summary>
		public int ID;
		/// <summary>
		/// 题目文本
		/// </summary>
		public string TitleText;
		/// <summary>
		/// 选择1
		/// </summary>
		public string Select1;
		/// <summary>
		/// 选择2
		/// </summary>
		public string Select2;
		/// <summary>
		/// 正确答案
		/// </summary>
		public List<int> CorrectAnswer;
		/// <summary>
		/// 图片索引
		/// </summary>
		public string Picture;
		/// <summary>
		/// 语音播报
		/// </summary>
		public string Audio;
		/// <summary>
		/// 类型
		/// </summary>
		public int Type;
		#endregion

		#region 扩展
		public static Dictionary<int, TableAnswerInfo_NJZWData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableAnswerInfo_NJZWData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableAnswerInfo_NJZWData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableAnswerInfo_NJZW").text);
			}
			return dic;
		}

	}
}
