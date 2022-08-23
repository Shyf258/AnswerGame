// FileName: TableFamilyData.cs
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
	public class TableFamilyData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// ID
		/// </summary>
		public int ID;
		/// <summary>
		/// 人名
		/// </summary>
		public string Name;
		/// <summary>
		/// 交谈内容
		/// </summary>
		public string ChatContent;
		/// <summary>
		/// 展现类型
		/// </summary>
		public int Type;
		/// <summary>
		/// 等待时间(秒)
		/// </summary>
		public int WaitTime;
		#endregion

		#region 扩展
		public static Dictionary<int, TableFamilyData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableFamilyData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableFamilyData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableFamily").text);
			}
			return dic;
		}

	}
}
