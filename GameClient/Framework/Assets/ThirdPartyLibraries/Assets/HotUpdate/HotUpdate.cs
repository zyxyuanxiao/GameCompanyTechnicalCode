using System.Collections.Generic;

namespace GameAssets
{
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