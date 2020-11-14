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
                    return scriptPath.Replace(scriptName+".cs","");
                }
            }
            Debug.LogError("没有找到脚本路径");
            return null;
        }
    }
}