// FileName: TableNetworkRequestData.cs
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
	public class TableNetworkRequestData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// 协议ID号
		/// </summary>
		public int ID;
		/// <summary>
		/// 内容介绍
		/// </summary>
		public string Name;
		/// <summary>
		/// 协议属性
		/// </summary>
		public string Method;
		/// <summary>
		/// 前缀url
		/// </summary>
		public int URL;
		/// <summary>
		/// 协议url
		/// </summary>
		public string JointURL;
		/// <summary>
		/// 是否验证SessionID
		/// </summary>
		public bool IsCheckSM;
		/// <summary>
		/// 数盟业务编码
		/// </summary>
		public string SMeventCode;
		/// <summary>
		/// 数盟自定义小心
		/// </summary>
		public string SMoptMsg;
		#endregion

		#region 扩展
		public static Dictionary<int, TableNetworkRequestData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableNetworkRequestData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableNetworkRequestData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableNetworkRequest").text);
			}
			return dic;
		}

	}
}
