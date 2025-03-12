using System;

namespace ilsFramework.Attributes
{
    /// <summary>
    /// 将该AudioChannel忽略,系统不会默认生成对应的实例以及Config配置
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class AudioChannelInstanceIgnore : Attribute
    {
        
    }
}