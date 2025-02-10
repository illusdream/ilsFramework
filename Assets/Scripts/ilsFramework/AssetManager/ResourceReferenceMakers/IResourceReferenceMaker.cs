using System;
using UnityEditor;
using UnityEngine.UI;

namespace ilsFramework
{
    public interface IResourceReferenceMaker
    {
        /// <summary>
        /// 获取目标后缀名
        /// </summary>
        /// <returns></returns>
        string[] GetTargetResourceFileExtensions();
        
        /// <summary>
        /// 获取目标类型
        /// </summary>
        /// <returns></returns>
        Type[] GetTargetResourceTypes();

        /// <summary>
        /// 添加具体的资源信息
        /// </summary>
        /// <param name="resouceInfo"></param>
        void AddTargetResourceInfo(ResouceInfo resouceInfo);


        /// <summary>
        /// 创建资源引用资源
        /// </summary>
        void CreateReferenceFile();
        
        
        
    }
}