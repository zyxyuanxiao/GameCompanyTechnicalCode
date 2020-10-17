/********************************************************************
 Date: 2020-09-23
 Name: LogFile
 author:  zhuzizheng
*********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LogSystem
{
    public class LogFile : ILoggerInterface
    {
        public static string LogFilePath = Application.persistentDataPath + "/logs.log";
        private int totalframe = 5 * 60 * 60; //1s 60帧 ,每间隔 5 分钟进行再上传
        private int frame = 0;

        public void Initialize()
        {
            LogFilePath = Application.persistentDataPath + "/logs.log";
            totalframe = 5 * 60 * 60;
            frame = 0;
        }

        public void Update()
        {
            if (frame < totalframe) //不足 5 分钟则不上传
            {
                frame++;
            }
            else
            {
                frame = 0;
                UploadLog();
            }
        }

        public void UnInitialize()
        {
            StopUploadLog();
            totalframe = 5 * 60 * 60;
            frame = 0;
        }

        /// <summary>
        /// 向文件中追加 Log 信息
        /// 单纯的保存到日志中
        /// </summary>
        /// <param name="logs"></param>
        public void SaveLogFileToDevice(List<LogEntity> logs = null)
        {
            try
            {
                if (logs == null) logs = LogManager.GetLogCacheData().logs;
                if (!File.Exists(LogFilePath)) File.Create(LogFilePath);
                List<string> fileContentsList = new List<string>();
                for (int i = 0; i < logs.Count; i++)
                {
                    fileContentsList.Add(logs[i].logType + "\n" + logs[i].condition + "\n" + logs[i].stacktrace);
                }
                File.AppendAllLines(LogFilePath, fileContentsList);
            }
            catch (Exception e)
            {
                Debug.LogError("保存文件时,退出了游戏:\n" + e);
            }
        }

        public void CheckFile()
        {
            if (!File.Exists(LogFilePath)) File.Create(LogFilePath);
            FileInfo f = new FileInfo(LogFilePath);
            if ((f.Length / 1024 / 1024) > LogManager.GetLogCacheData().maxSize)
            {
                File.Delete(LogFilePath);
            }
            if (!File.Exists(LogFilePath)) File.Create(LogFilePath);
        }

        /// <summary>
        /// 上传到内网的服务器上面,外网不可用
        /// </summary>
        private BestHTTP.HTTPRequest _uploadRequest;
        public void UploadLog()
        {
            if ((_uploadRequest != null) && (_uploadRequest.State != BestHTTP.HTTPRequestStates.Finished))
            {
                return;
            }
            SaveLogFileToDevice();
            string svn = "1";//DataCenter.Instance?.VersionInfos?.VersionNum;
            string openid = "ClientFramework";//DataCenter.Instance.OpenID;
            if (!string.IsNullOrEmpty(openid))
            {
                string url = "http://10.19.11.216/receive_logs.php?svn_ver=" + svn + "&openid=" + openid;
                _uploadRequest = new BestHTTP.HTTPRequest(new Uri(url), BestHTTP.HTTPMethods.Post, null);
                _uploadRequest.UploadStream = new System.IO.FileStream(LogFile.LogFilePath, System.IO.FileMode.Open);
                _uploadRequest.Timeout = TimeSpan.FromSeconds(5); //5 秒超时
                _uploadRequest.Send();
            }
        }


        public void StopUploadLog()
        {
            if ((_uploadRequest != null) && (_uploadRequest.State != BestHTTP.HTTPRequestStates.Finished))
            {
                if (_uploadRequest.UploadStream != null)
                {
                    _uploadRequest.UploadStream.Dispose();
                    _uploadRequest.UploadStream.Close();
                    _uploadRequest.UploadStream = null;
                }

                _uploadRequest.DisposeUploadStream = true;
                _uploadRequest.Abort();
                _uploadRequest = null;
            }
        }
    }
}





/*
     private void SaveLogsToDevice()
{
string filePath = Application.persistentDataPath + "/logs.log";
List<string> fileContentsList = new List<string>();
Debug.Log("Saving logs to " + filePath);
File.Delete(filePath);
for (int i = 0; i < logs.Count; i++)
{
    fileContentsList.Add(logs[i].logType + "\n" + logs[i].condition + "\n" + logs[i].stacktrace);
}
File.WriteAllLines(filePath, fileContentsList.ToArray());
}
 
          */

/*
//read build information 
IEnumerator readInfo()
{
    string prefFile = "build_info.log"; 
    string url      = prefFile; 

    if (prefFile.IndexOf("://") == -1) url = System.IO.Path.Combine(Application.persistentDataPath, prefFile);
    if (!url.Contains("://")) url          = "file://" + url;

    // float startTime = Time.realtimeSinceStartup;

    UnityWebRequest www = UnityWebRequest.Get(url);
    yield return www.SendWebRequest();

    if (!string.IsNullOrEmpty(www.error)) 
    {
        Debug.LogError(www.error);
    }
    else
    {
        buildDate = www.downloadHandler.text;
    }

    yield break;
}*/
