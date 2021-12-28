using BakaSnowTool.Http;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace TiebaApi.TiebaWebApi
{
    public class Tieba
    {
        /// <summary>
        /// 取安卓Stamp
        /// </summary>
        /// <returns></returns>
        public static string GetAndroidStamp()
        {
            //wappc_1584510405614_799

            Random ra = new Random();
            return $"wappc_{ra.Next(1000, 9999)}{ra.Next(100, 999)}{ra.Next(100, 999)}{ra.Next(100, 999)}_{ra.Next(100, 999)}";
        }

        /// <summary>
        /// 取贴吧Sign
        /// </summary>
        /// <param name="str">文本</param>
        /// <returns></returns>
        public static string GetTiebaSign(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            MD5 algorithm = MD5.Create();
            byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(Http.UrlDecodeUtf8(str.Replace("&", "") + "tiebaclient!!!")));
            algorithm.Dispose();

            string md5 = string.Empty;
            for (int i = 0; i < data.Length; i++)
            {
                md5 += data[i].ToString("x2").ToUpperInvariant();
            }

            return md5;
        }

        /// <summary>
        /// 过滤头像ID
        /// </summary>
        /// <param name="text">头像</param>
        /// <returns></returns>
        public static string GuoLvTouXiangID(string text) 
        {
            if (string.IsNullOrEmpty(text)) return "";

            return new Regex("tb.[a-zA-Z0-9\\._\\-]{30,33}").Match(text).Value;
        }
    }
}
