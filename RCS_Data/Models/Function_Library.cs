using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RCS_Data.Models
{
    public static class Function_Library
    {

        static string key { get { return "aabbccdd"; } }
        static string iv { get { return "eeffgghh"; } }

        /// <summary>
        /// 取得 Class TableName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetClassDisplayName<T>()
        {
            var displayName = typeof(T).GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault() as DisplayNameAttribute;
            if (displayName != null) return displayName.DisplayName;
            return "";
        }
        /// <summary>
        /// 取得現在日期格式
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string getDateNowString(DATE_FORMAT format)
        {
            DateTime dateNow = DateTime.Now;
            return Function_Library.getDateString(dateNow, format); 
        }

        /// <summary>
        /// 取得現在日期格式
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string getDateString(DateTime dateTime,  DATE_FORMAT format)
        { 
            switch (format)
            {
                case DATE_FORMAT.yyyy_MM_dd_125959:
                    return dateTime.ToString("yyyy-MM-dd 23:59:59");
                case DATE_FORMAT.yyyy_MM_dd_HHmmss:
                    return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                case DATE_FORMAT.yyyy_MM_dd_HHmm:
                    return dateTime.ToString("yyyy-MM-dd HH:mm");
                case DATE_FORMAT.HHmm:
                    return dateTime.ToString("HH:mm");
                case DATE_FORMAT.yyyyMMddHHmmssfffff:
                    return dateTime.ToString("yyyyMMddHHmmssfffff");
                case DATE_FORMAT.yyyy_MM_dd:
                    return dateTime.ToString("yyyy-MM-dd");
                case DATE_FORMAT.yyyy_MM_dd_125959_withSlash:
                    return dateTime.ToString("yyyy/MM/dd 23:59:59");
                case DATE_FORMAT.yyyy_MM_dd_HHmmss_withSlash:
                    return dateTime.ToString("yyyy/MM/dd HH:mm:ss");  
                case DATE_FORMAT.yyyy_MM_dd_HHmm_withSlash:
                    return dateTime.ToString("yyyy/MM/dd HH:mm");
                case DATE_FORMAT.yyyy_MM_dd_withSlash:
                    return dateTime.ToString("yyyy/MM/dd");
                case DATE_FORMAT.yyyy_MM_dd_2359:
                    return dateTime.ToString("yyyy-MM-dd 23:59");
                case DATE_FORMAT.yyyy_MM_dd_000000:
                    return dateTime.ToString("yyyy-MM-dd 00:00:00");
                case DATE_FORMAT.yyyy_MM_dd_0000:
                    return dateTime.ToString("yyyy-MM-dd 00:00");
                case DATE_FORMAT.yyyyMMdd:
                    return dateTime.ToString("yyyyMMdd");
                case DATE_FORMAT.HHmmss:
                    return dateTime.ToString("HHmmss");
                case DATE_FORMAT.yyyy_MM_ddTHHmmss:
                    return dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
                default:
                    break;
            }
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// https://stackoverflow.com/questions/30092463/hash-function-to-generate-16-alphanumerical-characters-from-input-string-in-c-sh
        /// 取得長度16的代碼
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string HashString16(string text)
        {
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);

            char[] hash2 = new char[16];

            // Note that here we are wasting bits of hash! 
            // But it isn't really important, because hash.Length == 32
            for (int i = 0; i < hash2.Length; i++)
            {
                hash2[i] = chars[hash[i] % chars.Length];
            }
            return new string(hash2);
        }

        /// <summary>
        ///  加密資料
        /// </summary>
        /// <param name="str"></param> 
        /// <returns></returns>
        public static string getcodeDES(string str )
        {
            return Com.Mayaminer.SecurityTool.EncodeDES(str , Function_Library.key, Function_Library.iv);
        } 

        /// <summary>
        ///  解密資料
        /// </summary>
        /// <param name="str"></param> 
        /// <returns></returns>
        public static string DecodeDES(string hexString)
        {
            return Com.Mayaminer.SecurityTool.DecodeDES(hexString, Function_Library.key, Function_Library.iv);
        }

    }

    public enum DATE_FORMAT
    {
        /// <summary>
        /// ToString("yyyy-MM-dd 23:59:59")
        /// </summary>
        [Description("yyyy-MM-dd 23:59:59")]
        yyyy_MM_dd_125959,
        /// <summary>
        /// ToString("yyyy-MM-dd 00:00:00")
        /// </summary>
        [Description("yyyy-MM-dd 00:00:00")]
        yyyy_MM_dd_000000,
        /// <summary>
        /// ToString("yyyy-MM-dd 00:00")
        /// </summary>
        [Description("yyyy-MM-dd 00:00")]
        yyyy_MM_dd_0000,
        /// <summary>
        /// ToString("yyyy-MM-dd HH:mm:ss")
        /// </summary>
        [Description("yyyy-MM-dd HH:mm:ss")]
        yyyy_MM_dd_HHmmss,
        /// <summary>
        /// ToString("yyyy-MM-dd HH:mm")
        /// </summary>
        [Description("yyyy-MM-ddTHH:mm:ss")]
        yyyy_MM_ddTHHmmss,
        /// <summary>
        /// ToString("yyyy-MM-dd HH:mm")
        /// </summary>
        [Description("yyyy-MM-dd HH:mm")]
        yyyy_MM_dd_HHmm,
        /// <summary>
        /// ToString("yyyy-MM-dd HH:mm")
        /// </summary>
        [Description("yyyy-MM-dd 23:59")]
        yyyy_MM_dd_2359,
        /// <summary>
        /// ToString("HH:mm")
        /// </summary>
        [Description("HH:mm")]
        HHmm,
        /// <summary>
        /// ToString("yyyyMMddHHmmssfffff")
        /// </summary>
        [Description("yyyyMMddHHmmssfffff")]
        yyyyMMddHHmmssfffff,
        /// <summary>
        /// ToString("yyyy-MM-dd")
        /// </summary>
        [Description("yyyy-MM-dd")]
        yyyy_MM_dd,
        /// <summary>
        /// ToString("yyyy/MM/dd 23:59:59")
        /// </summary>
        [Description("yyyy/MM/dd 23:59:59")]
        yyyy_MM_dd_125959_withSlash,
        /// <summary>
        /// ToString("yyyy/MM/dd HH:mm:ss")
        /// </summary>
        [Description("yyyy/MM/dd HH:mm:ss")]
        yyyy_MM_dd_HHmmss_withSlash,
        /// <summary>
        /// ToString("yyyy/MM/dd HH:mm:ss")
        /// </summary>
        [Description("yyyy/MM/dd HH:mm")]
        yyyy_MM_dd_HHmm_withSlash,
        /// <summary>
        /// ToString("yyyy/MM/dd")
        /// </summary>
        [Description("yyyy/MM/dd")]
        yyyy_MM_dd_withSlash,
        /// <summary>
        /// ToString("yyyyMMdd")
        /// </summary>
        [Description("yyyyMMdd")]
        yyyyMMdd, 
        /// <summary>
        /// ToString("HHmmss")
        /// </summary>
        [Description("HHmmss")]
        HHmmss,

    }
}
