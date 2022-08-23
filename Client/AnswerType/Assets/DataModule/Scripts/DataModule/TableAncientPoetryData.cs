// FileName: TableAncientPoetryData.cs
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
	public class TableAncientPoetryData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// ID
		/// </summary>
		public int ID;
		/// <summary>
		/// 古诗名称
		/// </summary>
		public string Name;
		/// <summary>
		/// 作者
		/// </summary>
		public string Author;
		/// <summary>
		/// 第一句
		/// </summary>
		public string TheFirst;
		/// <summary>
		/// 第二句
		/// </summary>
		public string TheSecond;
		/// <summary>
		/// 第三句
		/// </summary>
		public string TheThird;
		/// <summary>
		/// 第四句
		/// </summary>
		public string TheFourth;
		#endregion

		#region 扩展
		public static Dictionary<int, TableAncientPoetryData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableAncientPoetryData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableAncientPoetryData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableAncientPoetry").text);
			}
			return dic;
		}

	}
}
