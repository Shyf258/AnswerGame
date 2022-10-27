// FileName: TableLevelOrderData.cs
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
	public class TableLevelOrderData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// ID
		/// </summary>
		public int ID;
		/// <summary>
		/// 主题ID
		/// </summary>
		public int Level;
		#endregion

		#region 扩展
		public static Dictionary<int, TableLevelOrderData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableLevelOrderData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableLevelOrderData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableLevelOrder").text);
			}
			return dic;
		}

	}
}
