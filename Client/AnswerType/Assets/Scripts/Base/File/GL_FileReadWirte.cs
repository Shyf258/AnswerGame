//2018.09.04    关林
//文件读写

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GL_FileReadWirte
{
    public string _filePath { get; private set; }

    public void Init(string path)
    {
        _filePath = Application.persistentDataPath + "/" + path;
    }

    public void Wirte(object obj)
    {
        // 创建一个文件"data.xml"并将对象序列化后存储在其中
        //Debug.LogError("~~~写文件");
        Stream stream = File.Open(_filePath, FileMode.Create);
        if (stream == null) return; 
        BinaryFormatter formatter = new BinaryFormatter();

        formatter.Serialize(stream, obj);
        stream.Close();
        //Debug.LogError("~~~关闭文件");
    }

    public object Read()
    {
        object obj = null;
        // 打开文件"data.xml"并进行反序列化得到对象
        Stream stream = File.Open(_filePath, FileMode.OpenOrCreate);
        if (stream == null || stream.Length == 0)
        {
            stream.Close();
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();

        try
        {
            obj = formatter.Deserialize(stream);
            stream.Close();
            return obj;
        }
        catch (System.Exception e)
        {
            DDebug.LogError("~~~读取文件:" + e);
            stream.Close();
            return null;
            throw;
        }
        
    }

}
