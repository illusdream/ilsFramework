using System;

namespace ilsFramework.Core
{
    public interface IParser
    {
        public Type GetTargetType();
        public bool TryParse(string text, out object value);
    }
}