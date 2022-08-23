// FileName: TableItemData.cs
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
	public class TableItemData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// 音效ID
		/// </summary>
		public int ID;
		/// <summary>
		/// 类型
		/// </summary>
		public int ItemType;
		/// <summary>
		/// 路径
		/// </summary>
		public string IconPath;
		#endregion

		#region 扩展
		public static Dictionary<int, TableItemData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableItemData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableItemData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableItem").text);
			}
			return dic;
		}

	}
}
