using System;

namespace ilsFramework
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class AutoUIElement : Attribute
    {
        public string TargetComponentPath;

        public AutoUIElement(string targetComponentPath = "")
        {
            TargetComponentPath = targetComponentPath;
        }
    }
}