using System.IO;

namespace DLCAssets
{
    /// <summary>
    /// 文件夹的搜索模式,
    /// 搜索文件夹及其子文件夹的所有内容,根据文件搜索模式过滤不属于的文件
    /// 打包规则:搜索  _BuildAsset  文件夹下的所有类型的文件,对这些类型的文件进行打包
    /// 某些类型的文件,必定可以成为一个 AB 包,比如  Hdr Asset  TTF  Shader  Prefab  Spriteatlas  Unity
    /// 剩下类型的文件根据其大小进行 AB 包的融合
    ///
    /// 特殊规则:
    /// 所有的 Bytes Json XML (文本)文件将其压缩到一个压缩包,名字是 all_text.zip
    /// 下载或者解压到 Application.persistentDataPath 文件路径下 直接使用 bytes json xml 为后缀的名字,不再使用压缩包
    /// 
    /// </summary> 
    public static class FileFilter
    {
        //这些格式后缀必须小写
        public static string Png = "*.png";
        public static string Hdr = "*.hdr"; //一个 Hdr(大型图片) 必定是一个 AB 包
        public static string Asset = "*.asset"; //一个 asset(配置文件) 必定是一个 AB 包
        public static string TTF = "*.ttf"; //一个 ttf 必定是一个 AB 包
        public static string Shader = "*.shader"; //所有的 shader 必定是一个 AB 包
        public static string Controller = "*.controller";
        public static string Material = "*.mat";
        public static string Prefab = "*.prefab"; //一个 prefab 必定是一个 AB 包
        public static string Spriteatlas = "*.spriteatlas"; //一个图集必定是一个 AB 包
        public static string Unity = "*.unity"; //一个scene(后缀名为.unity)场景必定是一个 AB 包


        public static string BuildAssets = "BuildAssets"; //一个scene(后缀名为.unity)场景必定是一个 AB 包
        public static string AllText = "Binary.zip"; //所有文本文件的压缩包名字

        /// <summary>
        /// 如果一个资源路径是以 Assets 开头,并且,不是以 .dll .cs .meta .js .boo 结尾
        /// 则此资源可以被认为能打进 AssetBundle 里面
        /// </summary>
        /// <param name="file">资源路径</param>
        /// <returns></returns>
        public static bool QueryFileToAB(string file)
        {
            if (!file.StartsWith("Assets/")) return false;
            string ext = Path.GetExtension(file).ToLower();
            return ext != ".dll" && ext != ".cs" && ext != ".meta" && ext != ".js" && ext != ".boo" && ext != ".zip";
        }
    }
}