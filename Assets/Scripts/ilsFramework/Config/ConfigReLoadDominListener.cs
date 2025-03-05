#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ilsFramework
{
    //侦测编译是否完成，查找所有含有  特性的脚本，并根据这个创建文件
    //维护文件夹结构，保证更新

    public class ConfigReLoadDominListener
    {
        private struct ConfigFileChecker
        {
            //本地的数据
            public bool Old;
            //应该要生成的数据
            public bool New;
            //是否是文件夹
            public bool IsFolder;
            //是否是有效的文件
            public bool Enable;
            
            public Type ConfigType;
            
        }
        
        private class ConfigTargetInfo
        {
            public Type ConfigType;
            public string TargetPath;
            public string OriginalPath;
            public bool HasFile;

            public bool NeedMove = false;
            public string NowPath;
        }
        
        private struct ConfigNeedAdd
        {
            public Type ConfigType;
            public string TargetPath;
        }
        
        private struct ConfigsNeedMove
        {
            public Type ConfigType;
            public string NowPath;
            public string TargetPath;
            
        }
        private struct FileNeedRemove
        {
            
        }
        
        private const string configsRootPath = "Assets/Resources/ilsFramework/Configs";
        
#if UNITY_EDITOR
        [DidReloadScripts()]

        //先这样写了，要是太卡了我再想别的位置
        private static void Listener_ReloadDomin()
        {
            FileUtils.AssetFolder_CheckOrCreateFolder("Resources/ilsFramework/Configs");
            ConfigManager.CheckFrameworkConfig();
            
            Dictionary<Type, ConfigTargetInfo> configsUsed = GetUsedConfigs();
            CheckRootFolderVilad(configsRootPath);

            var rootValue = new ConfigFileChecker() { IsFolder = true, Old = true, New = false };

            FileTrie<ConfigFileChecker> HasCurrectFiles = new FileTrie<ConfigFileChecker>(configsRootPath,rootValue,StartBuildChecker); ;
            
            //构建文件树，再根据文件树查询
            string[] guids = AssetDatabase.FindAssets("*", new []{configsRootPath});
            string rootPath = configsRootPath + '/';

            //构建文件树
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string pathWithoutExtension = Path.ChangeExtension(path, null);
                string cPath = configsRootPath + pathWithoutExtension.Substring(rootPath.Length);
                //
                //添加到文件树
                var checker = new ConfigFileChecker()
                {
                    IsFolder = AssetDatabase.IsValidFolder(pathWithoutExtension),
                    Old = true,
                    New = false,
                    Enable = false
                };
                var fi = new FileIEnumerator(cPath, configsRootPath);
                if (HasCurrectFiles.Contains(fi))
                {
                    fi.Reset();
                    HasCurrectFiles.Set(fi,checker);
                }
                else
                {
                    fi.Reset();
                    HasCurrectFiles.Add(fi,checker);
                }
                //判断已存在的Config位置是否正确
                if (!AssetDatabase.IsValidFolder(pathWithoutExtension))
                {
                    var file =  AssetDatabase.LoadAssetAtPath(path,typeof(Object));
                    //存在该Config
                    if (file != null&&configsUsed.TryGetValue(file.GetType(),out ConfigTargetInfo targetInfo))
                    {
                        targetInfo.HasFile = true;
                        //位置不正确

                        if (pathWithoutExtension != (configsRootPath+"/" + targetInfo.OriginalPath))
                        {
                            targetInfo.NeedMove = true;
                            targetInfo.NowPath = path;
                        }

                    }
                }
            }

            
            //修改默认创建函数，用以标记需要生成的函数
            HasCurrectFiles.SetDefaultConstructor(NeedBuildChecker);
            //将需要生成的文件夹加入到树中
            foreach (var add in configsUsed.Values)
            {
                string cPath = add.TargetPath;
                var quary = new FileIEnumerator(cPath, configsRootPath);
                if (!HasCurrectFiles.Contains(quary))
                {
                    quary.Reset();
                    ConfigFileChecker fileChecker = new ConfigFileChecker()
                    {
                        IsFolder = false,
                        Old = false,
                        New = true,
                        ConfigType = add.ConfigType
                    };
                    HasCurrectFiles.Add(quary, fileChecker);
                }

            }
            //遍历节点，生成对应缺失的文件夹
            foreach (var checker in HasCurrectFiles)
            {
                if (checker.value.New)
                {
                    int ParentFolderLength = checker.GetPath().Length - checker.Name.Length - 1;

                    if (checker.value.IsFolder)
                    {
                        AssetDatabase.CreateFolder(checker.GetPath().Substring(0,ParentFolderLength), checker.Name);
                    }
                }
            }
            //遍历添加部分，查找哪些Config需要修改位置或者新增
            foreach (var configInfo in configsUsed.Values)
            {
                string targetPath =  $"{configsRootPath}/{configInfo.OriginalPath}.asset";
                if (!configInfo.HasFile)
                {
                    
                    var so_Instance =  ScriptableObject.CreateInstance(configInfo.ConfigType);
                    try
                    {
                        AssetDatabase.CreateAsset(so_Instance, targetPath);
                    }
                    catch (Exception e)
                    {
                        e.LogSelf();
                        throw;
                    }
                }
                else 
                {
                    if (configInfo.NeedMove)
                    {
                        Debug.Log($"Move : {configInfo.NowPath}");
                        Debug.Log($"Move : {targetPath}");
                        AssetDatabase.MoveAsset(configInfo.NowPath, targetPath);
                    }

                }
            }
            
            
            //删除无用文件
            CleanNoUsedFolders();
            


            ConfigFileChecker NeedBuildChecker()
            {
                return new ConfigFileChecker()
                {
                    IsFolder = true,
                    Old = false,
                    New = true,
                    Enable = false
                };
            }
        }

        private static void CheckRootFolderVilad(FileIEnumerator path)
        {
            if (!AssetDatabase.IsValidFolder(path.ToString()))
            {
                AssetDatabase.CreateFolder(path.GetFileParentPath(), path.GetFileName());
            }
        }

        private static Dictionary<Type,ConfigTargetInfo> GetUsedConfigs()
        {
            Dictionary<Type, ConfigTargetInfo> result = new Dictionary<Type, ConfigTargetInfo>();
            var types = Assembly.GetExecutingAssembly().GetTypes();
            //找出对应需要遍历程序集
            foreach (var type in types)
            {
                AutoBuildOrLoadConfig attribute = type.GetCustomAttribute<AutoBuildOrLoadConfig>();
                if (attribute != null)
                { 
                    string cPath =  $"{configsRootPath}{attribute.ConfigTargetPath}";
                    ConfigTargetInfo configInfo = new ConfigTargetInfo()
                    {
                        OriginalPath = attribute.ConfigTargetPath,
                        TargetPath = cPath,
                        HasFile = false,
                        ConfigType = type,
                        NeedMove = false
                    };
                    result.Add(type,configInfo);
                }
            }
            return result;
        }


        private static void CleanNoUsedFolders()
        {
            var rootValue = new ConfigFileChecker() { IsFolder = true, Old = true, New = false };

            FileTrie<ConfigFileChecker> HasCurrectFiles = new FileTrie<ConfigFileChecker>(configsRootPath,rootValue,StartBuildChecker);
            
            string[] guids = AssetDatabase.FindAssets("*", new []{configsRootPath});
            string rootPath = configsRootPath + '/';

            //构建文件树
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string pathWithoutExtension = Path.ChangeExtension(path, null);
                string cPath = configsRootPath + pathWithoutExtension.Substring(rootPath.Length);
                //
                //添加到文件树
                var checker = new ConfigFileChecker()
                {
                    IsFolder = AssetDatabase.IsValidFolder(pathWithoutExtension),
                    Old = true,
                    New = false,
                    Enable = false
                };
                var fi = new FileIEnumerator(cPath, configsRootPath);
                if (HasCurrectFiles.Contains(fi))
                {
                    fi.Reset();
                    HasCurrectFiles.Set(fi,checker);
                }
                else
                {
                    fi.Reset();
                    HasCurrectFiles.Add(fi,checker);
                }
            }
            Stack<FileTrieNode<ConfigFileChecker>> stack = new Stack<FileTrieNode<ConfigFileChecker>>();
            foreach (var trieNode in HasCurrectFiles)
            {
                stack.Push(trieNode);
            }
            HashSet<string> HasDeletedFiles = new HashSet<string>();
            foreach (var trieNode in stack)
            {
                if ( trieNode.value.IsFolder)
                {
                    if (trieNode.Children.Count == 0)
                    {
                        AssetDatabase.DeleteAsset(trieNode.GetPath());
                        HasDeletedFiles.Add(trieNode.GetPath());
                    }
                    else
                    {
                        bool shouldRemove = true;
                        foreach (var child in trieNode.Children.Values)
                        {
                            shouldRemove &= HasDeletedFiles.Contains(child.GetPath());
                        }

                        if (shouldRemove)
                        {
                            AssetDatabase.DeleteAsset(trieNode.GetPath());
                            HasDeletedFiles.Add(trieNode.GetPath());
                        }
                    }
                }
                
                
                //添加未存在的类
            }
        }
        
        
         static ConfigFileChecker StartBuildChecker()
        {
            return new ConfigFileChecker()
            {
                IsFolder = true,
                Old = true, 
                New = false ,
                Enable = false
            };
        }
#endif
    }
    

}
#endif