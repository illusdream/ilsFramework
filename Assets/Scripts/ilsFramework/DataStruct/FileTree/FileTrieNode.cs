using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ilsFramework
{
    [Serializable]
    public class FileTrieNode<T>
    {
        public string Name;
        public EFileTrieNodeType Type;
        public T value;
        
        public FileTrieNode<T> Parent;
        [ShowInInspector]
        public Dictionary<string, FileTrieNode<T>> Children;
        [ShowInInspector]
        public string Path => GetPath();
        

        public FileTrieNode(EFileTrieNodeType type, string name,T value ,FileTrieNode<T> parent = null)
        {
            Name = name;
            Type = type;
            this.value = value;
            Parent = parent;
            Children = new Dictionary<string, FileTrieNode<T>>();
        }

        public string GetPath()
        { 
            return (Parent != null ? Parent.GetPath()  + "/": string.Empty) + Name;
        }
        
        public void AddChild(FileIEnumerator fileIEnumerator,T value,Func<T> defaultConstructor)
        {
            //存在该子节点
            if (Children.TryGetValue(fileIEnumerator.CurrentFileName, out FileTrieNode<T> fileTreeNode))
            {
                //迭代器可以向下移动（非最终节点-》文件夹），交由下级节点处理
                if (fileIEnumerator.MoveNext())
                {
                    fileTreeNode.AddChild(fileIEnumerator, value,defaultConstructor);
                }
                //不能向下移动，说明已经是最终节点 -》 文件，给这个节点赋值
                else
                {
                    Children[fileIEnumerator.CurrentFileName] = new FileTrieNode<T>(EFileTrieNodeType.Asset,fileIEnumerator.CurrentFileName,value,this);
                }

            }
            //不存在子节点
            else
            {
                EFileTrieNodeType nodeType = fileIEnumerator.IsEnd() ? EFileTrieNodeType.Asset: EFileTrieNodeType.Folder;
                T nodeValue =fileIEnumerator.IsEnd() ? value : defaultConstructor.Invoke();
                var node = new FileTrieNode<T>(nodeType,fileIEnumerator.CurrentFileName, nodeValue,this);
                Children.Add(fileIEnumerator.CurrentFileName, node);
                string nodePath = fileIEnumerator.CurrentFileName;
                if (fileIEnumerator.MoveNext())
                {
                    Children[nodePath].AddChild(fileIEnumerator, value,defaultConstructor);
                }
            }
        }

        public void RemoveChild(FileIEnumerator fileIEnumerator)
        {
            if (Children.TryGetValue(fileIEnumerator.CurrentFileName, out FileTrieNode<T> fileTreeNode))
            {
                if (fileIEnumerator.MoveNext())
                {
                    fileTreeNode.RemoveChild(fileIEnumerator);
                }
                else
                {
                    Children.Remove(fileIEnumerator.CurrentFileName);
                }
            }
        }

        public bool TryGet(FileIEnumerator fileIEnumerator, out T value)
        {

            if (Children.TryGetValue(fileIEnumerator.CurrentFileName, out FileTrieNode<T> fileTreeNode))
            {
                if (fileIEnumerator.MoveNext())
                {
                    return fileTreeNode.TryGet(fileIEnumerator, out value);
                }
                else
                {
                    value = fileTreeNode.value;
                    return true;
                }
            }
            value = default(T);
            return false;
        }

        public void Set(FileIEnumerator fileIEnumerator, T value)
        {
            if (Children.TryGetValue(fileIEnumerator.CurrentFileName, out FileTrieNode<T> fileTreeNode))
            {
                if (fileIEnumerator.MoveNext())
                {
                    fileTreeNode.Set(fileIEnumerator, value);
                }
                else
                {
                    Children[fileIEnumerator.CurrentFileName].value = value;
                }
            }
        }
        
        public bool Contains(FileIEnumerator fileIEnumerator)
        {
            if (Children.TryGetValue(fileIEnumerator.CurrentFileName, out FileTrieNode<T> fileTreeNode))
            {
                if (fileIEnumerator.MoveNext())
                {
                    return fileTreeNode.Contains(fileIEnumerator);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            Children.Clear();
        }


        
        public override string ToString()
        {
            return $"Name: {Name}, Path: {GetPath()}, Value: {value}";
        }
    }
}