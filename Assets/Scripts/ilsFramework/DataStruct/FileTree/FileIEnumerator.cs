using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ilsFramework
{
    [Serializable]
    public class FileIEnumerator
    {
        private string[] fileNodes;

        public int currentIndex;
        
        public string CurrentFileName => fileNodes[currentIndex];

        public FileIEnumerator(string filePath,string firstNodeValue =null)
        {
            if (firstNodeValue != null)
            {   
                List<string> result = new List<string>();
                int l = firstNodeValue.Length;
                string first = filePath.Substring(0, l);
                if (firstNodeValue == first)
                {
                    result.Add(first);
                }
                result.AddRange((filePath.Substring(l)).Split("/"));
                fileNodes = result.ToArray();
            }
            else
            {
                fileNodes = filePath.Split("/");
            }

            currentIndex = 0;
        }
        
        
        public bool MoveNext()
        {
            if (currentIndex >= fileNodes.Length - 1)
            {
                return false;
            }
            currentIndex++;
            return true;
        }

        public bool IsEnd()
        {
            return currentIndex == fileNodes.Length - 1;
        }

        public static implicit operator FileIEnumerator(string filePath)
        {
            return new FileIEnumerator(filePath);
        }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < fileNodes.Length; i++)
            {
                result += $"{fileNodes[i]}/";
            }
            return result.Remove(result.Length - 1);
        }

        public string GetFileParentPath()
        {
            string result = "";
            for (int i = 0; i < fileNodes.Length-1; i++)
            {
                string split = i != 0 ? "/" : string.Empty;
                result += $"{fileNodes[i]}{split}";
            }
            return result.Remove(result.Length - 1);
        }

        public string GetFileName()
        {
            return fileNodes.Last();
        }

        public void Reset()
        {
            currentIndex = 0;
        }
    }
}