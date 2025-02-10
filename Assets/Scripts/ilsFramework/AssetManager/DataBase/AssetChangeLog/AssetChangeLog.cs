using System;
using System.Collections.Generic;
using System.Linq;
using SQLite4Unity3d;

namespace ilsFramework
{
    public class AssetChangeLog
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        
        public int AssetID { get; set; }
        
        public AssetChangeType ChangeType { get; set; }
        
        /// <summary>
        /// 这次修改前的值
        /// </summary>
        public string OldValue { get; set; }
        /// <summary>
        /// 这次修改后的值
        /// </summary>
        public string NewValue { get; set; }
        
        public DateTime Date { get; set; }
        
        public string GUID { get; set; }

        public static List<AssetChangeLog> GetChange(AssetInfo old, AssetInfo newValue,IEnumerable<AssetChangeType> needCheck =null)
        {
            List<AssetChangeLog> changes = new List<AssetChangeLog>();
            HashSet<AssetChangeType> _needCheck =needCheck == null ? new HashSet<AssetChangeType>(): needCheck.ToHashSet();
            
            if (old == null || newValue == null)
            {
                return changes;
            }
            
            string[] old_sp = old.AssetKey.Split('.');
            string[] new_sp = newValue.AssetKey.Split('.');
            
            if (old_sp[0] != new_sp[0] && _needCheck.Contains(AssetChangeType.ChangeCollection)) 
            {
                var instance = new AssetChangeLog()
                {
                    ChangeType = AssetChangeType.ChangeCollection,
                    OldValue = old.AssetKey,
                    NewValue = newValue.AssetKey,
                    Date = DateTime.Now,
                    AssetID =old.ID,
                    GUID = old.GUID
                };
                changes.Add(instance);
            }
            if (old_sp[1] != new_sp[1]&& _needCheck.Contains(AssetChangeType.ChangeKeyName)) 
            {
                var instance = new AssetChangeLog()
                {
                    ChangeType = AssetChangeType.ChangeKeyName,
                    OldValue = old.AssetKey,
                    NewValue = newValue.AssetKey,
                    Date = DateTime.Now,
                    AssetID =old.ID,
                    GUID = old.GUID
                };
                changes.Add(instance);
            }
            if (old.AssetDescription != newValue.AssetDescription && _needCheck.Contains(AssetChangeType.ChangeDescription))
            {
                var instance = new AssetChangeLog()
                {
                    ChangeType = AssetChangeType.ChangeDescription,
                    OldValue = old.AssetDescription,
                    NewValue = newValue.AssetDescription,
                    Date = DateTime.Now,
                    AssetID =old.ID,
                    GUID = old.GUID
                };
                changes.Add(instance);
            }

            if (old.UseAssetBundle != newValue.UseAssetBundle&& _needCheck.Contains(AssetChangeType.ChangeUseAssetBundle))
            {
                var instance = new AssetChangeLog()
                {
                    ChangeType = AssetChangeType.ChangeUseAssetBundle,
                    OldValue = old.UseAssetBundle ? "Use" : "NoUse",
                    NewValue = newValue.UseAssetBundle ? "Use" : "NoUse",
                    Date = DateTime.Now,
                    AssetID =old.ID,
                    GUID = old.GUID
                };
                changes.Add(instance);
            }
            if (old.AssetName != newValue.AssetName&& _needCheck.Contains(AssetChangeType.ChangeAssetName))
            {
                var instance = new AssetChangeLog()
                {
                    ChangeType = AssetChangeType.ChangeAssetName,
                    OldValue = old.AssetName,
                    NewValue = newValue.AssetName,
                    Date = DateTime.Now,
                    AssetID =old.ID,
                    GUID = old.GUID
                };
                changes.Add(instance);
            }
            if (old.AssetBundleName != newValue.AssetBundleName&& _needCheck.Contains(AssetChangeType.ChangeAssetBundle))
            {
                var instance = new AssetChangeLog()
                {
                    ChangeType = AssetChangeType.ChangeAssetBundle,
                    OldValue = old.AssetBundleName,
                    NewValue = newValue.AssetBundleName,
                    Date = DateTime.Now,
                    AssetID =old.ID,
                    GUID = old.GUID
                };
                changes.Add(instance);
            }
            if (old.ResourcePath != newValue.ResourcePath && _needCheck.Contains(AssetChangeType.ChangeAssetPath))
            {
                var instance = new AssetChangeLog()
                {
                    ChangeType = AssetChangeType.ChangeAssetPath,
                    OldValue = old.ResourcePath,
                    NewValue = newValue.ResourcePath,
                    Date = DateTime.Now,
                    AssetID =old.ID,
                    GUID = old.GUID
                };
                changes.Add(instance);
            }
            return changes;
        }

