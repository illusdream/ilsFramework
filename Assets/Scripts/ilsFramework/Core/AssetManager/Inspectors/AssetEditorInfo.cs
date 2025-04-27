#if UNITY_EDITOR


using System;
using System.Collections.Generic;
using ilsFramework.Core.SQLite4Unity3d;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ilsFramework.Core
{
    [Serializable]
    public class AssetEditorInfo
    {
        [HideInInspector] public int ID;

        [FoldoutGroup("Asset", GroupName = "@FullName")]
        [HorizontalGroup("Asset/Operator", 0.9f)]
        [VerticalGroup("Asset/Operator/Info")]
        [OnValueChanged("UpdateAssetKey")]
        //集合名
        public string AssetCollectionName;

        [OnValueChanged("UpdateAssetKey")] [VerticalGroup("Asset/Operator/Info")]
        //外部可以获取到的键名
        public string AssetKey;


        //键名对应的描述
        [OnValueChanged("UpdateDescription")] [VerticalGroup("Asset/Operator/Info")] [Multiline]
        public string AssetDescription;

        [HideInInspector] public string GUID;

        [HideInInspector] public List<string> allAssetbundleNames;

        [ValueDropdown("GetAllAssetbundleNames")] [OnValueChanged("UpdateAssetBundle")] [VerticalGroup("Asset/Operator/Info")]
        public string AssetBundlesName;

        private bool assetBundlesNameIsDirty;

        private bool assetKeyIsDirty;
        private int assetKeyTimer;

        private bool descriptionIsDirty;
        private int descriptionTimer;
        private string FullName => AssetCollectionName + "." + AssetKey;

        [VerticalGroup("Asset/Operator/Interface")]
        [Button(SdfIconType.FileCode, "复制引用")]
        private void CopyReference()
        {
            var textEditor = new TextEditor();
            textEditor.text = $"{AssetCollectionName}.{AssetKey}";
            textEditor.OnFocus();
            textEditor.Copy();
        }

        [VerticalGroup("Asset/Operator/Interface")]
        [Button(SdfIconType.Link45deg, "跳转至文件")]
        private void PingToTarget()
        {
            var path = AssetDatabase.GUIDToAssetPath(GUID);
            if (!string.IsNullOrEmpty(path))
            {
                // 加载对应的资源
                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (asset != null)
                {
                    // 选中文件
                    Selection.activeObject = asset;

                    // 聚焦到Project窗口
                    EditorGUIUtility.PingObject(asset);

                    // 显示Inspector窗口
                    EditorApplication.ExecuteMenuItem("Window/General/Inspector");
                }
            }
        }

        [VerticalGroup("Asset/Operator/Interface")]
        [Button(SdfIconType.ArrowRepeat, "更新文件")]
        private void ImmatadeRefreshDataBase()
        {
            if (descriptionIsDirty || assetKeyIsDirty || assetBundlesNameIsDirty)
            {
                var assetDataBaseConnection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);

                if (descriptionIsDirty) UpdateDescriptionInfo(assetDataBaseConnection);
                if (assetKeyIsDirty) UpdateAssetKeyInfo(assetDataBaseConnection);
                if (assetBundlesNameIsDirty) InnerUpdateAssetBundle(assetDataBaseConnection);

                assetDataBaseConnection.Close();
            }

            descriptionIsDirty = false;
            descriptionTimer = 0;
            assetKeyIsDirty = false;
            assetKeyTimer = 0;
        }

        public void UpdateDescription()
        {
            descriptionIsDirty = true;
            descriptionTimer = 0;
        }

        public void Update()
        {
            var assetDataBaseConnection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);
            if (descriptionIsDirty)
            {
                descriptionTimer++;
                if (descriptionTimer > 20)
                {
                    descriptionIsDirty = false;
                    descriptionTimer = 0;
                    UpdateDescriptionInfo(assetDataBaseConnection);
                }
            }

            if (assetKeyIsDirty)
            {
                assetKeyTimer++;
                if (assetKeyTimer > 20)
                {
                    assetKeyIsDirty = false;
                    assetKeyTimer = 0;
                    UpdateAssetKeyInfo(assetDataBaseConnection);
                }
            }

            if (assetBundlesNameIsDirty)
            {
                assetBundlesNameIsDirty = false;
                InnerUpdateAssetBundle(assetDataBaseConnection);
            }

            assetDataBaseConnection.Close();
        }

        private void UpdateDescriptionInfo(SQLiteConnection sqLiteConnection)
        {
            if (sqLiteConnection.TryGetTable<AssetInfo>(out var table))
                sqLiteConnection.RunInTransaction(() =>
                {
                    var oldAssetInfo = table.Where(a => a.ID == ID).First();
                    sqLiteConnection.Query<AssetInfo>("UPDATE AssetInfo SET AssetDescription = ? WHERE ID = ?", AssetDescription, ID);
                    var newAssetInfo = table.Where(a => a.ID == ID).First();
                    sqLiteConnection.InsertAll(AssetChangeLog.GetChange(oldAssetInfo, newAssetInfo, new[] { AssetChangeType.ChangeDescription }));
                });
        }

        private void UpdateAssetKey()
        {
            assetKeyIsDirty = true;
            assetKeyTimer = 0;
        }

        private void UpdateAssetKeyInfo(SQLiteConnection sqLiteConnection)
        {
            if (sqLiteConnection.TryGetTable<AssetInfo>(out var table))
                sqLiteConnection.RunInTransaction(() =>
                {
                    ID.LogSelf();
                    var oldAssetInfo = table.Where(a => a.ID == ID).First();
                    sqLiteConnection.Query<AssetInfo>("UPDATE AssetInfo SET AssetKey = ? , AssetCollection = ? WHERE ID = ?",
                        $"{AssetCollectionName}.{AssetKey}", AssetCollectionName, ID);
                    var newAssetInfo = table.Where(a => a.ID == ID).First();
                    sqLiteConnection.InsertAll(AssetChangeLog.GetChange(oldAssetInfo, newAssetInfo,
                        new[] { AssetChangeType.ChangeKeyName, AssetChangeType.ChangeCollection }));
                });
        }

        private void UpdateAssetBundle()
        {
            assetBundlesNameIsDirty = true;
        }

        private void InnerUpdateAssetBundle(SQLiteConnection sqLiteConnection)
        {
            if (sqLiteConnection.TryGetTable<AssetInfo>(out var table))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(GUID);
                var importer = AssetImporter.GetAtPath(assetPath);
                if (importer != null)
                {
                    importer.assetBundleName = AssetBundlesName;
                    AssetDatabase.ImportAsset(assetPath);
                }

                sqLiteConnection.RunInTransaction(() =>
                {
                    var oldAssetInfo = table.Where(a => a.ID == ID).First();
                    sqLiteConnection.Query<AssetInfo>("UPDATE AssetInfo SET AssetBundleName = ? WHERE ID = ?", AssetBundlesName, ID);
                    var newAssetInfo = table.Where(a => a.ID == ID).First();
                    sqLiteConnection.InsertAll(AssetChangeLog.GetChange(oldAssetInfo, newAssetInfo, new[] { AssetChangeType.ChangeAssetBundle }));
                });
            }
        }

        public void RefreshNeedUpdate(SQLiteConnection sqLiteConnection)
        {
            if (descriptionIsDirty)
                UpdateDescriptionInfo(sqLiteConnection);
            if (assetKeyIsDirty)
                UpdateAssetKeyInfo(sqLiteConnection);
            if (assetBundlesNameIsDirty) InnerUpdateAssetBundle(sqLiteConnection);
        }

        private List<string> GetAllAssetbundleNames()
        {
            return allAssetbundleNames;
        }
    }
}
#endif