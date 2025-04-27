using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ilsFramework.Core.SQLite4Unity3d;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ilsFramework.Core
{
    [AutoBuildOrLoadConfig("Asset")]
    public partial class AssetConfig : ConfigScriptObject
    {
        public const string DefaultAssetBundleName = "Default";
        public bool AutoTracingAsset = true;

        [LabelText("AssetKey过滤选项")] [Searchable] [ListDrawerSettings(ShowFoldout = true, ListElementLabelName = "FilterName")]
        public List<AssetFilter> AssetFilters = new();

        public HashSet<string> assetCollectionNames;

        public override string ConfigName => "Asset";

#if UNITY_EDITOR
        [Button]
        public void Refresh()
        {
            assetCollectionNames ??= new HashSet<string>();
            assetCollectionNames.Clear();
            CheckDataBaseActive(out var connection);

            foreach (var filter in AssetFilters)
            {
                if (!assetCollectionNames.Add(filter.AssetKeyCollectionName))
                    throw new Exception($"修改Filter:{filter.FilterName},已有相同名称的AssetKeyCollection:{filter.AssetKeyCollectionName}");
                filter.GetCanUsedAsset(connection);
            }

            CheckCollectionValid(connection);

            UpdateAllAssetKey(connection);
            connection.Close();
        }

        public void UpdateAllAssetKey(SQLiteConnection connection)
        {
            var st = new StackTrace(0, true);
            var path = Path.GetDirectoryName(st.GetFrame(0).GetFileName()) + @"\Collections";
            path = path.Replace(@"\", "/");
            path = path.Substring(Application.dataPath.Length - "Assets".Length);
            foreach (var guid in AssetDatabase.FindAssets("", new[] { path }))
            {
                var cPath = AssetDatabase.GUIDToAssetPath(guid);
                AssetDatabase.DeleteAsset(cPath);
            }

            var dic = new Dictionary<string, List<(string, string)>>();

            var all = connection.Table<AssetInfo>().Where(_ => true).ToList();

            foreach (var assetInfo in all)
            {
                var kp = assetInfo.AssetKey.Split('.');
                if (dic.TryGetValue(kp[0], out var list))
                    list.Add((kp[1], assetInfo.AssetDescription));
                else
                    dic.Add(kp[0], new List<(string, string)> { (kp[1], assetInfo.AssetDescription) });
            }

            foreach (var collection in dic)
            {
                var scriptGenerator = new ScriptGenerator("ilsFrameWork");
                var classGenerator = new ClassGenerator(EAccessType.Public, collection.Key);
                scriptGenerator.Append(classGenerator);

                foreach (var assetKey in collection.Value)
                {
                    var stringFieldGenerator = new StringFieldGenerator(
                        EFieldDeclarationMode.Const,
                        EAccessType.Public,
                        assetKey.Item1,
                        $"{collection.Key}.{assetKey.Item1}",
                        assetKey.Item2);
                    classGenerator.Append(stringFieldGenerator);
                }

                GetAssetCollectionPath(collection.Key, out var folderPath, out var fileName);
                scriptGenerator.GenerateScript(fileName, folderPath);
                AssetDatabase.Refresh();
            }
        }

        public void GetAssetCollectionPath(string assetCollectionName, out string folderPath, out string assetName)
        {
            var st = new StackTrace(0, true);
            var path = Path.GetDirectoryName(st.GetFrame(0).GetFileName()) + @"\Collections";
            folderPath = path;
            assetName = assetCollectionName + "Collection";
        }

        public void CheckCollectionValid(SQLiteConnection connection)
        {
            var activeCollections = new HashSet<string>();
            //检查目前Collection状态
            foreach (var filter in AssetFilters) activeCollections.Add(filter.AssetKeyCollectionName);
            //删除不存在当下的Collection
            var needRemove = connection.Table<AssetInfo>().Where(a => !activeCollections.Contains(a.AssetCollection)).ToList();
            foreach (var assetInfo in needRemove) connection.Execute("DELETE From AssetInfo WHERE ID=?", assetInfo.ID);
        }

        public HashSet<string> GetTargetAssetCollectionNames(string path)
        {
            var result = new HashSet<string>();

            foreach (var filter in AssetFilters)
                if (filter.IsTargetAsset(path))
                    result.Add(filter.AssetKeyCollectionName);

            return result;
        }

        private void CheckDataBaseActive(out SQLiteConnection connection)
        {
            //用于防止数据库不存在
            var assetDataBaseConnection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);
            if (!assetDataBaseConnection.TryGetTable<AssetInfo>(out var table1)) assetDataBaseConnection.CreateTable<AssetInfo>();
            if (!assetDataBaseConnection.TryGetTable<AssetChangeLog>(out var table2)) assetDataBaseConnection.CreateTable<AssetChangeLog>();
            connection = assetDataBaseConnection;
        }

#endif
    }


    [Serializable]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout, Expanded = true)]
    public class AssetFilter
    {
        [LabelText("过滤器名称")] public string FilterName;

        [LabelText("引用文件名")] [InfoBox("不建议使用中文/第一位不能是数字/不能是特殊字符")]
        public string AssetKeyCollectionName;

        [Title("目标文件夹")] [ShowInInspector] public List<string> TargetFolders;

        [Title("目标类型")] [Space(10)] [ShowInInspector]
        public List<string> TargetTypes;

        [Title("需要忽略的文件或文件夹")] [Space(10)] [ShowInInspector]
        public List<string> Ignore;


#if UNITY_EDITOR
        [Button(ButtonSizes.Large, DrawResult = false)]
        public void GetCanUsedAsset(SQLiteConnection assetDataBaseConnection)
        {
            var exsitGUIDs = new HashSet<string>();
            var filter = new StringBuilder();
            foreach (var type in TargetTypes)
            {
                filter.Append($"t:{type}");
                filter.Append(" ");
            }

            foreach (var targetFolder in TargetFolders)
                //文件夹或文件判断
                if (targetFolder.EndsWith("/"))
                {
                    var GUIDs = AssetDatabase.FindAssets(filter.ToString(), new[] { targetFolder });
                    foreach (var guid in GUIDs) UpdateSingleAsset(exsitGUIDs, assetDataBaseConnection, guid);
                }
                else
                {
                    var guid = AssetDatabase.AssetPathToGUID(targetFolder);
                    if (!string.IsNullOrEmpty(guid)) UpdateSingleAsset(exsitGUIDs, assetDataBaseConnection, guid);
                }

            DeleteNullReferenceKeys(exsitGUIDs, assetDataBaseConnection);
        }

        public void UpdateSingleAsset(HashSet<string> exsitGUIDs, SQLiteConnection connection, string GUID)
        {
            exsitGUIDs.Add(GUID);
            var assetPath = AssetDatabase.GUIDToAssetPath(GUID);
            var resourceParten = "^Assets/Resources/";

            var importer = AssetImporter.GetAtPath(assetPath);
            var inReourcesFolder = Regex.IsMatch(importer.assetPath, resourceParten);

            var useAssetBundle = !inReourcesFolder;
            if (useAssetBundle)
            {
                if (importer.assetBundleName == string.Empty) importer.assetBundleName = AssetConfig.DefaultAssetBundleName;
            }
            else
            {
                importer.assetBundleName = string.Empty;
            }

            var AssetName = useAssetBundle ? Path.GetFileNameWithoutExtension(assetPath) : string.Empty;
            var ResourcesTargetPath = useAssetBundle
                ? string.Empty
                : assetPath.Replace("Assets/Resources/", string.Empty).Replace(Path.GetExtension(assetPath), string.Empty);

            //先判断数据库里是否包括这个数据
            var list = connection.Table<AssetInfo>().Where(a => a.GUID == GUID && a.AssetCollection == AssetKeyCollectionName).ToList();
            switch (list.Count)
            {
                //没有这个Asset
                case 0:
                {
                    var addAssetInfo = new AssetInfo
                    {
                        GUID = GUID,
                        AssetCollection = AssetKeyCollectionName,
                        AssetKey = $"{AssetKeyCollectionName}.{Path.GetFileNameWithoutExtension(assetPath)}",
                        UseAssetBundle = useAssetBundle,
                        AssetBundleName = importer.assetBundleName,
                        AssetName = AssetName,
                        ResourcesTargetPath = ResourcesTargetPath,
                        ResourcePath = assetPath
                    };
                    connection.RunInTransaction(() =>
                    {
                        connection.Insert(addAssetInfo);
                        var assst = connection.Table<AssetInfo>().Where(a => a.GUID == GUID && a.AssetCollection == AssetKeyCollectionName).First();
                        connection.Insert(AssetChangeLog.Create_AssetImport(assst.ID, assst.ResourcePath, assst.AssetKey, assst.GUID));
                    });
                }
                    return;
                //有这个Asset
                case 1:
                {
                    var oldAsset = list.First();
                    if (oldAsset.ResourcePath != assetPath)
                        connection.Insert(AssetChangeLog.Create_ChangeAssetPath(oldAsset.ID, oldAsset.ResourcePath, assetPath, oldAsset.GUID));

                    if (oldAsset.AssetName != AssetName)
                        connection.Insert(AssetChangeLog.Create_ChangeAssetName(oldAsset.ID, oldAsset.AssetName, assetPath, oldAsset.GUID));

                    if (oldAsset.UseAssetBundle != useAssetBundle)
                        connection.Insert(AssetChangeLog.Create_ChangeUseAssetBundle(oldAsset.ID, oldAsset.UseAssetBundle, useAssetBundle, oldAsset.GUID));

                    if (oldAsset.AssetBundleName != importer.assetBundleName)
                        connection.Insert(AssetChangeLog.Create_ChangeAssetBundle(oldAsset.ID, oldAsset.AssetBundleName, importer.assetBundleName,
                            oldAsset.GUID));
                    var result = connection.Execute
                    (
                        "UPDATE AssetInfo SET ResourcePath =?, UseAssetBundle =?, AssetBundleName = ?, AssetName = ? , ResourcesTargetPath = ? WHERE GUID = ?",
                        assetPath, useAssetBundle, importer.assetBundleName, AssetName, ResourcesTargetPath, GUID
                    );
                }
                    return;
                default:
                    $"数据库数据异常: 检查: GUID:{GUID} AssetCollection:{AssetKeyCollectionName}".WarningSelf();
                    return;
            }
        }

        public void DeleteNullReferenceKeys(HashSet<string> exsitGUIDs, SQLiteConnection connection)
        {
            var needRemove = connection.Table<AssetInfo>().Where(a => a.AssetCollection == AssetKeyCollectionName && !exsitGUIDs.Contains(a.GUID)).ToList();
            foreach (var assetInfo in needRemove)
            {
                connection.Insert(AssetChangeLog.Create_AssetRemove(assetInfo.ID, assetInfo.ResourcePath, assetInfo.AssetKey, assetInfo.GUID));
                connection.Execute("DELETE FROM AssetInfo WHERE GUID = ? AND AssetCollection =?", assetInfo.AssetKey, AssetKeyCollectionName);
            }
        }

        public bool IsTargetAsset(string path)
        {
            var types = TargetTypes.ToHashSet();
            //检查文件夹是否符合要求(目标检查，忽略检查)
            var isTarget = false;
            foreach (var target in TargetFolders)
            {
                var parten = $@"^{Regex.Escape(target)}";
                bool _isTarget;
                if (target.EndsWith("/"))
                    _isTarget = Regex.IsMatch(path, parten);
                else
                    _isTarget = target == path;
                if (_isTarget)
                {
                    isTarget = true;
                    break;
                }
            }

            var IsIgnore = false;
            foreach (var i in Ignore)
            {
                var parten = $@"^{Regex.Escape(i)}";
                bool canIgnore;
                if (i.EndsWith("/"))
                    canIgnore = Regex.IsMatch(path, parten);
                else
                    canIgnore = i == path;
                if (canIgnore)
                {
                    IsIgnore = true;
                    break;
                }
            }

            //检查类型是否符合要求
            var IsTargetType = types.Contains(AssetDatabase.GetMainAssetTypeAtPath(path).Name);

            return isTarget && !IsIgnore && IsTargetType;
        }

#endif
    }
}