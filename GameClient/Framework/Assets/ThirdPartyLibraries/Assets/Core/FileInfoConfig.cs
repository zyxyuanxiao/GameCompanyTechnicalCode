namespace GameAssets
{
    /// <summary>
    /// 热更时,对比这些数据,几乎可以防止任何人更改热更数据包.
    /// </summary>
    public class FileInfoConfig
    {
        public string Name;//名字
        public string MD5Hash;//校验值
        public long Length;//文件大小
        public string LastWriteTime;//最后一次写入的时间
    }
}