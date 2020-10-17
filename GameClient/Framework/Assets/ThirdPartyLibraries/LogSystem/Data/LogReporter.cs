/********************************************************************
 Date: 2020-09-23
 Name: LogReporter
 author:  zhuzizheng
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace LogSystem
{
	/// <summary>
	/// 日志记者,将所有的日志进行记录
	/// </summary>
	public class LogReporter  : ILoggerInterface
	{
		private LogCacheData CacheData;
		private int LogHierarchyFrame = 60 * 60; //每隔 60 帧,记录一次Hierarchy的游戏物体节点
		private int frame = 0;
		private StringBuilder hierarchyBuilder;
		
		public void Initialize()
		{
			CacheData = LogManager.GetLogCacheData();
			Application.logMessageReceivedThreaded += LogMessageReceivedThreaded;
			AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;
			hierarchyBuilder = new StringBuilder ("");
		}
		
		/// <summary>
		/// 接收到的数据有可能子线程,有可能主线程
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="stacktrace"></param>
		/// <param name="type"></param>
		private void LogMessageReceivedThreaded(string condition, string stacktrace, LogType type)
		{
			if (LogManager.Instance == null) return;
			
			LogEntity log = new LogEntity() 
			                {condition = condition, stacktrace = stacktrace, logType = (LogType) type};
			
			lock (CacheData.threadedLogs) 
				CacheData.threadedLogs.Add(log);
		}

		private void LogUnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			if (args == null || args.ExceptionObject == null) {
				return;
			}
			if (LogManager.Instance == null) return;
			System.Exception e = (System.Exception) args.ExceptionObject;
			string condition = string.Format("{0}  {1}", e.GetType().Name, e.Message);
			
			StringBuilder stackTraceBuilder = new StringBuilder ("");
			StackTrace stackTrace = new StackTrace (e, true);
			int        count      = stackTrace.FrameCount;
			for (int i = 0; i < count; i++) {
				StackFrame frame = stackTrace.GetFrame (i);
            
				stackTraceBuilder.AppendFormat ("{0}.{1}", frame.GetMethod ().DeclaringType.Name, frame.GetMethod ().Name);
            
				ParameterInfo[] parameters = frame.GetMethod ().GetParameters ();
				if (parameters == null || parameters.Length == 0) {
					stackTraceBuilder.Append (" () ");
				} else {
					stackTraceBuilder.Append (" (");
                
					int pcount = parameters.Length;
                
					ParameterInfo param = null;
					for (int p = 0; p < pcount; p++) {
						param = parameters [p];
						stackTraceBuilder.AppendFormat ("{0} {1}", param.ParameterType.Name, param.Name);
                    
						if (p != pcount - 1) {
							stackTraceBuilder.Append (", ");
						}
					}
					param = null;
                
					stackTraceBuilder.Append (") ");
				}
            
				string fileName = frame.GetFileName ();
				if (!string.IsNullOrEmpty (fileName) && !fileName.ToLower ().Equals ("unknown")) {
					fileName = fileName.Replace ("\\", "/");
                
					int loc = fileName.ToLower ().IndexOf ("/assets/");
					if (loc < 0) {
						loc = fileName.ToLower ().IndexOf ("assets/");
					}
                
					if (loc > 0) {
						fileName = fileName.Substring (loc);
					}
                
					stackTraceBuilder.AppendFormat ("(at {0}:{1})", fileName, frame.GetFileLineNumber ());
				}
				stackTraceBuilder.AppendLine ();
			}
			
			
			LogEntity log = new LogEntity() 
			                {condition = condition, stacktrace = stackTraceBuilder.ToString(), logType = LogType.Exception};
			lock (CacheData.threadedLogs) 
				CacheData.threadedLogs.Add(log);
		}

		/// <summary>
		/// 主线程,不是子线程
		/// </summary>
		public void Update()
		{
			if (frame > LogHierarchyFrame)
			{
				frame++;
			}
			else
			{
				frame = 0;
				hierarchyBuilder.Clear();
				hierarchyBuilder.Append("Hierarchy上面的当前游戏物体,每 60 帧一次:\n");
				foreach (var go in GameObject.FindObjectsOfType<Transform>())
				{
					hierarchyBuilder.Append(go.name);
					hierarchyBuilder.Append("\n");
				}
				Debug.Log(hierarchyBuilder.ToString());
			}
			List<LogEntity> threadedLogs = CacheData.threadedLogs;
			if (threadedLogs.Count > 0)
			{
				lock (threadedLogs)
				{
					for (int i = 0; i < threadedLogs.Count; i++)
					{
						LogEntity l = threadedLogs[i];
						AddLog(l.condition, l.stacktrace, (LogType) l.logType);
					}

					threadedLogs.Clear();
				}
			}
		}

		public void UnInitialize()
		{
			List<LogEntity> threadedLogs = CacheData.threadedLogs;
			threadedLogs.Clear();
			Application.logMessageReceivedThreaded     -= LogMessageReceivedThreaded;
			AppDomain.CurrentDomain.UnhandledException -= LogUnhandledException;
		}


		void AddLog(string condition, string stacktrace, LogType type)
		{
			float           memUsage      = 0f;
			string          _condition    = "";
			List<LogEntity> collapsedLogs = CacheData.collapsedLogs;
			List<LogEntity> logs          = CacheData.logs;
			List<LogEntity> currentLog    = CacheData.currentLog;
			
			Dictionary<string, string> cachedString = CacheData.cachedString;
			if (cachedString.ContainsKey(condition))
			{
				_condition = cachedString[condition];
			}
			else
			{
				_condition = condition;
				cachedString.Add(_condition, _condition);
				memUsage += (string.IsNullOrEmpty(_condition) ? 0 : _condition.Length * sizeof(char));
				memUsage += System.IntPtr.Size;
			}

			string _stacktrace = "";
			if (cachedString.ContainsKey(stacktrace))
			{
				_stacktrace = cachedString[stacktrace];
			}
			else
			{
				_stacktrace = stacktrace;
				cachedString.Add(_stacktrace, _stacktrace);
				memUsage += (string.IsNullOrEmpty(_stacktrace) ? 0 : _stacktrace.Length * sizeof(char));
				memUsage += System.IntPtr.Size;
			}

			Sample sample = Sample.SaveASample();
			CacheData.graphMemUsage = sample.graphMemUsage;
			LogEntity log = new LogEntity()
			                {
				                logType  = (LogType) type, condition = _condition, stacktrace = _stacktrace,
				                sampleId = CacheData.samples.Count - 1
			                };
			memUsage += log.GetMemoryUsage();
			//memUsage += samples.Count * 13 ;

			CacheData.logsMemUsage += memUsage / 1024 / 1024;

			if (CacheData.TotalMemUsage > CacheData.maxSize)
			{
				Debug.Log("Memory Usage Reach" + CacheData.maxSize + " mb So It is Cleared");
				return;
			}

			bool isNew = false;
			//string key = _condition;// + "_!_" + _stacktrace ;
			LogMultiKeyDictionary<string, string, LogEntity> logsDic = CacheData.logsDic;
			if (logsDic.ContainsKey(_condition, stacktrace))
			{
				isNew = false;
				logsDic[_condition][stacktrace].count++;
			}
			else
			{
				isNew = true;
				collapsedLogs.Add(log);
				logsDic[_condition][stacktrace] = log;

				if (type == LogType.Log)
					CacheData.numOfCollapsedLogs++;
				else if (type == LogType.Warning)
					CacheData.numOfCollapsedLogsWarning++;
				else
					CacheData.numOfCollapsedLogsError++;
			}

			if (type == LogType.Log)
				CacheData.numOfLogs++;
			else if (type == LogType.Warning)
				CacheData.numOfLogsWarning++;
			else
				CacheData.numOfLogsError++;


			logs.Add(log);
			if (!CacheData.collapse || isNew)
			{
				bool skip = false;
				if (log.logType == LogType.Log && !CacheData.showLog)
					skip = true;
				if (log.logType == LogType.Warning && !CacheData.showWarning)
					skip = true;
				if (log.logType == LogType.Error && !CacheData.showError)
					skip = true;
				if (log.logType == LogType.Assert && !CacheData.showError)
					skip = true;
				if (log.logType == LogType.Exception && !CacheData.showError)
					skip = true;

				if (!skip)
				{
					string filterText = CacheData.filterText;
					if (string.IsNullOrEmpty(filterText) || log.condition.ToLower().Contains(filterText.ToLower()))
					{
						currentLog.Add(log);
					}
				}
			}
		}

	}
}