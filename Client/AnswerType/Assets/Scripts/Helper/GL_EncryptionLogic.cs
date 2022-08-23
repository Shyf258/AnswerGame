//2021.12.14    关林
//加密逻辑

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
namespace Encryption
{
    public class GL_EncryptionLogic
    {
        //Rsa的公钥
        private const string publicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAll6MxG7bWakSq9pJ/6+JFPmIMD+mPSeDvidOlLIRNmFFxojXSlVYWnFcbWnqS0WEFuU/FL9YrdHp/2t2+twriKiYnzY1qsqPfE68rsmjLrjX3AvtcQ7Mhn4qHfCh7bUzdbkgyAqnjI3rJc7IrQ3GUi6PZAMW8+33B2Fn3YgMSFDqXSl7REbujZ71DTMfMjtqQ/DyaYR//wllDbiNvGFx+ToEG2eNDUC199XeXEsy3RapnFV8ifc6EvQMeeqtRxPh8yqRY2HSbhDjKZEsH4NF6HeAb3B0WfIws5OOHiTaqVuLcTTbFg48fMvuVb3PpRqb79du2O8le+frx7ruJtY7+QIDAQAB";

        // GTM时区的时间经过MD5加密后的字符串
        public string authIV = "";
        // 随机出来的32字节的字符串
        public string authKey = "";

        public GL_EncryptionLogic()
        {
            //根据时间计算authIV . 16位
            DateTime dt = DateTime.UtcNow;
            string str = string.Format("{0:u}", dt);
            str = GL_Tools.GetMd5String(str);
            authIV = str.Substring(8, 16);

            //计算随机的 authKey
            authKey = GL_Tools.GetRandomStr(32, true, true, true, true);
        }

        #region RSA. 跟进公钥计算私钥
        public string GetAuthorization()
        {
            string encKey = RSA_GetKey(publicKey);

            //101是 公钥的版本 暂时写死
            string authorization = GetAppendStr("101", ":", encKey);
            return authorization;
        }

        private string GetAuthIV_AuthKey()
        {
            string authIV_authKey = GetAppendStr(authIV, ":", authKey);
            return authIV_authKey;
        }

        private string RSA_GetKey(string publicKey)
        {
            byte[] b = Encoding.UTF8.GetBytes(publicKey);
            //b = Convert.base64
            byte[] encryptByte = RSAHelper.EncryptForSpilt(Encoding.UTF8.GetBytes(GetAuthIV_AuthKey()), Encoding.UTF8.GetBytes(publicKey));
            // 为了方便观察吧加密后的数据用base64加密转一下，要不然看起来是乱码,所以解密是也是要用Base64先转换
            string afterEncrypt = Convert.ToBase64String(encryptByte);
            //string afterEncrypt = RSAHelper.RSAEncrypt(GetAuthIV_AuthKey(), publicKey);
            return afterEncrypt;
        }

        #endregion

        #region 消息包加密 解密

        public string AesEncrypt(string json)
        {
            return AESHelper.Encrypt(authKey, authIV, json);
        }

        public string AesDecrypt(string json)
        {
            return AESHelper.Decrypt(authKey, authIV, json);
        }

        #endregion


