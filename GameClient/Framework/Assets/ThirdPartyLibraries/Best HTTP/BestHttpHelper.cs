using System;
using UnityEngine;

namespace BestHTTP
{
    public static class BestHttpHelper
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/BestHttp/Test Get")]
        public static void TestBestHttp()
        {
            GET("http://127.0.0.1/DownloadAssets/OSX/VersionConfig.json",(b,s) =>
            {
                if (b)//请求成功,应该进行
                {
                    //资源服务器上面的版本配置文件
                    Debug.Log(s);
                }
                else
                {
                    Debug.LogError("资源服务器版本配置文件下载失败");
                }
            });
        }
#endif
        
        public static void GET(string url,Action<bool,string> end)
        {
            HTTPRequest getRequest = new HTTPRequest(new Uri(url), (request, response) =>
            {
                if (request.State == HTTPRequestStates.Finished && response.IsSuccess)
                {
                    end?.Invoke(true,response.DataAsText);
                }
                else
                {
                    Debug.LogError("GET 请求错误:" + url +"    " + response.DataAsText);
                    end?.Invoke(false,"");
                }
            });
            getRequest.IsKeepAlive = false;
            getRequest.DisableCache = true;
            getRequest.Timeout = TimeSpan.FromSeconds(2);
            getRequest.Send();
        }

        #region Download
        
        private static Action<bool,byte[],int> _DownloadAction;//第一个 bool 表示是否报错,true 为下载错误
        public static void Download(string url,Action<bool,byte[],int> d)
        {
            _DownloadAction = d;
            HTTPRequest dRequest = new HTTPRequest(new Uri(url),OnRequestFinished);
            dRequest.OnStreamingData += OnStreamingData;
            dRequest.Send();
        }
        
        private static bool OnStreamingData(HTTPRequest request, HTTPResponse response, byte[] dataFragment,
            int dataFragmentLength)
        {
            if (response.IsSuccess)
            {
                _DownloadAction?.Invoke(false, dataFragment, dataFragmentLength);
            }
            else
            {
                _DownloadAction?.Invoke(true, null, -200);
                Debug.LogError("Download 错误:" + request.Uri.ToString());
            }
            return true;
        }

        private static void OnRequestFinished(HTTPRequest request, HTTPResponse response)
        {
            if (request.State == HTTPRequestStates.Finished && response.IsSuccess)
            {
                _DownloadAction?.Invoke(false, null, -100);
            }
            else
            {
                _DownloadAction?.Invoke(true, null, -200);
                Debug.LogError("Download 错误:" + request.Uri.ToString());
            }
        }

        #endregion
        

        
    }
}