using System;
using System.Reflection;

namespace ilsFramework
{
    public class ConfigUtils
    {
        public T GetConfig<T>() where T : ConfigScriptObject
        {
            var attributes = typeof(T).GetCustomAttribute(typeof (AutoBuildOrLoadConfig));
            if (attributes != null)
            {
                
            }

            return null;
        }
    }
}