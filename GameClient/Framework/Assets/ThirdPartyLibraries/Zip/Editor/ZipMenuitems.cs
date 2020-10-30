using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ZipMenuitems
{
    [MenuItem("Tools/Zip/Gunzip File", false, 50003)]
    private static void GunzipFile()
    {
        //解压缩文件
        string zipPath = EditorUtility.OpenFilePanel("OpenFile", Environment.CurrentDirectory, "");
        if (string.IsNullOrEmpty(zipPath))
        {
            return;
        }
        ZipResult zipResult=new ZipResult();
        string targetPath = Environment.CurrentDirectory + "/zip/";
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }
        LZ4Helper.Decompress(zipPath,targetPath,ref zipResult);
        Debug.Log("Zip解压缩路径:" + targetPath);
    }
}
