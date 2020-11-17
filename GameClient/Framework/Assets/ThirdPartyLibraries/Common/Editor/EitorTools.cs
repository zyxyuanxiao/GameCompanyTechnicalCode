using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Common
{
    public static class EitorTools
    {
        /*获取当前脚本的文件夹路径，参数为脚本的名字*/
        public static string GetScriptPath(Type type)
        {
            string scriptName = type.ToString().Split('.').Last();
            string[] ids = UnityEditor.AssetDatabase.FindAssets(scriptName);
            foreach (string id in ids)
            {
                string scriptPath = AssetDatabase.GUIDToAssetPath(id);
                if (Path.GetExtension(scriptPath).ToLower().Contains("cs"))
                {
                    return scriptPath.Replace(scriptName + ".cs", "");
                }
            }

            Debug.LogError("没有找到脚本路径");
            return null;
        }

        /// <summary>
        /// 复制文件夹及文件
        /// </summary>
        /// <param name="sourceFolder">原文件路径</param>
        /// <param name="destFolder">目标文件路径</param>
        /// <returns></returns>
        public static void CopyDirAndFile(string sourceDir, string destDir)
        {
            string folderName = Path.GetFileName(sourceDir);
            string destfolderdir = Path.Combine(destDir, folderName);
            string[] filenames = Directory.GetFileSystemEntries(sourceDir);
            foreach (string file in filenames) // 遍历所有的文件和目录
            {
                if (Directory.Exists(file))
                {
                    string currentdir = Path.Combine(destfolderdir, Path.GetFileName(file));
                    if (!Directory.Exists(currentdir))
                    {
                        Directory.CreateDirectory(currentdir);
                    }

                    CopyDirAndFile(file, destfolderdir);
                }
                else
                {
                    string srcfileName = Path.Combine(destfolderdir, Path.GetFileName(file));
                    if (!Directory.Exists(destfolderdir))
                    {
                        Directory.CreateDirectory(destfolderdir);
                    }

                    if (Path.GetExtension(file).ToLower().Contains(".ds_store")) continue;
                    if (Path.GetExtension(file).ToLower().Contains(".manifest")) continue;
                    File.Copy(file, srcfileName,true);
                }
            }
        }

    }
}