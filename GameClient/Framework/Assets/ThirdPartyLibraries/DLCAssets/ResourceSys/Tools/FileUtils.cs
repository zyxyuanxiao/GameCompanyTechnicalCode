﻿using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using Best;
using System.IO.Compression;
using System.Text;
using LuaInterface;

public class FileUtils
{
    public const string SEPARATOR = "|";

    private static VersionInfo curVerInfo;

    public static VersionInfo GetCurrentVerNo(bool isForce = false)
    {
        if (curVerInfo == null || isForce)
        {
            try
            {
                //获取streamingAssetsPath目录下版本号
                string streamingVerFileUrl = string.Format("AssetBundles/{0}/{1}", BundleConfig.Instance.BundlePlatformStr, BundleConfig.Instance.VersionFileName);

                if (BundleConfig.IsConfuseModel)
                {
                    streamingVerFileUrl = string.Format("AssetBundles/{0}/{1}/{2}", BundleConfig.Instance.BundlePlatformStr, BundleConfig.ConfusedFolder, BundleConfig.Instance.VersionFileName);
                }

                string streamingVerFileInfo = null;
                Stream stream = StreamingAssetLoad.GetFile(streamingVerFileUrl);
                if (stream == null)
                {
                    LuaInterface.Debugger.LogError("本地版本文件不存在，{0}", streamingVerFileUrl);
                }
                else
                {
                    StreamReader st = new StreamReader(stream);
                    streamingVerFileInfo = st.ReadToEnd();
                    st.Close();
                    stream.Close();

                    VersionInfo streamingVerInfo = LitJson.JsonMapper.ToObject<VersionInfo>(streamingVerFileInfo);

                    //获取persistentDataPath目录下版本文件
                    string persistentVerFileUrl = BundleConfig.Instance.BundlesPathForPersist + BundleConfig.Instance.VersionFileName;
                    if (File.Exists(persistentVerFileUrl))
                    {
                        string persistentVerFileInfo = File.ReadAllText(persistentVerFileUrl);
                        VersionInfo persistentVerInfo = LitJson.JsonMapper.ToObject<VersionInfo>(persistentVerFileInfo);

                        Version persistentVer = new Version(persistentVerInfo.VersionNum);
                        Version streamingVer = new Version(streamingVerInfo.VersionNum);

                        if (persistentVer >= streamingVer)
                        {
                            curVerInfo = persistentVerInfo;
                        }
                        else
                        {
                            //如果补丁版本较低，则回被删除
                            Debug.LogWarning("Patch res is lower than cur ver no");
                            curVerInfo = streamingVerInfo;
                            string persistentBundlePath = BundleConfig.Instance.BundlesPathForPersist;
                            DirectoryInfo di = new DirectoryInfo(persistentBundlePath);
                            if (di.Exists)
                                di.Delete(true);
                        }
                    }
                    else
                    {
                        curVerInfo = streamingVerInfo;
                        string persistentBundlePath = BundleConfig.Instance.BundlesPathForPersist;
                        DirectoryInfo di = new DirectoryInfo(persistentBundlePath);
                        if (di.Exists)
                            di.Delete(true);
                    }
                }
            }
            catch { LuaInterface.Debugger.LogError("FileUtils: get current version fail !"); }
        }

        return curVerInfo;
    }

    /* 文件协议 */
#if UNITY_EDITOR
    public static string STREAM_FILE_PROTOCAL_ = "file:///";
#elif UNITY_IPHONE
            public static string STREAM_FILE_PROTOCAL_ = "file://";
#else
            public static string STREAM_FILE_PROTOCAL_ = "";
#endif

#if UNITY_EDITOR
    public static string PERSISTENT_FILE_PROTOCAL_ = "file:///";
#else
            public static string PERSISTENT_FILE_PROTOCAL_ = "file://";
#endif

    /* 获取stream文件路径前缀*/

    public static string GetStreamingFilePath()
    {
#if UNITY_EDITOR
        return "file:///" + Application.streamingAssetsPath;
#elif UNITY_IPHONE
        return "file://" + Application.streamingAssetsPath;
#else
        return Application.streamingAssetsPath;
#endif
    }

    /// <summary>
    /// 文件是否存在
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool FileExist(string name)
    {
        return File.Exists(name);
    }

    /// <summary>
    /// 保存文件.
    /// </summary>
    /// <param name="content">Content.</param>
    /// <param name="filename">Filename.</param>
    public static void SaveToFile(string content, string dirname, string filename)
    {
        byte[] byte_xml = System.Text.Encoding.Default.GetBytes(content);
        SaveToFile(byte_xml, dirname, filename);
    }

    public static void SaveToFile(byte[] bytes, string dirname, string filename)
    {
        if (!Directory.Exists(dirname))
        {
            Directory.CreateDirectory(dirname);
        }

        string filepath = dirname + filename;

        /* 把xml存储到文件中 */
        using (FileStream fileStream = new FileStream(filepath, File.Exists(filepath) ? FileMode.Truncate : FileMode.OpenOrCreate))
        {
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();
            fileStream.Dispose();
        }
    }

