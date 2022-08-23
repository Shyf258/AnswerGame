// FileName: TableNetworkURLData.cs
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
	public class TableNetworkURLData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// 
		/// </summary>
		public int ID;
		/// <summary>
		/// 测试版本
		/// </summary>
		public string TestURL;
		/// <summary>
		/// 内部测试
		/// </summary>
		public string IntranetTest;
		/// <summary>
		/// 正式-中国
		/// </summary>
		public string URL_CN;
		/// <summary>
		/// 正式-美区
		/// </summary>
		public string URL_USA;
		#endregion

		#region 扩展
		public static Dictionary<int, TableNetworkURLData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableNetworkURLData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableNetworkURLData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableNetworkURL").text);
			}
			return dic;
		}

	}
}
