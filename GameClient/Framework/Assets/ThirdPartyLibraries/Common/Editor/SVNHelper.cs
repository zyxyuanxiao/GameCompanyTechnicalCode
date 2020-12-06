using System.IO;
using System.Text;
using UnityEngine;

namespace Common
{
	public class SVNHelper
	{
		public static string GetSvnVersion()
		{
#if UNITY_EDITOR && UNITY_EDITOR_OSX
			return SvnInfoUpdateOSX();
#elif UNITY_EDITOR && UNITY_EDITOR_WIN
		return SvnInfoUpdateWIN();
#else
		return "0";	
#endif
		}

		public static string GetSvnBlame(string filepath)
		{
#if UNITY_EDITOR && UNITY_EDITOR_OSX
			return SvnBlameOSX(filepath);
#elif UNITY_EDITOR && UNITY_EDITOR_WIN
		return SvnBlameWIN(filepath);
#else
		return "0";	
#endif
		}

#if UNITY_EDITOR_OSX
		/// <summary>
		/// 刷新svn版本信息
		/// </summary>
		[UnityEditor.MenuItem("Tools/SVN/SVN Version")]
		static string SvnInfoUpdateOSX()
		{
			string cmd = Application.dataPath + "/../shell/svn_info.command";
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.FileName = cmd;
			process.StartInfo.Arguments = Application.dataPath + "/../";
			process.StartInfo.RedirectStandardOutput = true;

			process.Start();
			StringBuilder q = new StringBuilder();
			while (!process.HasExited)
			{
				string s = process.StandardOutput.ReadToEnd();
				q.Append(s);
			}

			string result = q.ToString();
			result = result.Replace("\r\n", "");
			result = result.Replace("\n", "");
			if (string.IsNullOrEmpty(result))result = "1";
			Debug.Log("svn:" + result);
			return result;
		}


		/// <summary>
		/// svn信息
		/// </summary>
		static string SvnBlameOSX(string filePath)
		{
			string cmd = Application.dataPath + "/../shell/svn_blame.command";
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.FileName = cmd;
			process.StartInfo.Arguments = filePath;
			process.StartInfo.RedirectStandardOutput = true;

			process.Start();
			StringBuilder q = new StringBuilder();
			while (!process.HasExited)
			{
				string s = process.StandardOutput.ReadToEnd();
				q.Append(s);
			}

			string result = q.ToString();
			result = result.Replace("\r\n", "");
			result = result.Replace("\n", "");
			if (string.IsNullOrEmpty(result))result = "1";
			Debug.Log(result);
			return result;
		}

		/// <summary>
		/// 刷新svn版本信息
		/// </summary>
		[UnityEditor.MenuItem("Tools/SVN/Open UserPath")]
		static void OpenUserPath()
		{
			string cmd = Application.dataPath + "/../shell/open_userpath.command";
			System.Diagnostics.Process sh = new System.Diagnostics.Process();
			sh.StartInfo.UseShellExecute = true;
			sh.StartInfo.FileName = cmd;
			sh.StartInfo.Arguments = "\"" + Application.persistentDataPath + "\"";
			sh.Start();
			sh.WaitForExit();
		}
#endif

#if UNITY_EDITOR && !UNITY_EDITOR_OSX
	[UnityEditor.MenuItem("Tools/SVN/SVN Version")]
	public static string SvnInfoUpdateWIN()
	{
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		string projectPath = Application.dataPath.Replace("/", "\\");
		string[] cmds = projectPath.Split('\\');
		string path = string.Empty;
		for (int i = 0; i < cmds.Length; i++)
		{
			if (cmds[i].Equals("Assets"))
			{
				break;
			}

			path += cmds[i] + "\\";
		}
		path += "shell\\svn_info.bat";
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.FileName = path;
		process.StartInfo.Arguments = Application.dataPath + "/../";
		process.StartInfo.RedirectStandardOutput = true;

		process.Start();
		StringBuilder q = new StringBuilder();
		while (!process.HasExited)
		{
			string s = process.StandardOutput.ReadToEnd();
			q.Append(s);
		}
		
		string result = q.ToString();
		result = result.Replace("\r\n", "|");
		result = result.Replace("\n", "|");
		string[] res = result.Split('|');
		result = res[res.Length - 2];
		Debug.Log("svn:" + result);
		return result;
	}
	static string SvnBlameWIN(string filePath)
	{
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		string projectPath = Application.dataPath.Replace("/", "\\");
		string[] cmds = projectPath.Split('\\');
		string path = string.Empty;
		for (int i = 0; i < cmds.Length; i++)
		{
			if (cmds[i].Equals("Assets"))
			{
				break;
			}

			path += cmds[i] + "\\";
		}
		path += "shell\\svn_blame.bat";
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.FileName = path;
		process.StartInfo.Arguments = filePath;
		process.StartInfo.RedirectStandardOutput = true;

		process.Start();
		StringBuilder q = new StringBuilder();
		while (!process.HasExited)
		{
			string s = process.StandardOutput.ReadToEnd();
			q.Append(s);
		}
		string result = q.ToString();
		Debug.Log("输出:" + result);
		return result;
	}
#endif

#if UNITY_EDITOR_OSX
		/// <summary>
		/// 更新svn
		/// </summary>
		[UnityEditor.MenuItem("Tools/SVN/SVN Update(OSX) %#u")]
		public static void SvnUpdateOSX()
		{
			string cmd = Application.dataPath + "/../shell/svn_update.command";
			System.Diagnostics.Process sh = new System.Diagnostics.Process();
			sh.StartInfo.UseShellExecute = true;
			sh.StartInfo.FileName = cmd;
			sh.StartInfo.Arguments = Application.dataPath + "/../";
			sh.Start();
			sh.WaitForExit();

			string log = File.ReadAllText(Application.dataPath + "/StreamingAssets/packVersionInfo");
			Debug.Log(log);
		}

#endif

#if !UNITY_EDITOR_OSX && UNITY_EDITOR
	[UnityEditor.MenuItem("Tools/SVN/SVN Update(WIN)")]
	public static void SvnUpdateWIN()
	{
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		string projectPath = Application.dataPath.Replace("/", "\\");
		string[] cmds = projectPath.Split('\\');
		string path = string.Empty;
		for (int i = 0; i < cmds.Length; i++)
		{
			if (cmds[i].Equals("Assets"))
			{
				break;
			}

			path += cmds[i] + "\\";
		}
		path += "shell\\svn_update.bat";
		process.StartInfo.FileName = path;
//		process.StartInfo.CreateNoWindow = true;
//		process.StartInfo.UseShellExecute = false;
//		process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
		process.Start();
//		process.WaitForExit();
		
		string log = File.ReadAllText(Application.dataPath + "/StreamingAssets/packVersionInfo");
		Debug.Log(log);
	}
#endif
	}

}