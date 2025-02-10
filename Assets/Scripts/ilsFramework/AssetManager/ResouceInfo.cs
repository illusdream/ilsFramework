using System;
using System.Net;

namespace ilsFramework
{
    public struct ResouceInfo
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName;
        
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath;
        
        /// <summary>
        /// 文件后缀
        /// </summary>
        public string FileExtension;
        
        /// <summary>
        /// 文件加载后的类型
        /// </summary>
        public Type FileLoadIntanceType;
        
        /// <summary>
        /// 文件加载后具体的内容
        /// </summary>
        public object FileContent;
    }
}