/********************************************************************
	created:	2015/12/20  15:44
	file base:	AssetBundleExportType
	file ext:	cs
	author:		luke
	
	purpose:	定义了资源类型，需要被打成资源包的条件
*********************************************************************/

using System.IO;

namespace Best
{
    /// <summary>
    /// 导出AB的条件
    /// </summary>
    [System.Serializable]
    public struct ExportABTerms
    {
        /// <summary>
        /// 记录需要打包的资源的路径
        /// </summary>
        public string AssetDir;

        /// <summary>
        /// 记录需要打包的过滤条件，控制包的精细度
        /// </summary>
        public string[] Filter;

        /// <summary>
        /// 根据上述Filter的过滤的资源的依赖资源中如果符合SubalternFilter后缀条件的也需要打包
        /// </summary>
        public string[] SubalternFilterSuffixs;
    }
}
