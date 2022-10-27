//2021.8.13 关林
//图集池

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class GL_SpriteAtlasPool : Singleton<GL_SpriteAtlasPool>
{
    private Dictionary<string, SpriteAtlas> _spriteAtlasPool = new Dictionary<string, SpriteAtlas>();

    //通过图集名,  图片名, 加载图片
    public Sprite GetSprite(string spriteAtlasName, string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName))
            return null;
        if(!_spriteAtlasPool.ContainsKey(spriteAtlasName))
        {
            //加载图集
            string path = "SpriteAtlas/" + spriteAtlasName;
            SpriteAtlas sa = Resources.Load(path) as SpriteAtlas;
            if(sa == null)
            {
                DDebug.LogError("当前图集未找到: " + spriteAtlasName + "   : " + spriteName);
                return null;
            }
            else
            {
                _spriteAtlasPool.Add(spriteAtlasName, sa);
            }
        }
        if (_spriteAtlasPool[spriteAtlasName] == null)
            return null;

        return _spriteAtlasPool[spriteAtlasName].GetSprite(spriteName);
    }


    public void Clear()
    {
        if (_spriteAtlasPool.Count == 0)
            return;
        //foreach (var sprite in _spriteAtlasPool.Values)
        //{
        //    GameObject.Destroy(sprite);
        //}
        _spriteAtlasPool.Clear();
    }
}
