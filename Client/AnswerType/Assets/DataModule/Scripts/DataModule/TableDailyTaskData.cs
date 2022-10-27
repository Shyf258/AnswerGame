// FileName: TableDailyTaskData.cs
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
	public class TableDailyTaskData : DataModuleBase 
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
		/// 完成提示
		/// </summary>
		public string FinishTips;
		#endregion

		#region 扩展
		public static Dictionary<int, TableDailyTaskData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableDailyTaskData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableDailyTaskData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableDailyTask").text);
			}
			return dic;
		}

	}
}
