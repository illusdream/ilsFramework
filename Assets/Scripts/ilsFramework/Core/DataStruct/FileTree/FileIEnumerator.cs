using System;
using System.Collections.Generic;
using System.Linq;

namespace ilsFramework.Core
{
    [Serializable]
    public class FileIEnumerator
    {
        public int currentIndex;
        private string[] fileNodes;

        public FileIEnumerator(string filePath, string firstNodeValue = null)
        {
            if (firstNodeValue != null)
            {
                var result = new List<string>();
                var l = firstNodeValue.Length;
                var first = filePath.Substring(0, l);
                if (firstNodeValue == first) result.Add(first);
                result.AddRange(filePath.Substring(l).Split("/"));
                fileNodes = result.ToArray();
            }
            else
            {
                fileNodes = filePath.Split("/");
            }

            currentIndex = 0;
        }

        public string CurrentFileName => fileNodes[currentIndex];


        public bool MoveNext()
        {
            if (currentIndex >= fileNodes.Length - 1) return false;
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
            var result = "";
            for (var i = 0; i < fileNodes.Length; i++) result += $"{fileNodes[i]}/";
            return result.Remove(result.Length - 1);
        }

        public string GetFileParentPath()
        {
            var result = "";
            for (var i = 0; i < fileNodes.Length - 1; i++)
            {
                var split = i != 0 ? "/" : string.Empty;
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