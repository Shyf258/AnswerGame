// FileName: TableActivityData.cs
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
	public class TableActivityData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// ID
		/// </summary>
		public int ID;
		/// <summary>
		/// 任务描述
		/// </summary>
		public string Description;
		/// <summary>
		/// 
		/// </summary>
		public string SubDescription;
		/// <summary>
		/// 图标名称
		/// </summary>
		public string IconName;
		#endregion

		#region 扩展
		public static Dictionary<int, TableActivityData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableActivityData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableActivityData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableActivity").text);
			}
			return dic;
		}

	}
}
