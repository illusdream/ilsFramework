#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ilsFramework.Core.SQLite4Unity3d;
using UnityEditor;

namespace ilsFramework.Core
{
    /// <summary>
    ///     自动化处理文件操作
    /// </summary>
    public class ilsAssetPostProcessor : AssetPostprocessor
    {
        public static int test;

        private static Core.AssetConfig _config;

        public ilsAssetPostProcessor()
        {
            CheckDataBaseActive();
        }

        private static Core.AssetConfig config
        {
            get
            {
                if (_config == null)
                    _config = AssetDatabase.LoadAssetAtPath(
                        "Assets/Resources/ilsFramework/Configs/" + typeof(Core.AssetConfig).GetCustomAttribute<AutoBuildOrLoadConfig>().ConfigTargetPath + ".asset",
                        typeof(Core.AssetConfig)) as Core.AssetConfig;
                return _config;
            }
        }

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (!config.AutoTracingAsset) return;
            CheckDataBaseActive();
            var assetDataBaseConnection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);
            config.CheckCollectionValid(assetDataBaseConnection);
            //收集需要重新构建的AssetCollection引用文件
            var needReBuildAssetCollections = new HashSet<string>();

            //处理Import 主要是新增的Asset 
            ProcessImportedAssets(importedAssets, assetDataBaseConnection, ref needReBuildAssetCollections);
            //处理删除的文件
            ProcessDeletedAssets(deletedAssets, assetDataBaseConnection, ref needReBuildAssetCollections);
            //处理移动的文件（包括文件名改动和文件位置变化）
            ProcessMovedAssets(movedAssets, movedFromAssetPaths, assetDataBaseConnection, ref needReBuildAssetCollections);
            //重新构建AssetCollection引用文件
            RebuildAssetCollections(needReBuildAssetCollections, assetDataBaseConnection);
            assetDataBaseConnection.Close();
        }

        //侦测AssetBundle的变化
        public void OnPostprocessAssetbundleNameChanged(string assetPath, string previousAssetBundleName, string newAssetBundleName)
        {
            var assetDataBaseConnection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);
            var GUID = AssetDatabase.AssetPathToGUID(assetPath);

            var resourceParten = "^Assets/Resources/";

            var inReourcesFolder = Regex.IsMatch(assetPath, resourceParten);
            var useAssetBundle = newAssetBundleName != string.Empty && !inReourcesFolder;
            var AssetName = useAssetBundle ? Path.GetFileNameWithoutExtension(assetPath) : string.Empty;
            var ResourcesTargetPath = useAssetBundle
                ? string.Empty
                : assetPath.Replace("Assets/Resources/", string.Empty).Replace(Path.GetExtension(assetPath), string.Empty);

            foreach (var assetInfo in assetDataBaseConnection.Table<AssetInfo>().Where(a => a.GUID == GUID))
            {
                if (assetInfo.UseAssetBundle != useAssetBundle)
                    assetDataBaseConnection.Insert(AssetChangeLog.Create_ChangeUseAssetBundle(
                        assetInfo.ID,
                        assetInfo.UseAssetBundle,
                        useAssetBundle,
                        GUID
                    ));

                if (assetInfo.AssetName != AssetName)
                    assetDataBaseConnection.Insert(AssetChangeLog.Create_ChangeAssetName(
                        assetInfo.ID,
                        assetInfo.AssetName,
                        AssetName,
                        GUID));


                assetDataBaseConnection.Insert(AssetChangeLog.Create_ChangeAssetBundle(
                    assetInfo.ID,
                    previousAssetBundleName,
                    newAssetBundleName,
                    GUID
                ));
            }


            var result = assetDataBaseConnection.Execute
            (
                "UPDATE AssetInfo SET UseAssetBundle =?, AssetBundleName = ?, AssetName = ? , ResourcesTargetPath = ? WHERE GUID = ?",
                useAssetBundle, newAssetBundleName, AssetName, ResourcesTargetPath, GUID
            );
            assetDataBaseConnection.Close();
        }

        /// <summary>
        ///     检查数据库是否可用，若不可用则重写生成数据库
        /// </summary>
        private static void CheckDataBaseActive()
        {
            //用于防止数据库不存在
            var assetDataBaseConnection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);
            if (!assetDataBaseConnection.TryGetTable<AssetInfo>(out var table1)) assetDataBaseConnection.CreateTable<AssetInfo>();
            if (!assetDataBaseConnection.TryGetTable<AssetChangeLog>(out var table2)) assetDataBaseConnection.CreateTable<AssetChangeLog>();
            assetDataBaseConnection.Close();
        }

        /// <summary>
        ///     处理Import 主要是新增的Asset
        /// </summary>
        /// <param name="importedAssets">Import文件</param>
        /// <param name="assetDataBaseConnection">Sql链接</param>
        /// <param name="needReBuildAssetCollections">需要重构的AssetCollection</param>
        private static void ProcessImportedAssets(string[] importedAssets, SQLiteConnection assetDataBaseConnection,
            ref HashSet<string> needReBuildAssetCollections)
        {
            var resourceParten = "^Assets/Resources/";
            foreach (var importedAsset in importedAssets)
            {
                var targetAssetKeyCollection = config.GetTargetAssetCollectionNames(importedAsset);

                if (targetAssetKeyCollection.Count != 0)
                    "InCollection".LogSelf();
                else
                    return;

                var GUID = AssetDatabase.AssetPathToGUID(importedAsset);

                var importer = AssetImporter.GetAtPath(importedAsset);
                var inReourcesFolder = Regex.IsMatch(importer.assetPath, resourceParten);

                var list = assetDataBaseConnection
                    .Table<AssetInfo>()
                    .Where(a => a.GUID == GUID)
                    .ToList();
                //获取差集和交集
                var needAdd = targetAssetKeyCollection.Where(akc => !list.Exists(a => a.AssetCollection == akc));

                var useAssetBundle = !inReourcesFolder;
                if (useAssetBundle)
                {
                    if (importer.assetBundleName == string.Empty) importer.assetBundleName = Core.AssetConfig.DefaultAssetBundleName;
                }
                else
                {
                    importer.assetBundleName = string.Empty;
                }

                var AssetName = useAssetBundle ? Path.GetFileNameWithoutExtension(importedAsset) : string.Empty;
                var ResourcesTargetPath = useAssetBundle
                    ? string.Empty
                    : importedAsset.Replace("Assets/Resources/", string.Empty).Replace(Path.GetExtension(importedAsset), string.Empty);

                //添加新引用
                foreach (var addAssetCollection in needAdd)
                {
                    var addAssetInfo = new AssetInfo
                    {
                        GUID = GUID,
                        AssetCollection = addAssetCollection,
                        AssetKey = $"{addAssetCollection}.{Path.GetFileNameWithoutExtension(importedAsset)}",
                        UseAssetBundle = useAssetBundle,
                        AssetBundleName = importer.assetBundleName,
                        AssetName = AssetName,
                        ResourcesTargetPath = ResourcesTargetPath,
                        ResourcePath = importedAsset
                    };
                    assetDataBaseConnection.Insert(addAssetInfo);
                    needReBuildAssetCollections.Add(addAssetCollection);

                    var assst = assetDataBaseConnection.Table<AssetInfo>().Where(a => a.GUID == GUID && a.AssetCollection == addAssetCollection).First();
                    //生成Log
                    assetDataBaseConnection.Insert(AssetChangeLog.Create_AssetImport(assst.ID, assst.ResourcePath, assst.AssetKey, assst.GUID));
                }
            }
        }

        /// <summary>
        ///     处理删除的文件
        /// </summary>
        /// <param name="deletedAssets">Delete文件</param>
        /// <param name="assetDataBaseConnection">Sql链接</param>
        /// <param name="needReBuildAssetCollections">需要重构的AssetCollection</param>
        private static void ProcessDeletedAssets(string[] deletedAssets, SQLiteConnection assetDataBaseConnection,
            ref HashSet<string> needReBuildAssetCollections)
        {
            //删除对应的数据库数据，并返回重新编译的部分
            foreach (var deletedAsset in deletedAssets)
            {
                var GUID = AssetDatabase.AssetPathToGUID(deletedAsset);
                //没被录入
                if (assetDataBaseConnection.Table<AssetInfo>().Count(a => a.GUID == GUID) <= 0) continue;

                //添加Log
                foreach (var assetInfo in assetDataBaseConnection.Table<AssetInfo>().Where(a => a.GUID == GUID))
                {
                    assetDataBaseConnection.Insert(AssetChangeLog.Create_AssetRemove(assetInfo.ID, assetInfo.ResourcePath, assetInfo.AssetKey, GUID));
                    needReBuildAssetCollections.Add(assetInfo.AssetCollection);
                }

                assetDataBaseConnection.Execute(
                    "DELETE FROM AssetInfo WHERE GUID=? ",
                    GUID
                );
            }
        }

        /// <summary>
        ///     处理移动的文件（包括文件名改动和文件位置变化）
        /// </summary>
        /// <param name="movedAssets">移动的文件</param>
        /// <param name="movedFromAssetPaths">文件从哪里移动过来的</param>
        /// <param name="assetDataBaseConnection">Sql链接</param>
        /// <param name="needReBuildAssetCollections">需要重构的AssetCollection</param>
        private static void ProcessMovedAssets(string[] movedAssets, string[] movedFromAssetPaths, SQLiteConnection assetDataBaseConnection,
            ref HashSet<string> needReBuildAssetCollections)
        {
            var resourceParten = "^Assets/Resources/";
            for (var i = 0; i < movedAssets.Length; i++)
            {
                var moveInfo = (movedAssets[i], movedFromAssetPaths[i]);
                var GUID = AssetDatabase.AssetPathToGUID(moveInfo.Item1);

                //没被录入
                if (assetDataBaseConnection.Table<AssetInfo>().Count(a => a.GUID == GUID) <= 0) continue;

                var importer = AssetImporter.GetAtPath(moveInfo.Item1);
                var inReourcesFolder = Regex.IsMatch(importer.assetPath, resourceParten);

                var useAssetBundle = !inReourcesFolder;
                if (useAssetBundle)
                {
                    if (importer.assetBundleName == string.Empty) importer.assetBundleName = Core.AssetConfig.DefaultAssetBundleName;
                }
                else
                {
                    importer.assetBundleName = string.Empty;
                }

                var AssetName = useAssetBundle ? Path.GetFileNameWithoutExtension(moveInfo.Item1) : string.Empty;
                var ResourcesTargetPath = useAssetBundle
                    ? string.Empty
                    : moveInfo.Item1.Replace("Assets/Resources/", string.Empty).Replace(Path.GetExtension(moveInfo.Item1), string.Empty);

                //添加Log
                foreach (var assetInfo in assetDataBaseConnection.Table<AssetInfo>().Where(a => a.GUID == GUID))
                {
                    var changeLog1 = new AssetChangeLog
                    {
                        AssetID = assetInfo.ID,
                        ChangeType = AssetChangeType.ChangeAssetPath,
                        Date = DateTime.Now,
                        GUID = GUID,
                        NewValue = moveInfo.Item1,
                        OldValue = moveInfo.Item2
                    };
                    assetDataBaseConnection.Insert(changeLog1);

                    if (assetInfo.UseAssetBundle != useAssetBundle)
                    {
                        var changeLog2 = new AssetChangeLog
                        {
                            AssetID = assetInfo.ID,
                            ChangeType = AssetChangeType.ChangeUseAssetBundle,
                            Date = DateTime.Now,
                            GUID = GUID,
                            OldValue = assetInfo.UseAssetBundle ? "Use" : "NoUse",
                            NewValue = useAssetBundle ? "Use" : "NoUse"
                        };
                        assetDataBaseConnection.Insert(changeLog2);
                    }

                    if (assetInfo.AssetName != AssetName)
                    {
                        var changeLog2 = new AssetChangeLog
                        {
                            AssetID = assetInfo.ID,
                            ChangeType = AssetChangeType.ChangeAssetName,
                            Date = DateTime.Now,
                            GUID = GUID,
                            OldValue = assetInfo.AssetName,
                            NewValue = AssetName
                        };
                        assetDataBaseConnection.Insert(changeLog2);
                    }

                    if (assetInfo.AssetBundleName != importer.assetBundleName)
                    {
                        var changeLog2 = new AssetChangeLog
                        {
                            AssetID = assetInfo.ID,
                            ChangeType = AssetChangeType.ChangeAssetBundle,
                            Date = DateTime.Now,
                            GUID = GUID,
                            OldValue = assetInfo.AssetBundleName,
                            NewValue = importer.assetBundleName
                        };
                        assetDataBaseConnection.Insert(changeLog2);
                    }
                }

                var result = assetDataBaseConnection.Execute
                (
                    "UPDATE AssetInfo SET ResourcePath =?, UseAssetBundle =?, AssetBundleName = ?, AssetName = ? , ResourcesTargetPath = ? WHERE GUID = ?",
                    moveInfo.Item1, useAssetBundle, importer.assetBundleName, AssetName, ResourcesTargetPath, GUID
                );


                $"Move : {moveInfo.Item2} -> {moveInfo.Item1} ".LogSelf();
            }
        }

        /// <summary>
        ///     重新构建AssetCollection引用文件
        /// </summary>
        /// <param name="needReBuildAssetCollections">需要重构的AssetCollection</param>
        /// <param name="assetDataBaseConnection">Sql链接</param>
        private static void RebuildAssetCollections(HashSet<string> needReBuildAssetCollections, SQLiteConnection assetDataBaseConnection)
        {
            foreach (var collection in needReBuildAssetCollections)
            {
                var scriptGenerator = new ScriptGenerator("ilsFrameWork");
                var classGenerator = new ClassGenerator(EAccessType.Public, collection);
                scriptGenerator.Append(classGenerator);

                foreach (var assetInfo in assetDataBaseConnection.Table<AssetInfo>().Where(a => a.AssetCollection == collection))
                {
                    var stringFieldGenerator = new StringFieldGenerator(
                        EFieldDeclarationMode.Const,
                        EAccessType.Public,
                        assetInfo.AssetKey.Replace(assetInfo.AssetCollection + ".", string.Empty),
                        assetInfo.AssetKey,
                        assetInfo.AssetDescription);
                    classGenerator.Append(stringFieldGenerator);
                }

                config.GetAssetCollectionPath(collection, out var folderPath, out var fileName);
                scriptGenerator.GenerateScript(fileName, folderPath);
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif