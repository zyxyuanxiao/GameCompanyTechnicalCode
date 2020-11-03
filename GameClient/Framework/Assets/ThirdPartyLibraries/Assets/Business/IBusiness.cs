using System.Collections;

namespace GameAssets
{
    public interface IBusiness
    {
        int Progress { get; set; }
        IEnumerator Work();
    }
}