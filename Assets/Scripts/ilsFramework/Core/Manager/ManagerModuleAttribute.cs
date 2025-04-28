using System;
using System.Collections.Generic;

namespace ilsFramework.Core
{
    /// <summary>
    /// 模块特性，通过该特性为Manager添加初始化模块
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true, Inherited = false)]
    public class ManagerModuleAttribute : Attribute
    {
        public Type[] ModuleType;

        public ManagerModuleAttribute(params Type[] moduleType)
        {
            ModuleType = moduleType;
        }
    }
}