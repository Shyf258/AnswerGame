using System.Collections;
using System.Collections.Generic;
using SUIFW;
using UnityEngine;
using UnityEngine.UI;

public class LevelItem : UI_BaseItem
{
    public uint _level;
    private Text _levelCount;

    private void Awake()
    {
        
    }

    public override void Init()
    {
        base.Init();
        _levelCount = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "LevelCount");
    }

    public void To(uint Form,uint To)
    {
        _level = To;
        if (_levelCount != null)
        {
            _levelCount.text = To.ToString();
        }
        else
        {
            _levelCount = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "LevelCount");
            _levelCount.text = To.ToString();
        }
        this.gameObject.name = To.ToString();
    }
}