        /// <summary>
        /// 修改Key名，只可在AssetManagerConfig修改
        /// </summary>
        public static AssetChangeLog Create_ChangeKeyName(int assetID, string oldKeyName, string newKeyName,string GUID)
        {
            return new AssetChangeLog()
            {
                AssetID = assetID,
                GUID = GUID,
                OldValue = oldKeyName,
                NewValue = newKeyName,
                Date = DateTime.Now
            };
        }
        /// <summary>
        /// 修改所属引用集合名，只可在AssetManagerConfig修改
        /// </summary>
        public static AssetChangeLog Create_ChangeCollection(int assetID, string oldCollectionName, string newCollectionName,string GUID)
        {
            return new AssetChangeLog()
            {
                AssetID = assetID,
                GUID = GUID,
                OldValue = oldCollectionName,
                NewValue = newCollectionName,
                Date = DateTime.Now
            };
        }
        /// <summary>
        /// 修改对应描述，只可在AssetManagerConfig修改
        /// </summary>
        public static AssetChangeLog Create_ChangeDescription(int assetID, string oldDescription, string newDescription,string GUID)
        {
            return new AssetChangeLog()
            {
                AssetID = assetID,
                GUID = GUID,
                OldValue = oldDescription,
                NewValue = newDescription,
                Date = DateTime.Now
            };
        }
        /// <summary>
        /// 新增资源
        /// </summary>
        public static AssetChangeLog Create_AssetImport(int assetID, string assetPath,string assetKey,string GUID)
        {
            return new AssetChangeLog()
            {
                AssetID = assetID,
                GUID = GUID,
                OldValue = assetKey,
                NewValue = assetPath,
                Date = DateTime.Now
            };
        }
        /// <summary>
        /// 资源移除,(需要额外遍历来寻找是否有不存在的GUID)
        /// </summary>
        public static AssetChangeLog Create_AssetRemove(int assetID, string assetPath,string assetKey,string GUID)
        {
            return new AssetChangeLog()
            {
                AssetID = assetID,
                GUID = GUID,
                OldValue = assetPath,
                NewValue = assetKey,
                Date = DateTime.Now
            };
        }
        /// <summary>
        /// 修改是否使用AssetBundle,可在Unity编辑器/AssetMangerConfig修改
        /// </summary>
        public static AssetChangeLog Create_ChangeUseAssetBundle(int assetID, bool oldUseAssetBundle, bool newUseAssetBundle,string GUID)
        {
            return new AssetChangeLog()
            {
                AssetID = assetID,
                GUID = GUID,
                OldValue = oldUseAssetBundle ? "Use" : "NoUse",
                NewValue = newUseAssetBundle ? "Use" : "NoUse",
                Date = DateTime.Now
            };
        }
        /// <summary>
        /// 修改所属AssetBundle包,可在Unity编辑器/AssetMangerConfig修改
        /// </summary>
        public static AssetChangeLog Create_ChangeAssetBundle(int assetID, string oldAssetBundle, string newAssetBundle,string GUID)
        {
            return new AssetChangeLog()
            {
                AssetID = assetID,
                GUID = GUID,
                OldValue = oldAssetBundle,
                NewValue = newAssetBundle,
                Date = DateTime.Now
            };
        }
        /// <summary>
        /// 修改资源名(文件名/AssetBundle包中的名字),可在Unity编辑器修改
        /// </summary>
        public static AssetChangeLog Create_ChangeAssetName(int assetID, string oldAssetName, string newAssetName,string GUID)
        {
            return new AssetChangeLog()
            {
                AssetID = assetID,
                GUID = GUID,
                OldValue = oldAssetName,
                NewValue = newAssetName,
                Date = DateTime.Now
            };
        }
        /// <summary>
        /// 修改资源地址,可在Unity编辑器修改
        /// </summary>
        public static AssetChangeLog Create_ChangeAssetPath(int assetID, string oldAssetPath, string newAssetPath,string GUID)
        {
            return new AssetChangeLog()
            {
                AssetID = assetID,
                GUID = GUID,
                OldValue = oldAssetPath,
                NewValue = newAssetPath,
                Date = DateTime.Now
            };
        }
        public static SQLiteConnection GetConnect()
        {
            var assetDataBaseConnection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);
            if (assetDataBaseConnection == null)
            {
                throw new NullReferenceException("AssetDataBaseConnection is null");
            }
            if (!assetDataBaseConnection.TryGetTable<AssetChangeLog>(out var table))
            {
                assetDataBaseConnection.CreateTable<AssetChangeLog>();
            }
            return assetDataBaseConnection;
        }
    }
}