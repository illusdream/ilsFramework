using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Sirenix.OdinInspector;

namespace ilsFramework
{
    /// <summary>
    /// 特殊的前缀树，但是每个节点存储的是文件夹名字，分成两类节点：文件节点/文件夹节点
    /// </summary>
    [Serializable]
    public class FileTrie<T> : IEnumerable<FileTrieNode<T>>
    {
        [ShowInInspector]
         public  FileTrieNode<T> root;
         private Func<T> defaultConstructor;
         public FileTrie(string RootFolderName,T RootObject,Func<T> DefaultConstructor)
         {
             root = new FileTrieNode<T>(EFileTrieNodeType.Folder,RootFolderName,RootObject,null);
             this.defaultConstructor = DefaultConstructor;
         }
         
         public void Add(FileIEnumerator fileIEnumerator, T value)
         {
             if (root.Name == fileIEnumerator.CurrentFileName &&fileIEnumerator.MoveNext())
                 root.AddChild(fileIEnumerator, value,defaultConstructor);
         }

         public void Remove(FileIEnumerator fileIEnumerator)
         {
             if (root.Name == fileIEnumerator.CurrentFileName)
             {
                 if (fileIEnumerator.MoveNext())
                 {
                     root.RemoveChild(fileIEnumerator);
                 }
                 else
                 {
                     root.Clear();
                 }
                 
             }
         }

         public bool TryGet(FileIEnumerator path, out T value)
         {
             if (path.CurrentFileName == root.Name)
             {
                 if (path.MoveNext())
                 {
                     return root.TryGet(path, out value);
                 }
             }
             value = default(T);
             return false;
         }
         
         public void Set(FileIEnumerator fileIEnumerator, T value)
         {
             if (fileIEnumerator.CurrentFileName == root.Name)
             {
                 if (fileIEnumerator.MoveNext())
                 {
                     root.Set(fileIEnumerator, value);
                 }
             }
         }

         public bool Contains(FileIEnumerator fileIEnumerator)
         {
             if (fileIEnumerator.CurrentFileName == root.Name && fileIEnumerator.MoveNext())
             {
                 return root.Contains(fileIEnumerator);
             }
             return false;
         }

         public T GetDefault()
         {
             return  defaultConstructor == null ? default : defaultConstructor.Invoke();
         }

         public void SetDefaultConstructor(Func<T> constructor)
         {
             defaultConstructor = constructor;
         }
         
         

         public IEnumerator<FileTrieNode<T>> GetEnumerator()
         {
             return new FileTrieEnumerator(root);
         }

         IEnumerator IEnumerable.GetEnumerator()
         {
             return GetEnumerator();
         }
         private class  FileTrieEnumerator : IEnumerator<FileTrieNode<T>>
         {
             private Stack<FileTrieNode<T>> stack;

             public FileTrieEnumerator(FileTrieNode<T> root)
             {
                 stack = new Stack<FileTrieNode<T>>();
                 if (root != null)
                 {
                     stack.Push(root);
                 }
             }
             public bool MoveNext()
             {
                 if (stack.Count == 1 && stack.Peek().Children.Count ==0)
                 {
                     return false;
                 }
                 
                 var cNode = stack.Pop();
                 foreach (var child in cNode.Children.Values.Reverse())
                 {
                     stack.Push(child);
                 }
                 return true;
             }

             public void Reset()
             {
                 throw new NotImplementedException("不支持重置迭代器");
             }

             public FileTrieNode<T> Current
             {
                 get
                 {
                     if (stack == null)
                     {
                         throw new InvalidOperationException("未初始化迭代器");
                     }

                     if (stack.Count == 0)
                     {
                         return null;
                     }
                     return stack.Peek();
                 }
             }

             object IEnumerator.Current => Current;

             public void Dispose()
             {
                 stack.Clear();
                 stack = null;
             }
         }

        private class FileTrieReverseEnumerator : IEnumerator<FileTrieNode<T>>
        {
            private Stack<FileTrieNode<T>> stack;

            public FileTrieReverseEnumerator(FileTrieNode<T> root)
            {
                stack = new Stack<FileTrieNode<T>>();
                if (root != null)
                {
                    stack.Push(root);
                }
            }
            public bool MoveNext()
            {
                if (stack.Count == 1 && stack.Peek().Children.Count ==0)
                {
                    return false;
                }
                 
                var cNode = stack.Pop();
                foreach (var child in cNode.Children.Values.Reverse())
                {
                    stack.Push(child);
                }
                return true;
            }

            public void Reset()
            {
                throw new NotImplementedException("不支持重置迭代器");
            }

            public FileTrieNode<T> Current
            {
                get
                {
                    if (stack == null)
                    {
                        throw new InvalidOperationException("未初始化迭代器");
                    }

                    if (stack.Count == 0)
                    {
                        return null;
                    }
                    return stack.Peek();
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                stack.Clear();
                stack = null;
            }
        }
    }
}