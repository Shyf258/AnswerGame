// FileName: TableTextData.cs
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
	public class TableTextData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// ID
		/// </summary>
		public string ID;
		/// <summary>
		/// 英文
		/// </summary>
		public string EN;
		/// <summary>
		/// 简体中文
		/// </summary>
		public string CN;
		/// <summary>
		/// 繁體中文
		/// </summary>
		public string HK;
		/// <summary>
		/// 日语
		/// </summary>
		public string JA;
		/// <summary>
		/// 韩语
		/// </summary>
		public string KO;
		/// <summary>
		/// 意大利语
		/// </summary>
		public string IT;
		/// <summary>
		/// 法语
		/// </summary>
		public string FR;
		/// <summary>
		/// 葡萄牙语
		/// </summary>
		public string PT;
		/// <summary>
		/// 西班牙语
		/// </summary>
		public string ES;
		/// <summary>
		/// 德语
		/// </summary>
		public string DE;
		/// <summary>
		/// etc
		/// </summary>
		public string ETC;
		/// <summary>
		/// 解释
		/// </summary>
		public string DESC;
		#endregion

		#region 扩展
		public static Dictionary<string, TableTextData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<string, TableTextData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<string, TableTextData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableText").text);
			}
			return dic;
		}

	}
}
