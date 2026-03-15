using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Laoyoutiao.Common
{
	/// <summary>
	/// 加密解密实用类。
	/// </summary>
	public class Encrypt
    {
        private static string key;//密钥
        static Encrypt()
        {
            key = "laoyoutiaosoft2026";
        }
        //密钥
        private static byte[] arrDESKey = new byte[] {42, 16, 93, 156, 78, 4, 218, 32};
		private static byte[] arrDESIV = new byte[] {55, 103, 246, 79, 36, 99, 167, 3};
        //private static byte[] arrDESIV = new byte[] { 0x12, 0x34, 0x56, 120, 0x90, 0xab, 0xcd, 0xef };//8个bit位，是DES算法的初始化向量  加解密钥也是8位;
        /// <summary>
        /// 加密。
        /// </summary>
        /// <param name="m_Need_Encode_String"></param>
        /// <returns></returns>
        public static string Encode(string m_Need_Encode_String)
		{
			if (m_Need_Encode_String == null)
			{
				throw new Exception("Error: \n源字符串为空！！");
			}

            using var objDES = DES.Create();
            using var objMemoryStream = new MemoryStream();
            using var objCryptoStream = new CryptoStream(objMemoryStream, objDES.CreateEncryptor(arrDESKey, arrDESIV), CryptoStreamMode.Write);
            using var objStreamWriter = new StreamWriter(objCryptoStream);
			objStreamWriter.Write(m_Need_Encode_String);
			objStreamWriter.Flush();
			//objCryptoStream.FlushFinalBlock();
			objMemoryStream.Flush();
			return Convert.ToBase64String(objMemoryStream.GetBuffer(), 0, (int)objMemoryStream.Length);
		}

        /// <summary>
        /// 解密。
        /// </summary>
        /// <param name="m_Need_Encode_String"></param>
        /// <returns></returns>
        //public static string Decode(string m_Need_Encode_String)
        //{
        //	if (m_Need_Encode_String == null)
        //	{
        //		throw new Exception("Error: \n源字符串为空！！");
        //	}
        //	DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();  

        //          byte[] arrInput = Convert.FromBase64String(m_Need_Encode_String);
        //	MemoryStream objMemoryStream = new MemoryStream(arrInput);
        //	CryptoStream objCryptoStream = new CryptoStream(objMemoryStream,objDES.CreateDecryptor(arrDESKey,arrDESIV),CryptoStreamMode.Read);
        //	StreamReader  objStreamReader  = new StreamReader(objCryptoStream);
        //	return objStreamReader.ReadToEnd();
        //}

        /// <summary>
        /// 解密。
        /// </summary>
        /// <param name="m_Need_Encode_String"></param>
        /// <returns></returns>
        public static string Decode(string m_Need_Encode_String)
        {
            if (m_Need_Encode_String == null)
            {
                throw new Exception("Error: \n源字符串为空！！");
            }

            byte[] arrInput = Convert.FromBase64String(m_Need_Encode_String);

            using var objDES = DES.Create();
            using var objMemoryStream = new MemoryStream(arrInput);
            using var objCryptoStream = new CryptoStream(objMemoryStream, objDES.CreateDecryptor(arrDESKey, arrDESIV), CryptoStreamMode.Read);
            using var objStreamReader = new StreamReader(objCryptoStream);
            return objStreamReader.ReadToEnd();
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="strText">要加密字符串</param>
        /// <param name="IsLower">是否以小写方式返回</param>
        /// <returns></returns>
        public static string MD5Encrypt(string strText, bool IsLower)
        {
            using var md5 = MD5.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(strText);
            bytes = md5.ComputeHash(bytes);

            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }

            return ret.PadLeft(32, '0');
        }

        //public static string MD5Encrypt(string strText, bool IsLower)
        //{
        //    MD5 md5 = new MD5CryptoServiceProvider();
        //    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strText);
        //    bytes = md5.ComputeHash(bytes);
        //    md5.Clear();

        //    string ret = "";
        //    for (int i = 0; i < bytes.Length; i++)
        //    {
        //        ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
        //    }

        //    return ret.PadLeft(32, '0');
        //}
	}
}
