using System;

namespace ilsFramework
{
    [AttributeUsage(AttributeTargets.Class,Inherited = false, AllowMultiple = false)]
    public class AutoBuildOrLoadConfig : Attribute
    {
        public string ConfigTargetPath;

        public AutoBuildOrLoadConfig(string configTargetPath)
        {
            ConfigTargetPath = configTargetPath;
        }
    }
}