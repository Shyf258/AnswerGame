#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：StringHelper
// 创 建 者：Yangsong
// 创建时间：2021年12月02日 星期四 10:48
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;

namespace Helpser
{
    /// <summary>
    /// 字符串帮助类
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// 根据所需字符长度获取字符
        /// </summary>
        /// <param name="originStr">原有字符</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static String GetSubStr(string originStr,int length)
        {
            if (originStr.Length <= length)
            {
                return originStr;
            }
            
            String result = originStr.Substring(0, length);
            
            if (float.Parse(result) <= 0)
                result = "0";

            return result;
        }
        
    }
}