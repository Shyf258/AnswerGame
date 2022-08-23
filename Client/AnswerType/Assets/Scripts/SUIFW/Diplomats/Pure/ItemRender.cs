using System;
using System.Collections;
using System.Collections.Generic;
using SUIFW;
using UnityEngine.UI;

public class ItemRender : DynamicInfinityItem
{
    private Text _idiomText;

    private Button _btn;

    private UI_IF_PureGame _uiIfPureGame;
	// Use this for initialization
	void Start ()
	{

		_idiomText = transform.Find("IdiomText").GetComponent<Text>();
		_btn = _idiomText.transform.GetComponent<UI_Button>();
		
		_btn.onClick.AddListener(() =>
		{
			Click();
		});
	}

    protected override void OnRenderer()
    {
        base.OnRenderer();
        try
        {
	        _idiomText.text = "第"+mData+"题";
        }
        catch 
        {
	        _idiomText = transform.Find("IdiomText").GetComponent<Text>();
	        _idiomText.text = "第"+mData+"题";
        }
        
    }

    private void Click()
    {
	 
	    GL_PlayerData._instance.AnswerCount = (int)mData;
	  
	    DDebug.LogError("选择题目 切换界面"+ GL_PlayerData._instance.AnswerCount);
	    // try
	    // {
		   //  _uiIfPureGame.ChangePage("Answer");
	    // }
	    // catch 
	    // {
		   // _uiIfPureGame = UIManager.GetInstance().GetUI(SysDefine.UI_Path_PureGame) as UI_IF_PureGame;
		   // _uiIfPureGame.ChangePage("Task");
	    // }
	   
    }
}
