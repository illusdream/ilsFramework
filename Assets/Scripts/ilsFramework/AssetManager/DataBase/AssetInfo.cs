using SQLite4Unity3d;

namespace ilsFramework
{
    public class AssetInfo
    {
        //主键
        [PrimaryKey]
        [AutoIncrement]
        public int ID { get; set; }
        //文件的Unity-GUID
        public string GUID { get; set; }
        
        //外部可以获取到的键名
        [Unique]
        public string AssetKey { get; set; }
        
        public string AssetCollection  { get; set; }
        //描述
        public string AssetDescription { get; set; }

        //是否使用AssetBundle,不使用则为使用Resources文件夹加载
        public bool UseAssetBundle { get; set; }
        /// <summary>
        /// Resources文件夹加载路径
        /// </summary>
        public string ResourcesTargetPath { get; set; }

        public string ResourcePath { get; set; }
        /// <summary>
        /// AssetBundle名字
        /// </summary>
        public string AssetBundleName { get; set; }
        /// <summary>
        /// AssetBundle里该资源的名字
        /// </summary>
        public string AssetName { get; set; }
    }
}