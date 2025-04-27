using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace ilsFramework.Core.Editor
{
    public class ilsFrameworkSettingWindows : OdinMenuEditorWindow
    {
        [ShowInInspector] public FrameworkConfig FrameworkConfig;

        [MenuItem("ilsFramework/框架配置")]
        private static void OpenWindow()
        {
            var window = GetWindow<ilsFrameworkSettingWindows>();

            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(720, 720);
            window.titleContent = new GUIContent("ilsFramework配置");
        }


        protected override OdinMenuTree BuildMenuTree()
        {
            CheckBaseConfigFile();
            var tree = new OdinMenuTree();
            tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
            var dicList = new List<string>();
            var treeList = new List<(string, int, ConfigContainer)>();
            dicList.AddRange(Directory.GetDirectories(ConfigScript.ConfigsFolder, "*", SearchOption.TopDirectoryOnly));
            dicList.AddRange(Directory.GetFiles(ConfigScript.ConfigsFolder, "*.asset", SearchOption.TopDirectoryOnly));
            foreach (var path in dicList)
            {
                var keyName = Path.GetFileNameWithoutExtension(path);
                var instance = new ConfigContainer(path);
                treeList.Add((keyName, FrameworkConfig.GetConfigSortOrder(keyName), instance));
            }

            FrameworkConfig.ReSortConfigShow();
            for (var i = 0; i < treeList.Count; i++)
            {
                var t = treeList[i];
                treeList[i] = (t.Item1, FrameworkConfig.GetConfigSortOrder(t.Item1), t.Item3);
            }

            treeList.Sort((a, b) => a.Item2.CompareTo(b.Item2));
            foreach (var valueTuple in treeList) tree.Add(valueTuple.Item1, valueTuple.Item3);
            return tree;
        }


        private void CheckBaseConfigFile()
        {
            //检查资源Config
            FrameworkConfig = CheckSingleConfigFile<FrameworkConfig>(ConfigScript.ConfigsFolder + "FrameworkConfig.asset");
        }

        private T CheckSingleConfigFile<T>(string path) where T : ScriptableObject
        {
            var targetConfigFile = AssetDatabase.LoadAssetAtPath<T>(path);
            if (!targetConfigFile)
            {
                targetConfigFile = CreateInstance<T>();
                AssetDatabase.CreateAsset(targetConfigFile, path);
            }

            return targetConfigFile;
        }
    }
}