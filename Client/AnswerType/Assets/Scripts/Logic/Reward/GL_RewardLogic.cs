//2018.11.28    关林
//奖励逻辑解析

using System;
using System.Collections;
using System.Collections.Generic;
using DataModule;
using Logic.Fly;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;

public class GL_RewardLogic : Singleton<GL_RewardLogic>
{
    public void RewardSettlement(SRewardSimpleData sd)
    {
        RewardSettlement((EItemType)sd._itemTableID, sd._num, 1);
    }

    //public void RewardSettlement(SRewardSimpleData sd, ERewardSource source)
    //{
    //    SRewardData reward = new SRewardData(sd._itemTableID, source);
    //    RewardSettlement(sd._itemTableID, sd._num, 1);
    //}

    public void RewardSettlement(SRewardData data, int count, ERewardSource source = ERewardSource.None)
    {
        RewardSettlement(data._rewardType, count, 1, source);
    }
    public void RewardSettlement(int tableID, int count, int rate = 1, ERewardSource source = ERewardSource.None)
    {
        RewardSettlement((EItemType)tableID, count, rate);
    }
    public void RewardSettlement(EItemType type, int count, int rate = 1, ERewardSource source = ERewardSource.None)
    {
        //实际结算逻辑

        switch (type)
        {
            case EItemType.Coin:
                GL_PlayerData._instance.Coin += count;
                break;
            case EItemType.Bogus:
                GL_PlayerData._instance.Bogus += count;
                break;
            case EItemType.Strength:
                GL_PlayerData._instance.Strength += count;
                break;
            default:
                break;
        }
        
    }
    
    public Sprite GetItemSprite(EItemType type)
    {
        var data = GameDataTable.GetTableItemTypeData((int)type);
        return GL_SpriteAtlasPool._instance.GetSprite("Sprites", data.IconPath);
    }
    
    public string GetItemNumber(EItemType type, int num)
    {
        string result = "";
        switch (type)
        {
            case EItemType.None:
            case EItemType.Coin:
            case EItemType.Strength:
            default:
                result = num.ToString();
                break;
            case EItemType.RMB:
            case EItemType.Bogus:
                result = num * 0.01f + "元";
                break;
        }

        return result;
    }
    
    //通用奖励获取
    public void GetReward(List<Rewards> rewardsList,bool isActiveMainUp)
    {
        foreach (var reward in rewardsList)
        {
            switch ((EItemType)reward.type)
            {
                case EItemType.Coin:
                    if (reward.num == 0)
                        break;
                    SRewardData rewardData1 = new SRewardData(EItemType.Coin);
                    RewardSettlement(rewardData1, reward.num);
                    Fly_Manager._instance.MainUpFly(EFlyItemType.Coin, Vector3.zero,isActiveMainUp);
                    UI_HintMessage._.ShowMessage($"恭喜您，领取{reward.num}金币！");
                    break;
                case EItemType.Bogus:
                    if (reward.num == 0)
                        break;
                    SRewardData rewardData2 = new SRewardData(EItemType.Bogus);
                    RewardSettlement(rewardData2, reward.num);
                    Fly_Manager._instance.MainUpFly(EFlyItemType.Bogus,Vector3.zero,isActiveMainUp);
                    UI_HintMessage._.ShowMessage($"恭喜您，领取{(reward.num /100f) :0.00}元红包！");
                    break;
            }
        }
    }
    
}

//奖励的数据结构
public class SRewardData
{
    public EItemType _rewardType; //奖励类型
    //public Sprite _rewardSprite;    //奖励的图标
    public string _rewardDes;        //奖励描述
    public ERewardSource _rewardSource; //奖励来源

    public SRewardData(EItemType type)
    {
        var data = GameDataTable.GetTableItemTypeData((int)type);
        Init((EItemType)data.ItemType, data.IconPath);
    }
    public SRewardData(EItemType type, ERewardSource source)
    {
        var data = GameDataTable.GetTableItemTypeData((int)type);
        Init((EItemType)data.ItemType, data.IconPath);
        _rewardSource = source;
    }
    //public SRewardData(int tableID)
    //{
    //    var data = GameDataTable.GetTableItemData(tableID);
    //    Init((EItemType)data.ID, data.IconPath);
    //}
    //public SRewardData(int tableID, ERewardSource source)
    //{
    //    var data = GameDataTable.GetTableItemData(tableID);
    //    Init((EItemType)data.ID, data.IconPath);
    //    _rewardSource = source;
    //}

    private void Init(EItemType type, string path)
    {
        _rewardType = type;
        //_rewardSprite = GL_SpriteAtlasPool._instance.GetSprite("PopIcon", path);
    }
}


//奖励数据的简单结构
public class SRewardSimpleData
{
    public int _itemTableID;    //item表格ID
    public int _num;            //数量
}
