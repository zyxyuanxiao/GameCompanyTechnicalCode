/********************************************************************
	created:	2017/09/02 15:26
	file base:	BuildManifestUtility
	file ext:	cs
	author:		wuzhou

	purpose:	Util class for Build Manifest
*********************************************************************/

using System;
using UnityEngine;
using Best;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using LitJson;

public static class BuildManifestUtility 
{
	public const string BuildManifestFileName = "BuildManifest.bytes";
	public const string EncryptBuildManifestFileName = "ebm.bytes";
	public const string EncryptKeyFileName = "ek.txt";
	
	private static BuildManifest _manifest;

	public static BuildManifest GetBuildManifest()
	{
		if (_manifest == null)
		{
			string key = GetEncryptKey();

			if (string.IsNullOrEmpty(key))
			{
				using (Stream stream = StreamingAssetLoad.GetFile(BuildManifestFileName))
				{
					if (stream == null)
					{
						Debug.LogError("Build Manifest not exists!");
						_manifest = new BuildManifest();
					}

					using (StreamReader sr = new StreamReader(stream))
					{
						string manifestStr = sr.ReadToEnd();
						_manifest = JsonMapper.ToObject<BuildManifest>(manifestStr); 
					}
				}
			}
			else
			{
				string buildManifestStr = GetDecryptedBuildManifestStr(key);
				
				_manifest = JsonMapper.ToObject<BuildManifest>(buildManifestStr); 
			}
		}

		return _manifest;
	}

	public static string GetDecryptedBuildManifestStr(string keyStr)
	{
		string decryptedStr = "";

		using (Stream stream = StreamingAssetLoad.GetFile(EncryptBuildManifestFileName))
		{
			if (stream == null)
			{
				Debug.LogError("Build Manifest not exists!");
				return decryptedStr;
			}

			using (StreamReader sr = new StreamReader(stream))
			{
				DESCryptoServiceProvider cryptoService = new DESCryptoServiceProvider();
				
				string manifestStr = sr.ReadToEnd();
				byte[] data = Convert.FromBase64String(manifestStr);
				byte[] key = Encoding.UTF8.GetBytes(keyStr);
                
				MemoryStream ms = new MemoryStream();
                
				CryptoStream cs = new CryptoStream(ms, cryptoService.CreateDecryptor(key, 
					key), CryptoStreamMode.Write);
                
				cs.Write(data, 0, data.Length);

				cs.FlushFinalBlock();

				decryptedStr = Encoding.UTF8.GetString(ms.ToArray());
                
				//Debug.Log("decrypted str " + decryptedStr);
			}
		}

		return decryptedStr;
	}

	public static string GetEncryptKey()
	{
		string key = "";

#if UNITY_IOS
		using (Stream stream = StreamingAssetLoad.GetFile(EncryptKeyFileName))
		{
			if (stream == null) {
				Debug.Log ("Build Manifest encrypt key not exists!");
			} else {
				using (StreamReader sr = new StreamReader(stream))
				{
					key = sr.ReadToEnd();
				}
			}
		}
#endif
		
		return key;
	}

    public static void Clear()
    {
        _manifest = null;
    }

    /// <summary>
    /// 修改BuildManifest.byte文件中的LocalizationID的值
    /// </summary>
    /// <param name="localizationID"></param>
    public static void ModifyLocalization(uint localizationID)
    {
        GetBuildManifest();
        _manifest.LocalizationID = localizationID;
        string jsonStr = JsonMapper.ToJson(_manifest);
        Stream stream = GetFile(BuildManifestFileName);
        if (stream != null)
        {
            StreamWriter sw = new StreamWriter(stream);
            sw.WriteLine(jsonStr);
            sw.Close();
        }
    }

    /// <summary>
    /// 提供得到一个可写入文件的stream的接口
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static Stream GetFile(string file)
    {
        string fullfile = ResourcesPath.streamingAssetsPath + file;
        if (!File.Exists(fullfile))
        {
            return null;
        }

        try
        {
            return File.Open(fullfile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }
        catch (Exception ex)
        {
            if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer) Debug.LogException(ex);
            return null;
        }
    }
}
