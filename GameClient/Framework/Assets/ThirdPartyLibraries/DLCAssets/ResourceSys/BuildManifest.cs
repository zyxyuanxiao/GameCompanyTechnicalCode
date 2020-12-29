/********************************************************************
	created:	2017/00/02  11:00
	file base:	BuildManifest
	file ext:	cs
	author:		wuzhou

	purpose:    Define the features of every build. this mainfest will not
				be modified after the build.
*********************************************************************/

namespace Best
{
    public class BuildManifest
    {
        /// <summary>
        /// Is release version
        /// </summary>
        public bool IsReleaseVer;

        /// <summary>
        /// Whether this build has debug log out put
        /// </summary>
        public bool HasDebugLogOutput;

        /// <summary>
        /// 是否属于边玩边下载
        /// </summary>
        public bool isPlayingDownLoad;

        /// <summary>
        /// 是否默认暂停下载
        /// </summary>
        public bool IsPlayingDownLoadPause;

        /// <summary>
        /// 是否大小服
        /// </summary>
        public bool IsSmallServer;

        /// <summary>
        /// 语言ID
        /// </summary>
        public uint LocalizationID;

	    /// <summary>
	    /// 判断渠道
	    /// </summary>
	    public string SDKTag;

		/// <summary>
		/// 是否是 AppStore 版本
		/// </summary>
		public bool IsAppStoreVer;

        /// <summary>
        /// 打包时间 
        /// </summary>
        public string BuildTime;
    }
}