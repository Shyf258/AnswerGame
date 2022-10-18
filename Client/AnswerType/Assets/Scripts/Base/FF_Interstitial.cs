using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_Interstitial : Mono_Singleton_DontDestroyOnLoad<FF_Interstitial>
{

   public int _onclickTimes;
   
   public void Init()
   {
      _onclickTimes = 0;
   }

   public void DoUpdate()
   {

      if (GL_Game._instance.GameState == EGameState.GameMain 
          && GL_PlayerData._instance._PlayerCostState._costState == CostState.Low)
      {
         InputUp();
      }
      
   }


   private void InputUp()
   {
      if ( GL_Input._instance.Sweep())
      {
         DDebug.LogError("抬起点击");
         _onclickTimes++;

         if (_onclickTimes>=GL_ConstData.Interstitial)
         {
            DDebug.LogError("*****  播放低价值被动插屏，当前点击累计次数：" + _onclickTimes);
            _onclickTimes = 0;
            //播插屏
            GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Interstitial_AllDialog);
         }
         
      }
   }

   public void Minus()
   {
      if (_onclickTimes>=GL_ConstData.Interstitial-1)
      {
        _onclickTimes--;
      }
   }

}
/// <summary>
/// 用户价值
/// </summary>
public class PlayerCostState
{
   public CostState _costState;
}

public enum  CostState
{
   /// <summary>/// 普通未分类用户/// </summary>
   Normal = 0 ,
   /// <summary>/// 低价值用户/// </summary>
   Low, 
   /// <summary>/// 中等价值用户/// </summary>
   Middle, 
   /// <summary>/// 高价值用户/// </summary>
   High, 
   
}
