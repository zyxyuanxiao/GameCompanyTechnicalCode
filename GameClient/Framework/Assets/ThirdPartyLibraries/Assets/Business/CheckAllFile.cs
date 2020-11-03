using System.Collections;

namespace GameAssets
{
    public class CheckAllFile : IBusiness
    {
        public int Progress { get; set; }
        public IEnumerator Work()
        {
            yield return AssetConfig.OneFrame;
        }
    }
}