using System.Collections.Generic;

namespace GameAssets
{
    /// <summary>
    /// 没有热更,不需要这个模块的资源加载
    /// 只需要把所有的东西都塞进 Resources/StreamingAssets 里面就行了,根本没有必要用到资源管理.
    /// 
    /// </summary>
    public class HotUpdate
    {
        private static List<IBusiness> _huBusiness = new List<IBusiness>()
        {
            new ReadConfig(),
            new DownloadAssets(),
            new UnZipFiles(),
            new LoadAllFiles(),
            new CheckAllFile()
        };
        
        public void Awake()
        {
            
        }

        public void Start()
        {

        }

        public void OnDestroy()
        {
            
        }

        public void Update()
        {

        }

        public static IBusiness QueryBusiness(int index)
        {
            return _huBusiness[index];
        }
        
    }
}