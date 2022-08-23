// FileName: TableGlobalVariableData.cs
// ExcelTool自动生成于 2021年11月18日  11:38:22
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
	public class TableGlobalVariableData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// 1
		/// </summary>
		public int ID;
		/// <summary>
		/// 默认货币数量
		/// </summary>
		public int MoneyNumber;
		/// <summary>
		/// 插屏间隔时间(秒
		/// </summary>
		public int ADIntervalTime;
		/// <summary>
		/// 默认体力最大值
		/// </summary>
		public int MaxEnergy;
		/// <summary>
		/// 通行证体力最大值
		/// </summary>
		public int PassMaxEnergy;
		/// <summary>
		/// 默认体力时间间隔
		/// </summary>
		public float EnergyDuration;
		/// <summary>
		/// 每次看广告 获得的道具数量
		/// </summary>
		public int GainPropCount;
		/// <summary>
		/// 每天获取体力次数
		/// </summary>
		public int GainEnergyPreDay;
		/// <summary>
		/// 每次获取体力数
		/// </summary>
		public int GainEnergyPreTime;
		/// <summary>
		/// 每天获取金币次数
		/// </summary>
		public int GainGoldPreDay;
		/// <summary>
		/// 每次获取金币数
		/// </summary>
		public int GainGoldPreTime;
		/// <summary>
		/// 每天获取钻石次数
		/// </summary>
		public int GainDiamondPreDay;
		/// <summary>
		/// 每次获取钻石数
		/// </summary>
		public int GainDiamondPreTime;
		/// <summary>
		/// 连击最大时间间隔
		/// </summary>
		public float  ComboTime;
		/// <summary>
		/// 通关奖励翻倍
		/// </summary>
		public float PassLevelRewardRate;
		/// <summary>
		/// VIP无限体力时限(小时)
		/// </summary>
		public float VipFreeEnergyTime;
		/// <summary>
		/// 转盘CD(分钟)
		/// </summary>
		public int RotateDrawCD;
		/// <summary>
		/// 转盘次数上限
		/// </summary>
		public int RotateDrawMaxCount;
		/// <summary>
		/// 金币小游戏次数上限
		/// </summary>
		public int CoinDrawMaxCount;
		/// <summary>
		/// 金币小游戏开启活跃值
		/// </summary>
		public int CoinDrawOpenValue;
		/// <summary>
		/// 金币小游戏倍数
		/// </summary>
		public List<int> CoinDrawMult;
		/// <summary>
		/// 金币小游戏倍数权重
		/// </summary>
		public List<int> CoinDrawMultWeight;
		/// <summary>
		/// 金币小游戏基数
		/// </summary>
		public List<int> CoinDrawBaseValue;
		/// <summary>
		/// 金币小游戏基数权重
		/// </summary>
		public List<int> CoinDrawBaseValueWeight;
		/// <summary>
		/// 第一次复活花费
		/// </summary>
		public int FirstReviveCost;
		/// <summary>
		/// 第二次复活花费
		/// </summary>
		public int SecondReviveCost;
		/// <summary>
		/// 第三次复活花费
		/// </summary>
		public int ThirdReviveCost;
		/// <summary>
		/// 复活增加的额外步数
		/// </summary>
		public int ReviveStepCount;
		/// <summary>
		/// 复活通行证增加的额外步数
		/// </summary>
		public int ReviveStepPassCount;
		/// <summary>
		/// 获得钥匙概率
		/// </summary>
		public float GetKeyProbability;
		/// <summary>
		/// 每局获得钥匙最大数量
		/// </summary>
		public int GetKeyMaxCount;
		/// <summary>
		/// 打开九宫格宝箱需要得钥匙数量
		/// </summary>
		public int NumberKeysNeededChest;
		/// <summary>
		/// 储蓄罐最大容量
		/// </summary>
		public int PiggyBankMaxCapacity;
		/// <summary>
		/// 储蓄罐每关加入金币
		/// </summary>
		public int PiggyBankRewardPreLevel;
		/// <summary>
		/// 掉落物携带礼品卡概率
		/// </summary>
		public List<float> RemindGiftRate;
		/// <summary>
		/// 回满体力
		/// </summary>
		public int FullStrength;
		/// <summary>
		/// 触碰警戒线后复活费用
		/// </summary>
		public int WarningReviveCost;
		/// <summary>
		/// 战前 道具 combo增加数
		/// </summary>
		public int PrePropAddCombo;
		/// <summary>
		/// 战前道具复制器掉落合成元素
		/// </summary>
		public List<int> PrePropCopyMo;
		/// <summary>
		/// 单步分值
		/// </summary>
		public float SingleStepScore;
		/// <summary>
		/// 单次合成额外奖励金币值
		/// </summary>
		public int SingleCompositeGold;
		/// <summary>
		/// 铁锤爆炸范围_盒状X(倍数)
		/// </summary>
		public float HammerBombRange_X;
		/// <summary>
		/// 铁锤爆炸范围_盒状Y(倍数)
		/// </summary>
		public float HammerBombRange_Y;
		/// <summary>
		/// 铁锤爆炸范围_圆形半径(倍数)
		/// </summary>
		public float HammerBombRange_Radius;
		/// <summary>
		/// 引导对话框读字速度(一个字几秒)
		/// </summary>
		public float DialogPrintSpeed;
		/// <summary>
		/// a版本第一关步数
		/// </summary>
		public List<float> UsTotalScoreA;
		/// <summary>
		/// b版本第一关步数
		/// </summary>
		public List<float> UsTotalScoreB;
		/// <summary>
		/// 评价开始关卡
		/// </summary>
		public int CommentBeginLevel;
		/// <summary>
		/// 评价循环关卡数
		/// </summary>
		public int CommentLoopLevel;
		/// <summary>
		/// 评价结束关卡数
		/// </summary>
		public int CommentEndLevel;
		/// <summary>
		/// 加速道具生成所需次数
		/// </summary>
		public int AccPropCreateNumber;
		/// <summary>
		/// 普通关卡增加数量
		/// </summary>
		public int NormalPassKeyAdd;
		/// <summary>
		/// 难度关卡增加数量
		/// </summary>
		public int HardPassKeyAdd;
		/// <summary>
		/// 超难关卡增加数量
		/// </summary>
		public int SuperHardPassKeyAdd;
		/// <summary>
		/// 通行证队伍奖励金币
		/// </summary>
		public int TeamPassReward;
		/// <summary>
		/// 预知分钟数转金币
		/// </summary>
		public int CrystalBallTransformCoin;
		/// <summary>
		/// 开启战前道具送的个数
		/// </summary>
		public int UnLockAccPropGetCount;
		/// <summary>
		/// 技巧大师默认排行榜数量
		/// </summary>
		public int BoosterBattleRankNum;
		/// <summary>
		/// 新手礼包解锁关卡
		/// </summary>
		public List<int> NewUserUnlockLevels;
		/// <summary>
		/// 新手礼包 触发时间
		/// </summary>
		public int NewUserStayTimes;
		/// <summary>
		/// 失败界面 vip和通行证触发轮次
		/// </summary>
		public int ReviveVipAndPassCount;
		/// <summary>
		/// 问答游戏时间长(分钟)
		/// </summary>
		public int QuestionDuration;
		#endregion

		#region 扩展
		public static Dictionary<int, TableGlobalVariableData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableGlobalVariableData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableGlobalVariableData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableGlobalVariable").text);
			}
			return dic;
		}

	}
}
