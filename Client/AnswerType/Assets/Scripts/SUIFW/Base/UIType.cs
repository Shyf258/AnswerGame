/***
 * 
 *    Title: "SUIFW" UI框架项目
 *           主题： 窗体类型   
 *    Description: 
 *           功能： 
 *                  
 *    Date: 2017
 *    Version: 0.1版本
 *    Modify Recoder: 
 *    
 *   
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SUIFW
{
    [System.Serializable]
	public class UIType {
        //是否清空“栈集合”
	    public bool IsClearStack = false;

	    //是否清空“栈集合”
	    public bool IsClearStackOnHide = false;
        //UI窗体（位置）类型
	    public UIFormType UIForms_Type = UIFormType.Normal;
        //UI窗体显示类型
	    public UIFormShowMode UIForms_ShowMode = UIFormShowMode.Normal;
        //UI窗体透明度类型
        //只对窗体显示类型为 弹出窗体 的生效
	    public UIFormLucenyType UIForm_LucencyType = UIFormLucenyType.Lucency;

	}
}