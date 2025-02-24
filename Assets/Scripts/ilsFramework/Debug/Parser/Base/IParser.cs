using System;

namespace ilsFramework
{
    public interface  IParser
    {
        public Type GetTargetType();
        public bool TryParse(string text, out object value);
    }
}