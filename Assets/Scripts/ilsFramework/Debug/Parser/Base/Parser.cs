using System;

namespace ilsFramework
{
    public abstract class Parser<T> : IParser
    {
        public Type TargetType => typeof(T);
        public Type GetTargetType()
        {
            return TargetType;
        }
        public abstract bool TryParse(string text, out object value);
    }
}