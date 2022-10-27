// FileName: TableAudioData.cs
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
	public class TableAudioData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// 音效ID
		/// </summary>
		public int ID;
		/// <summary>
		/// 音量
		/// </summary>
		public float Volume;
		/// <summary>
		/// 是否循环
		/// </summary>
		public bool Loop;
		/// <summary>
		/// 路径
		/// </summary>
		public string Path;
		#endregion

		#region 扩展
		public static Dictionary<int, TableAudioData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableAudioData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableAudioData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableAudio").text);
			}
			return dic;
		}

	}
}
