using System.Collections;

namespace GameAssets
{
    public class UnZipFiles : IBusiness
    {
        public int Progress { get; set; }
        public IEnumerator Work()
        {
            yield return AssetsConfig.OneFrame;
        }
    }
}