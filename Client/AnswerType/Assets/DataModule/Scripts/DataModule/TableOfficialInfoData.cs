// FileName: TableOfficialInfoData.cs
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
	public class TableOfficialInfoData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// ID
		/// </summary>
		public int ID;
		/// <summary>
		/// 官位
		/// </summary>
		public string Official;
		/// <summary>
		/// 皮肤
		/// </summary>
		public string Skin;
		/// <summary>
		/// 房子
		/// </summary>
		public string HouseName;
		/// <summary>
		/// 房子资源
		/// </summary>
		public string HousePath;
		/// <summary>
		/// 所需关卡数量
		/// </summary>
		public int LevelIndex;
		#endregion

		#region 扩展
		public static Dictionary<int, TableOfficialInfoData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableOfficialInfoData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableOfficialInfoData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableOfficialInfo").text);
			}
			return dic;
		}

	}
}