    /// <summary>
    /// 写入内容到路径文件中（中文）
    /// </summary>
    /// <param name="content"></param>
    /// <param name="filepath"></param>
    public static void SaveToFile(string content, string filepath)
    {
        byte[] bytes = Encoding.GetEncoding("utf-8").GetBytes(content);
        /* 把xml存储到文件中 */
        using (FileStream fileStream = new FileStream(filepath, File.Exists(filepath) ? FileMode.Truncate : FileMode.OpenOrCreate))
        {
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();
            fileStream.Dispose();
        }
    }

    // 读取文件对应的byte数组
    public static byte[] getFileBytes(string filepath)
    {
        if (File.Exists(filepath) == false)
        {
            return new byte[0] { };
        }
        byte[] bytes = null;
        using (FileStream fileStream = new FileStream(filepath, FileMode.Open))
        {
            bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, int.Parse(fileStream.Length + ""));

            fileStream.Close();
            fileStream.Dispose();
        }
        return bytes;
    }

    // 读取文件对应的字符串
    public static string getFileString(string filepath)
    {
        if (File.Exists(filepath) == false)
        {
            return "";
        }
        byte[] bytes = null;
        using (FileStream fileStream = new FileStream(filepath, FileMode.Open))
        {
            bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, int.Parse(fileStream.Length + ""));

            fileStream.Close();
            fileStream.Dispose();
        }
        return System.Text.Encoding.ASCII.GetString(bytes);
    }

    /// <summary>
    /// 获取文件大小.
    /// </summary>
    /// <returns>The file size.</returns>
    /// <param name="filepath">Filepath.</param>
    public static long GetFileSize(string filepath)
    {
        FileInfo bundleinfo = new FileInfo(filepath);
        long bundlesize = bundleinfo.Length;
        return bundlesize;
    }

    /// <summary>
    /// 获取文件的 md5.
    /// </summary>
    /// <returns>The file md5.</returns>
    /// <param name="fileurl">Fileurl.</param>
    public static string getFileMd5(string fileurl)
    {
        string strmd5 = "";
        if (File.Exists(fileurl) == true)
        {
            using (FileStream fileStream = new FileStream(fileurl, FileMode.Open))
            {
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, int.Parse(fileStream.Length + ""));

                MD5 md5 = MD5.Create();
                byte[] bMd5 = md5.ComputeHash(bytes);

                md5.Clear();

                for (int i = 0; i < bMd5.Length; i++)
                {
                    strmd5 += bMd5[i].ToString("x").PadLeft(2, '0');
                }

                fileStream.Close();
                fileStream.Dispose();
            }
        }
        else
        {
#if DEV_BUILD
            Debug.LogError(fileurl + "文件不存在");
#endif
        }
        return strmd5;
    }

    public static string getBytesMd5(byte[] bytes)
    {
        string strmd5 = "";

        if (bytes != null)
        {
            MD5 md5 = MD5.Create();
            byte[] bMd5 = md5.ComputeHash(bytes);

            md5.Clear();

            for (int i = 0; i < bMd5.Length; i++)
            {
                strmd5 += bMd5[i].ToString("x").PadLeft(2, '0');
            }
        }

        return strmd5;
    }

    public static void renameFile(string source, string destination, bool bDeleteIfExit = false)
    {
        if (File.Exists(destination) == true)
        {
            if (bDeleteIfExit)
            {
                File.Delete(destination);
            }
            else
            {
                return;
            }
        }
        File.Move(source, destination);
    }

    public static void copyFile(string source, string destination)
    {
        if (File.Exists(destination) == true)
        {
            File.Delete(destination);
        }
        try
        {
            File.Copy(source, destination);
        }
        catch
        {
#if DEV_BUILD
            Debug.Log("拷贝文件" + source + "发生错误，请检查是否存在");
#endif
        }
    }

    /// <summary>
    /// 获取文件列表.
    /// </summary>
    /// <returns>The file list.</returns>
    /// <param name="directory_list">Directory_list.</param>
    public static List<string> getFileList(string dirpath, string cont = "*.*")
    {
        List<string> file_list = new List<string>();

        /* 遍历所有文件 枚举所有依赖 */
        DirectoryInfo directory = new DirectoryInfo(dirpath);
        FileInfo[] dirs = directory.GetFiles(cont, SearchOption.AllDirectories);

        /* 遍历所有Prefab */
        foreach (FileInfo info in dirs)
        {
            /* 把遍历到的资源路径存起来 */
            file_list.Add(info.FullName);
        }

        return file_list;
    }

    // 清空一个文件的content
    public static void Clear(string fileName)
    {
        if (File.Exists(fileName) == true)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                fileStream.SetLength(0);
                fileStream.Close();
            }
        }
    }

    // 清空一个文件夹
    public static void ClearDir(string dir)
    {
        try
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            Directory.CreateDirectory(dir);
        }
        catch
        {
#if DEV_BUILD
            Debug.LogError("清空文件夹" + dir + "出错");
#endif
        }
    }

    public static byte[] GetBytes(string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    public static string GetString(byte[] bytes)
    {
        char[] chars = new char[bytes.Length / sizeof(char)];
        System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

    public static void CreateAndWriteToFile(string fileName, byte[] bytes)
    {
        FileStream versionFile = new FileStream(fileName, FileMode.Create);
        BinaryWriter versionBinary = new BinaryWriter(versionFile);
        versionBinary.Write(bytes);
        versionFile.Close();
        //File.WriteAllBytes(fileName, bytes);
    }

    public static void CreateAndWriteToFile(string fileName, string content)
    {
        File.WriteAllText(fileName, content);
    }

    public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
        DirectoryInfo[] dirs = dir.GetDirectories();

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        // If the destination directory doesn't exist, create it.
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            if (File.Exists(temppath))
                File.Delete(temppath);
            file.MoveTo(temppath);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
        }
    }

    // 获取length对应的字符串
    public static string GetSizeString(int length)
    {
        if (length < 1024)
        {
            return length + " bytes";
        }
        else if (length < 1048576)
        {
            float num = (float)length / 1024;
            return num.ToString("F2") + " KB";
        }
        else
        {
            float num = (float)length / 1048576;
            return num.ToString("F2") + " MB";
        }
    }

    // 获取指定长度的字符，如果是中文则取一般长度
    public static string CutStr(string str, int len)
    {
        if (str == null || str.Length == 0 || len <= 0)
        {
            return string.Empty;
        }

        int l = str.Length;

        #region 计算长度

        int clen = 0;
        while (clen < len && clen < l)
        {
            //每遇到一个中文，则将目标长度减一。
            if ((int)str[clen] > 128) { len--; }
            clen++;
        }

        #endregion 计算长度

        if (clen < l)
        {
            return str.Substring(0, clen) + "..";
        }
        else
        {
            return str;
        }
    }

    /// <summary>
    /// 得到去除后缀和路径的single file name
    /// </summary>
    public static string GetSingleFileName(string fullPathFileName)
    {
        string fileName = fullPathFileName.Split('.')[0];
        string[] tmpStringArray = fileName.Split('\\');
        fileName = tmpStringArray[tmpStringArray.Length - 1];
        return fileName;
    }

    /// <summary>
    /// 比较新老版本大小.
    /// </summary>
    public static int CompareVersion(string oldVersion, string newVersion)
    {
        Version oldVer = new Version(oldVersion);
        Version newVer = new Version(newVersion);

        return newVer.CompareTo(oldVer);
    }


    /// <summary>
    /// 加载配置文件
    /// </summary>
    /// <param name="resPath">Resources目录相对路径</param>
    /// <param name="fileSuffix">文件后缀名</param>
    /// <returns>文件的bytes</returns>
    public static byte[] LoadConfigFile(string resPath, string fileSuffix)
    {
        byte[] bytes = null;
       
        System.Text.StringBuilder fileUrl = LuaInterface.StringBuilderCache.Acquire();
        fileUrl.Append(resPath);
        fileUrl.Append(".");
        fileUrl.Append(fileSuffix);

        
        if (AssetBundleExistsInfo.CheckFileType(fileUrl.ToString()) == FileFixType.Normal)//判断热更记录文件和边玩边下记录文件是否有这个文件
        {
            StringBuilder finalPath = LuaInterface.StringBuilderCache.Acquire();
            finalPath.Append(Best.BundleConfig.Instance.BundlesPathForPersist);
            finalPath.Append(LuaInterface.StringBuilderCache.GetStringAndRelease(fileUrl));
            bytes = File.ReadAllBytes(LuaInterface.StringBuilderCache.GetStringAndRelease(finalPath));
           
        }
        else if (BundleConfig.Instance.hadOpenGm)
        {
           
            StringBuilder finalPath = LuaInterface.StringBuilderCache.Acquire();
            finalPath.Append(Best.BundleConfig.Instance.BundlesPathForPersist);
            finalPath.Append(LuaInterface.StringBuilderCache.GetStringAndRelease(fileUrl));
           
            if (File.Exists(finalPath.ToString()))
            {
                bytes = File.ReadAllBytes(LuaInterface.StringBuilderCache.GetStringAndRelease(finalPath));
            }
        }
        
        if(bytes == null)
        {
            TextAsset asset = Resources.Load<TextAsset>(resPath);
            if (asset != null)
            {
                bytes = asset.bytes;
                Resources.UnloadAsset(asset);
            }
        }

        if (bytes == null)
        {
            if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer) Debugger.LogError("can find config:" + resPath);
        }

        return bytes;
    }
}