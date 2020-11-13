using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Common
{
    public static class Tool
    {
        public static string QueryPlatform()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.OSXPlayer :
                    return "OSX";
                case RuntimePlatform.OSXEditor:
                    return "OSX";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.WindowsEditor:
                    return "Windows";
                default:
                    return "Unknown";
            }
        }
        
        #region 根据文件的日期变动,检查是否被修改,简单防作弊
        
        
        /// <summary>
        /// 获取字符串SH1哈希值
        /// </summary>
        /// <param name="str_sha1_in"></param>
        /// <returns></returns>
        public static string QuerySHA1HashOfString(string str_sha1_in)
        {
            byte[] bytes = UTF8Encoding.Default.GetBytes(str_sha1_in);
            return QuerySHA1HashOfString(bytes);
        }
        
        /// <summary>
        /// 获取字符串SH1哈希值
        /// </summary>
        /// <param name="str_sha1_in"></param>
        /// <returns></returns>
        public static string QuerySHA1HashOfString(byte[] bytes)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_sha1_in = bytes;
            byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);

            string str_sha1_out = BitConverter.ToString(bytes_sha1_out).Replace("-", "");
            //str_sha1_out = str_sha1_out.Replace("-", "");
            return str_sha1_out;
        }
        
        /// <summary>
        /// Appends the directory last write time.
        /// </summary>
        /// <returns>The directory last write time.</returns>
        /// <param name="path">Path.</param>
        public static string QueryAppendDirectoryLastWriteTime(string path)
        {
            if (!Directory.Exists(path))
            {
                return "0";
            }
            List<string> list = new List<string>();
            QueryAppendDirectoryLastWriteTime(path, ref list);
            list.Sort(CompareString);
            string result = "";
            for (int i = 0; i < list.Count; i++)
            {
                result += list[i];
            }
            return result;
        }

        private static void QueryAppendDirectoryLastWriteTime(string path, ref List<string> list)
        {
            //判断源目录和目标目录是否存在，如果不存在，则创建一个目录
            list.Add(Directory.GetLastWriteTime(path).ToFileTimeUtc().ToString());

            System.IO.DirectoryInfo floder = System.IO.Directory.CreateDirectory(path);
            DirectoryInfo[] subFloders = floder.GetDirectories();

            for (int i = 0; i < subFloders.Length; i++)
            {
                //递归调用
                QueryAppendDirectoryLastWriteTime(subFloders[i].FullName, ref list);
            }
        }
        
        
        /// <summary>
        /// 排序准则
        /// </summary>
        /// <returns>The SK.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        public static int CompareString(string a, string b)
        {
            if (a[0] < b[0])
                return -1;
            else if (a[0] > b[0])
                return 1;
            else
            {
                int La = a.Length;
                int Lb = b.Length;
                int L = System.Math.Min(La, Lb);
                for (int i = 1; i < L; i++)
                {
                    if (a[i] < b[i])
                    {
                        return -1;
                    }
                    if (a[i] > b[i])
                    {
                        return 1;
                    }
                }
                if (La < Lb)
                {
                    return -1;
                }
                else if (La > Lb)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion
    }
}