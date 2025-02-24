using System;

namespace ilsFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class DebugCommendAttribute : Attribute
    {
        public string CommendTargetMethod;

        public DebugCommendAttribute(string commendTargetMethod)
        {
            CommendTargetMethod = commendTargetMethod;
        }
    }
}