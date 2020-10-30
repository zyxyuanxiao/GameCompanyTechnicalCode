﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

public class AssetZip
{
//#if !UNITY_IPHONE || UNITY_EDITOR
    ZipFile mZipfile = null;

    // 释放资源
    public void Release()
    {
        if (mZipfile != null)
        {
            mZipfile.Close();
            mZipfile = null;
        }
    }

    public void Init(string filepath, string password = "")
    {
        try
        {
            mZipfile = new ZipFile(filepath);
            mZipfile.Password = password;
            InitFileList();
        }
        catch (System.Exception e)
        {
            return;
        }
    }

    Dictionary<string, long> mZipEntrys = new Dictionary<string, long>();

    // 初始化文件列表
    void InitFileList()
    {
        // zip包当中的所有文件列表
        IEnumerator itor = mZipfile.GetEnumerator();
        while (itor.MoveNext())
        {
            ZipEntry entry = itor.Current as ZipEntry;
            if (entry.IsFile)
                mZipEntrys.Add(entry.Name.ToLower(), entry.ZipFileIndex);
        }
    }

    public Stream FindFileStream(string file)
    {
        long entryIndex = -1;
        if (!mZipEntrys.TryGetValue(file.ToLower(), out entryIndex))
        {
            return null;
        }

        return mZipfile.GetInputStream(entryIndex);
    }

    public void EachAllFile(System.Action<string> fun)
    {
        foreach (KeyValuePair<string, long> itor in mZipEntrys)
            fun(itor.Key);
    }
//#endif
}
