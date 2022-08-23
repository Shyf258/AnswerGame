// FileName: TableIdiomInfoData.cs
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
	public class TableIdiomInfoData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// ID
		/// </summary>
		public int ID;
		/// <summary>
		/// 成语内容
		/// </summary>
		public string Text;
		/// <summary>
		/// 拼音
		/// </summary>
		public string PinYin;
		/// <summary>
		/// 解释
		/// </summary>
		public string Explain;
		#endregion

		#region 扩展
		public static Dictionary<int, TableIdiomInfoData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableIdiomInfoData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableIdiomInfoData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableIdiomInfo").text);
			}
			return dic;
		}

	}
}
