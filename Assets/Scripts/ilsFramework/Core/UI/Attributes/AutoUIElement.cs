using System;

namespace ilsFramework.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class AutoUIElement : Attribute
    {
        public string TargetComponentPath;

        public AutoUIElement(string targetComponentPath = "")
        {
            TargetComponentPath = targetComponentPath;
        }
    }
}