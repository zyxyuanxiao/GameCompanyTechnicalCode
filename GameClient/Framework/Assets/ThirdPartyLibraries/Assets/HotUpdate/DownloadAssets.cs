using System.Collections;

namespace GameAssets
{
    public class DownloadAssets : IBusiness
    {
        public int Progress { get; set; }
        public IEnumerator Work()
        {
            yield return AssetsConfig.OneFrame;
        }
    }
}