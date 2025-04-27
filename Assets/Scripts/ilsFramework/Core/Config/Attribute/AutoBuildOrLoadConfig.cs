using System;

namespace ilsFramework.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AutoBuildOrLoadConfig : Attribute
    {
        public string ConfigTargetPath;

        public AutoBuildOrLoadConfig(string configTargetPath)
        {
            ConfigTargetPath = configTargetPath;
        }
    }
}