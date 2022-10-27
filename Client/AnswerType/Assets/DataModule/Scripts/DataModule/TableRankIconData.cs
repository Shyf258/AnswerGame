// FileName: TableRankIconData.cs
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
	public class TableRankIconData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// ID
		/// </summary>
		public int ID;
		/// <summary>
		/// 路径
		/// </summary>
		public string IconPath;
		#endregion

		#region 扩展
		public static Dictionary<int, TableRankIconData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableRankIconData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableRankIconData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableRankIcon").text);
			}
			return dic;
		}

	}
}
