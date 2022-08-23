// FileName: TableActivitySignData.cs
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
	public class TableActivitySignData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// ID
		/// </summary>
		public int ID;
		/// <summary>
		/// 奖励
		/// </summary>
		public int Reward;
		#endregion

		#region 扩展
		public static Dictionary<int, TableActivitySignData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableActivitySignData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableActivitySignData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableActivitySign").text);
			}
			return dic;
		}

	}
}
