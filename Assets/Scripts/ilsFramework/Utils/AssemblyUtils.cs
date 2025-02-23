using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ilsFramework
{
    public static class AssemblyUtils
    {
        public static List<(FieldInfo,T)> GetAllFieldInfoWithAttribute<T>(Type type,params Type[] matchTypes ) where T : Attribute
        {
            HashSet<Type> _matchedTypes = matchTypes.ToHashSet();
            List<(FieldInfo,T)> fieldInfos = new List<(FieldInfo,T)>();
            var allFields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var fieldInfo in allFields)
            {
                var attribute = fieldInfo.GetCustomAttribute<T>();
                bool isMatch = _matchedTypes.Contains(fieldInfo.FieldType) || matchTypes.Length == 0;
                if (attribute != null &&isMatch)
                {
                    fieldInfos.Add((fieldInfo, attribute));
                }
            }
            return fieldInfos;
        }
        
        public static List<(PropertyInfo,T)> GetAllPropertyInfoWithAttribute<T>(Type type,params Type[] matchTypes ) where T : Attribute
        {
            HashSet<Type> _matchedTypes = matchTypes.ToHashSet();
            List<(PropertyInfo,T)> propertyInfos = new List<(PropertyInfo,T)>();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var propertyInfo in properties)
            {
                var attribute = propertyInfo.GetCustomAttribute<T>();
                bool isMatch = _matchedTypes.Contains(propertyInfo.PropertyType) || matchTypes.Length == 0;
                if (attribute != null && isMatch)
                {
                    propertyInfos.Add((propertyInfo, attribute));
                }
            }
            return propertyInfos;
        }
    }
}