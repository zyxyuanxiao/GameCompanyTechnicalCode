using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace Best {
	public class AssetBundleExistsInfo
	{	
		private static Dictionary<string, bool> updateFileInfo;

		private static bool hadInit = false;
		
		
		public static void Init()
		{
            lock (lockObj)
            {
                if (hadInit)
                {
                    return;
                }
                //Debug.Log("AssetBundleExistsInfo Init");
                if (updateFileInfo == null || updateFileInfo.Keys.Count == 0)
                {
                    updateFileInfo = new Dictionary<string, bool>();
                }
                InitPlayingDownloadFile();
                hadInit = true;
            }
		}

		public static FileFixType CheckFileType(string name)
		{	
			if (hadInit == false)
			{
				Init();
			}
			//判断边玩边下是否有这个文件
			FileFixType playingDownFileFixType = FileFixType.Normal;
			if (updateFileInfo.ContainsKey(name))
			{
				if (updateFileInfo[name])
				{
					playingDownFileFixType = FileFixType.Encryp;
				}
			}
			else
			{
				playingDownFileFixType = FileFixType.None;
			}

			//判断热更是否有这个文件
			FileFixType updateFileFixType = FileFixType.None;

			//如果边玩边下和热更都没有这个文件的话，那返回none
			if (playingDownFileFixType == FileFixType.None && updateFileFixType == FileFixType.None)
			{
				return FileFixType.None;
			}
			else
			{
				if (playingDownFileFixType == FileFixType.Encryp || updateFileFixType == FileFixType.Encryp)
				{
					return FileFixType.Encryp;
				}
				else
				{
					return FileFixType.Normal;
				}
			}
			
		}
		public static void ForceInit()
		{
			hadInit = false;
			Init();
		}

		private static object lockObj = new object();
		private static void InitPlayingDownloadFile()
		{
			string localUnZipPath = BundleConfig.Instance.BundlesPathForPersist;
			if (!Directory.Exists(localUnZipPath))
			{
				return;
			}

			List<string> md5FileList = new List<string>();
			string md5FileRecordPath = Path.Combine(localUnZipPath, "MD5FileRecord.txt");
			using (StreamReader streamReader = new StreamReader(File.Open(md5FileRecordPath, FileMode.OpenOrCreate)))
			{
				while (!streamReader.EndOfStream)
				{
					md5FileList.Add(streamReader.ReadLine());
				}
			}

			if (md5FileList.Count <= 0)
			{
				return;
			}
				
			StringBuilder sb = LuaInterface.StringBuilderCache.Acquire();
			bool hadFile = false;
			foreach (string md5FilePath in md5FileList)
			{
				if (File.Exists(md5FilePath))
				{
					hadFile = true;
					using (StreamReader streamReader = new StreamReader(File.Open(md5FilePath, FileMode.Open)))
					{
						sb.Append(streamReader.ReadToEnd());
					}
				}
			}
			
			if (hadFile)
			{
				UpdateFileInfo(sb);
			}
		}

		public static void AddNewAbRecord(string md5FilePath)
        {
            lock (lockObj)
            {
                if (hadInit == false)
                {
                    Init();
                }
                
                if (File.Exists(md5FilePath))
                {
	                StringBuilder sb = LuaInterface.StringBuilderCache.Acquire();
	                using (StreamReader streamReader = new StreamReader(File.Open(md5FilePath, FileMode.Open)))
	                {
		                sb.Append(streamReader.ReadToEnd());
	                }
	                UpdateFileInfo(sb);
                }
            }
        }
		
		private static void UpdateFileInfo(StringBuilder sb)
		{
			string[] allFileArr = LuaInterface.StringBuilderCache.GetStringAndRelease(sb).Split('\n');
			string[] currFileArr;
			for (int i = 0; i < allFileArr.Length; i++)
			{
				currFileArr = allFileArr[i].Split('|');
				if (currFileArr.Length == 2)
				{
					if (currFileArr[0].EndsWith(BundleConfig.EncrypFilePostfix))
					{
						updateFileInfo[string.Intern(currFileArr[0].Replace(BundleConfig.CompleteEncrypFilePostfix, ""))] =
							true;
					}
					else
					{
						updateFileInfo[string.Intern(currFileArr[0])] = false;
					}
				}
			}
		}
	}

	/// <summary>
	/// 更新文件的类型
	/// </summary>
	public enum FileFixType
	{
		None=0,//不存在这个更新文件
		Normal=1,//.ab后缀文件
		Encryp=2 //.ecp加密后缀文件
	}
}