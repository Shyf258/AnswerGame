// FileName: TableEcpmData.cs
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
	public class TableEcpmData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// 
		/// </summary>
		public int ID;
		/// <summary>
		/// 当前档,最大ECPM
		/// </summary>
		public int MaxECPM;
		/// <summary>
		/// 插屏播放概率
		/// </summary>
		public float InterstitialProbability;
		#endregion

		#region 扩展
		public static Dictionary<int, TableEcpmData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableEcpmData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableEcpmData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableEcpm").text);
			}
			return dic;
		}

	}
}
