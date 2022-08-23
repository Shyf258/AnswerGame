// FileName: TableEffectData.cs
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
	public class TableEffectData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// 特效ID
		/// </summary>
		public int ID;
		/// <summary>
		/// 1
		/// </summary>
		public float Life;
		/// <summary>
		/// 路径
		/// </summary>
		public string Path;
		#endregion

		#region 扩展
		public static Dictionary<int, TableEffectData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableEffectData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableEffectData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableEffect").text);
			}
			return dic;
		}

	}
}