        private string GetAppendStr(string params1, string params2, string params3)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(params1);
            sb.Append(params2);
            sb.Append(params3);
            return sb.ToString();
        }
    }



    public static class RSAHelper
    {
        public static string RSA = "RSA";// 非对称加密密钥算法
        public static string ECB_PKCS1_PADDING = "RSA/None/PKCS1Padding";//加密填充方式
        public static int DEFAULT_KEY_SIZE = 2048;//秘钥默认长度
        public static string DEFAULT_SPLIT = "#PART#";    // 当要加密的内容超过bufferSize，则采用partSplit进行分块加密
        public static int DEFAULT_BUFFERSIZE = (DEFAULT_KEY_SIZE / 8) - 11;// 当前秘钥支持加密的最大字节数

        /// <summary>
        /// 使用公钥加密
        /// </summary>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] text, byte[] publicKey)
        {
            byte[] byteEncrypt;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportCspBlob(publicKey);
                //byte[] byteText = Encoding.UTF8.GetBytes();
                byteEncrypt = rsa.Encrypt(text, false);
            }
            return byteEncrypt;
        }
        public static string RSAEncrypt(string content, string publickey)
        {
            //publickey = @"<RSAKeyValue><Modulus>5m9m14XH3oqLJ8bNGw9e4rGpXpcktv9MSkHSVFVMjHbfv+SJ5v0ubqQxa5YjLN4vc49z7SVju8s0X4gZ6AzZTn06jzWOgyPRV54Q4I0DCYadWW4Ze3e+BOtwgVU1Og3qHKn8vygoj40J6U85Z/PTJu3hN1m75Zr195ju7g9v4Hk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publickey);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);

            return Convert.ToBase64String(cipherbytes);
        }
        /// <summary>
        /// 私钥解密
        /// </summary>
        /// <param name="strEntryText"></param>
        /// <param name="strPrivateKey"></param>
        /// <returns></returns>
        public static string Decrypt(string encryptText, string privateKey)
        {
            byte[] byteText;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportCspBlob(Convert.FromBase64String(privateKey));
                byte[] byteEncrypt = Convert.FromBase64String(encryptText);
                byteText = rsa.Decrypt(byteEncrypt, false);
            }
            return Encoding.UTF8.GetString(byteText);
        }

        /// <summary>
        /// 获取公钥和私钥
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetKey()
        {
            Dictionary<string, string> dicKey = new Dictionary<string, string>();
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                string public_Key = Convert.ToBase64String(rsa.ExportCspBlob(false));
                string private_Key = Convert.ToBase64String(rsa.ExportCspBlob(true));
                dicKey.Add("PublicKey", public_Key);
                dicKey.Add("PrivateKey", private_Key);
            }
            return dicKey;
        }

        public static byte[] EncryptForSpilt(byte[] data, byte[] publicKey)
        {
            int dataLen = data.Length;
            if (dataLen <= DEFAULT_BUFFERSIZE)
            {
                return Encrypt(data, publicKey);
            }
            List<Byte> allBytes = new List<Byte>();
            int bufIndex = 0;
            int subDataLoop = 0;
            byte[] buf = new byte[DEFAULT_BUFFERSIZE];
            for (int i = 0; i < dataLen; i++)
            {
                buf[bufIndex] = data[i];
                if (++bufIndex == DEFAULT_BUFFERSIZE || i == dataLen - 1)
                {
                    subDataLoop++;
                    if (subDataLoop != 1)
                    {
                        foreach (byte b in Encoding.UTF8.GetBytes(DEFAULT_SPLIT))
                        {
                            allBytes.Add(b);
                        }
                    }
                    byte[] encryptBytes = Encrypt(buf, publicKey);
                    foreach (var b in encryptBytes)
                    {
                        allBytes.Add(b);
                    }
                    bufIndex = 0;
                    if (i == dataLen - 1)
                    {
                        buf = null;
                    }
                    else
                    {
                        buf = new byte[Math.Min(DEFAULT_BUFFERSIZE, dataLen - i - 1)];
                    }
                }
            }
            byte[] bytes = new byte[allBytes.Count];
            {
                int i = 0;
                foreach (var b in allBytes)
                {
                    bytes[i++] = b;
                }
            }
            return bytes;
        }
    }

    public static class AESHelper
    {
        //为什么要用base64,因为得到的密文是byte[]，所以默认用base64转成str方便查看
        // AES 加密的初始化向量,加密解密需设置相同的值。需要是16字节
        //public static byte[] AES_IV = Encoding.UTF8.GetBytes("1234567890123458");

        /// <summary>
        ///  加密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="data">要被加密的明文</param>
        /// <returns>密文base64</returns>
        public static string Encrypt(string key, string iv, string data)
        {
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(data);
                        }
                        byte[] bytes = msEncrypt.ToArray();
                        return Convert.ToBase64String(bytes);
                    }
                }
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="encryptData">密文</param>
        /// <returns>明文</returns>
        public static string Decrypt(string key, string iv, string encryptData)
        {
            byte[] inputBytes = Convert.FromBase64String(encryptData);
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream(inputBytes))
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srEncrypt = new StreamReader(csEncrypt))
                        {
                            return srEncrypt.ReadToEnd();
                        }
                    }
                }
            }
        }


    }
}

