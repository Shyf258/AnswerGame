// FileName: TableGuideData.cs
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
	public class TableGuideData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// 
		/// </summary>
		public int ID;
		/// <summary>
		/// 触发条件
		/// </summary>
		public int Group;
		/// <summary>
		/// 
		/// </summary>
		public int UnlockLevel;
		/// <summary>
		/// 
		/// </summary>
		public int PreGroup;
		/// <summary>
		/// 
		/// </summary>
		public bool ForceNext;
		/// <summary>
		/// 
		/// </summary>
		public int TriggerType;
		/// <summary>
		/// 
		/// </summary>
		public bool IsForce;
		/// <summary>
		/// 引导表现
		/// </summary>
		public int GuideType;
		/// <summary>
		/// 
		/// </summary>
		public bool IsShowMask;
		/// <summary>
		/// 
		/// </summary>
		public bool IsClickOtherClose;
		/// <summary>
		/// 
		/// </summary>
		public bool IsShowFinger;
		/// <summary>
		/// 
		/// </summary>
		public List<int> FingerOffset;
		/// <summary>
		/// 
		/// </summary>
		public bool IsShowDialog;
		/// <summary>
		/// 
		/// </summary>
		public float DialogOffset;
		/// <summary>
		/// 
		/// </summary>
		public string Dialog;
		/// <summary>
		/// 
		/// </summary>
		public string ObjectName;
		/// <summary>
		/// 
		/// </summary>
		public List<string> PreFunc;
		/// <summary>
		/// 
		/// </summary>
		public List<string> AfterFunc;
		#endregion

		#region 扩展
		public static Dictionary<int, TableGuideData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableGuideData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableGuideData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableGuide").text);
			}
			return dic;
		}

	}
}
