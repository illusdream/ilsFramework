#if UNITY_EDITOR


using System;
using ilsFramework.Core.SQLite4Unity3d;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ilsFramework.Core
{
    [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
    [ReadOnly]
    public abstract class AssetChangeLogeDetailWindow
    {
        protected Action _needFresh;

        [HideInInspector] public int AssetID;

        [HideInInspector] public AssetChangeType ChangeType;

        [Title("$Title", titleAlignment: TitleAlignments.Left)] [HideLabel] [PropertyOrder(int.MinValue)] [ShowInInspector]
        private title configName;

        [HideInInspector] public DateTime Date;

        [HideInInspector] public string GUID;

        [HideInInspector] public int ID;

        /// <summary>
        ///     这次修改后的值
        /// </summary>
        [HideInInspector] public string NewValue;

        /// <summary>
        ///     这次修改前的值
        /// </summary>
        [HideInInspector] public string OldValue;

        public AssetChangeLogeDetailWindow(AssetChangeLog assetChangeLog, SQLiteConnection connection)
        {
            ID = assetChangeLog.ID;
            AssetID = assetChangeLog.AssetID;
            ChangeType = assetChangeLog.ChangeType;
            OldValue = assetChangeLog.OldValue;
            NewValue = assetChangeLog.NewValue;
            Date = assetChangeLog.Date;
            GUID = assetChangeLog.GUID;
        }

        public event Action NeedFresh
        {
            add => _needFresh += value;
            remove => _needFresh -= value;
        }

        public abstract string Title();

        [DisableInInlineEditors]
        private struct title
        {
        }
    }

    public class ChangeKeyNameWindow : AssetChangeLogeDetailWindow
    {
        public ChangeKeyNameWindow(AssetChangeLog assetChangeLog, SQLiteConnection connection) : base(assetChangeLog, connection)
        {
        }

        [FoldoutGroup("Info")]
        [VerticalGroup("Info/1")]
        [ShowInInspector]
        [LabelText("旧引用名")]
        [HorizontalGroup("Info/1/Old", 0.7f)]
        public string OldName => OldValue;

        [ShowInInspector]
        [LabelText("新引用名")]
        [HorizontalGroup("Info/1/New", 0.7f)]
        public string NewName => NewValue;

        public override string Title()
        {
            return "修改引用名 : " + OldName + " -> " + NewValue;
        }

        [HorizontalGroup("Info/1/Old")]
        [Button(SdfIconType.FileCode, "复制旧引用")]
        private void CopyOldData()
        {
            var textEditor = new TextEditor();
            textEditor.text = OldValue;
            textEditor.OnFocus();
            textEditor.Copy();
        }

        [HorizontalGroup("Info/1/New")]
        [Button(SdfIconType.FileCode, "复制新引用")]
        private void CopyNewData()
        {
            var textEditor = new TextEditor();
            textEditor.text = NewValue;
            textEditor.OnFocus();
            textEditor.Copy();
        }

        [VerticalGroup("Info/1")]
        [Button(SdfIconType.Link45deg, "定位至文件")]
        private void PingObject()
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

        [VerticalGroup("Info/1")]
        [Button(SdfIconType.Reply, "撤销更改")]
        private void Reset()
        {
            var collection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);
            collection.Query<AssetInfo>("UPDATE AssetInfo SET AssetKey = ? WHERE ID = ?", OldValue, AssetID);
            collection.Execute("DELETE FROM AssetChangeLog WHERE AssetID = ? AND ChangeType = ?", AssetID, ChangeType);
            _needFresh?.Invoke();
        }
    }

    public class ChangeCollectionWindow : AssetChangeLogeDetailWindow
    {
        public ChangeCollectionWindow(AssetChangeLog assetChangeLog, SQLiteConnection connection) : base(assetChangeLog, connection)
        {
        }

        [FoldoutGroup("Info")]
        [VerticalGroup("Info/1")]
        [ShowInInspector]
        [LabelText("旧引用名")]
        [HorizontalGroup("Info/1/Old", 0.7f)]
        public string OldName => OldValue;

        [ShowInInspector]
        [LabelText("新引用名")]
        [HorizontalGroup("Info/1/New", 0.7f)]
        public string NewName => NewValue;

        public override string Title()
        {
            return "修改引用组 : " + OldName + " -> " + NewValue;
        }

        [HorizontalGroup("Info/1/Old")]
        [Button(SdfIconType.FileCode, "复制旧引用")]
        private void CopyOldData()
        {
            var textEditor = new TextEditor();
            textEditor.text = OldValue;
            textEditor.OnFocus();
            textEditor.Copy();
        }

        [HorizontalGroup("Info/1/New")]
        [Button(SdfIconType.FileCode, "复制新引用")]
        private void CopyNewData()
        {
            var textEditor = new TextEditor();
            textEditor.text = NewValue;
            textEditor.OnFocus();
            textEditor.Copy();
        }

        [VerticalGroup("Info/1")]
        [Button(SdfIconType.Link45deg, "定位至文件")]
        private void PingObject()
        {
            var path = AssetDatabase.GUIDToAssetPath(GUID);
            path.LogSelf();
            GUID.LogSelf();
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

        [VerticalGroup("Info/1")]
        [Button(SdfIconType.Reply, "撤销更改")]
        private void Reset()
        {
            var collection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);
            collection.Query<AssetInfo>("UPDATE AssetInfo SET AssetKey = ?,AssetCollection = ? WHERE ID = ?", OldValue, OldValue.Split(".")[0], AssetID);
            collection.Execute("DELETE FROM AssetChangeLog WHERE AssetID = ? AND ChangeType = ?", AssetID, ChangeType);
            _needFresh?.Invoke();
        }
    }

    public class ChangeDescriptionWindow : AssetChangeLogeDetailWindow
    {
        private readonly string Name;

        public ChangeDescriptionWindow(AssetChangeLog assetChangeLog, SQLiteConnection connection) : base(assetChangeLog, connection)
        {
            Name = connection.Table<AssetInfo>().Where(a => a.ID == ID).First().AssetKey;
        }

        [FoldoutGroup("Info")]
        [VerticalGroup("Info/1")]
        [ShowInInspector]
        [LabelText("旧注释")]
        [HorizontalGroup("Info/1/Old", 0.7f)]
        public string OldName => OldValue;

        [ShowInInspector]
        [LabelText("新注释")]
        [HorizontalGroup("Info/1/New", 0.7f)]
        public string NewName => NewValue;

        [HorizontalGroup("Info/1/Old")]
        [Button(SdfIconType.Link45deg, "定位至文件")]
        private void PingObject()
        {
            var path = AssetDatabase.GUIDToAssetPath(GUID);
            path.LogSelf();
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

        [HorizontalGroup("Info/1/New")]
        [Button(SdfIconType.Reply, "撤销更改")]
        private void Reset()
        {
            var collection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);
            collection.Query<AssetInfo>("UPDATE AssetInfo SET AssetDescription = ? WHERE ID = ?", OldValue, AssetID);
            collection.Execute("DELETE FROM AssetChangeLog WHERE AssetID = ? AND ChangeType = ?", AssetID, ChangeType);
            _needFresh?.Invoke();
        }

        public override string Title()
        {
            return "修改引用注释 : " + Name;
        }
    }

    public class AssetRemoveWindow : AssetChangeLogeDetailWindow
    {
        public AssetRemoveWindow(AssetChangeLog assetChangeLog, SQLiteConnection connection) : base(assetChangeLog, connection)
        {
        }

        [ShowInInspector] [LabelText("文件名")] public string OldName => OldValue;

        public override string Title()
        {
            return "删除文件: " + OldValue;
        }
    }

    public class ChangeUseAssetBundleWindow : AssetChangeLogeDetailWindow
    {
        private readonly string Path;

        public ChangeUseAssetBundleWindow(AssetChangeLog assetChangeLog, SQLiteConnection connection) : base(assetChangeLog, connection)
        {
            Path = AssetDatabase.GUIDToAssetPath(GUID);
        }

        public override string Title()
        {
            return "修改是否使用AssetBundle: " + Path + "|" + (NewValue == "NoUse" ? "已不使用" : "已使用");
        }
    }

    public class ChangeAssetBundleWindow : AssetChangeLogeDetailWindow
    {
        public ChangeAssetBundleWindow(AssetChangeLog assetChangeLog, SQLiteConnection connection) : base(assetChangeLog, connection)
        {
        }

        [FoldoutGroup("Info")]
        [VerticalGroup("Info/1")]
        [ShowInInspector]
        [LabelText("旧AssetBundle")]
        [HorizontalGroup("Info/1/Old", 0.7f)]
        public string OldName => OldValue;

        [ShowInInspector]
        [LabelText("新AssetBundle")]
        [HorizontalGroup("Info/1/New", 0.7f)]
        public string NewName => NewValue;

        public override string Title()
        {
            return "修改所在AssetBundle";
        }

        [VerticalGroup("Info/1")]
        [Button(SdfIconType.Link45deg, "定位至文件")]
        private void PingObject()
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

        [VerticalGroup("Info/1")]
        [Button(SdfIconType.Reply, "撤销更改")]
        private void Reset()
        {
            var collection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);
            collection.Query<AssetInfo>("UPDATE AssetInfo SET AssetBundleName = ? WHERE ID = ?", OldValue, AssetID);
            collection.Execute("DELETE FROM AssetChangeLog WHERE AssetID = ? AND ChangeType = ?", AssetID, ChangeType);
            _needFresh?.Invoke();
        }
    }

    public class ChangeAssetNameWindow : AssetChangeLogeDetailWindow
    {
        public ChangeAssetNameWindow(AssetChangeLog assetChangeLog, SQLiteConnection connection) : base(assetChangeLog, connection)
        {
        }

        public override string Title()
        {
            return "修改资源名";
        }
    }

    public class ChangeAssetPathWindow : AssetChangeLogeDetailWindow
    {
        public ChangeAssetPathWindow(AssetChangeLog assetChangeLog, SQLiteConnection connection) : base(assetChangeLog, connection)
        {
        }

        public override string Title()
        {
            return "修改资源路径";
        }
    }
}
#endif