using Sirenix.OdinInspector;

namespace ilsFramework
{
    public enum AssetChangeType
    {
        /// <summary>
        /// 修改Key名，只可在AssetManagerConfig修改
        /// </summary>
        ChangeKeyName,
        /// <summary>
        /// 修改所属引用集合名，只可在AssetManagerConfig修改
        /// </summary>
        ChangeCollection,
        /// <summary>
        /// 修改对应描述，只可在AssetManagerConfig修改
        /// </summary>
        ChangeDescription,
        /// <summary>
        /// 新增资源
        /// </summary>
        AssetImport,
        /// <summary>
        /// 资源移除,(需要额外遍历来寻找是否有不存在的GUID)
        /// </summary>
        AssetRemove,
        /// <summary>
        /// 修改是否使用AssetBundle,可在Unity编辑器/AssetMangerConfig修改
        /// </summary>
        ChangeUseAssetBundle,
        /// <summary>
        /// 修改所属AssetBundle包,可在Unity编辑器/AssetMangerConfig修改
        /// </summary>
        ChangeAssetBundle,
        /// <summary>
        /// 修改资源名(文件名/AssetBundle包中的名字),可在Unity编辑器修改
        /// </summary>
        ChangeAssetName,
        /// <summary>
        /// 修改资源地址,可在Unity编辑器修改
        /// </summary>
        ChangeAssetPath
    }
}