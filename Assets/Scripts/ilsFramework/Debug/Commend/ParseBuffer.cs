using System;
using System.Collections.Generic;
using System.Reflection;

namespace ilsFramework
{
    public class ParseBuffer
    {
        /// <summary>
        /// 存储解析参数
        /// </summary>
        private object[] parseBuffer;

        private Type[] targetParameterTypes;

        private Dictionary<int, object> defaultValues;
        
        /// <summary>
        /// 当前已填充的参数数目（不包括有默认参数的）
        /// </summary>
        private int currentInputCount;
        
        /// <summary>
        /// 当前已填充的参数数目（不包括有默认参数的）
        /// </summary>
        public int CurrentInputCount => currentInputCount;
        
        /// <summary>
        /// 最小可执行数目（假设所有默认参数未额外填充）
        /// </summary>
        public int MinCanInvokeCount { get; private set; }
        /// <summary>
        /// 最大可执行数目（假设所有默认参数都已被填充）
        /// </summary>
        public int MaxCanInvokeCount { get; private set; }

        public ParseBuffer(ParameterInfo[] parameters)
        {
            int _minCanInvokeCount = 0;
            
            parseBuffer = new object[parameters.Length];
            targetParameterTypes = new Type[parameters.Length];
            defaultValues = new Dictionary<int, object>();
            
            for (int i = 0; i < targetParameterTypes.Length; i++)
            {
                if (!parameters[i].HasDefaultValue)
                {
                    _minCanInvokeCount++;
                }
                else
                {
                    //保存默认值
                    defaultValues[i] = parameters[i].DefaultValue;
                    parseBuffer[i] = parameters[i].DefaultValue;
                }
                targetParameterTypes[i] = parameters[i].ParameterType;
            }
            
            currentInputCount = 0;
            MinCanInvokeCount = _minCanInvokeCount;
            MaxCanInvokeCount = parameters.Length;
        }

        public Type GetNeedType(int index)
        {
            return targetParameterTypes[index];
        }

        public void Push(Object value)
        {
            parseBuffer[currentInputCount] = value;
            currentInputCount++;
        }

        public void Pop()
        {
            currentInputCount--;
            parseBuffer[currentInputCount] = null;
        }

        public bool CanInvoke()
        {
            return currentInputCount >= MinCanInvokeCount && currentInputCount <= MaxCanInvokeCount;
        }

        public object[] GetParseBuffer()
        {
            return parseBuffer;
        }

        public void Reset()
        {
            for (int i = 0; i < MinCanInvokeCount; i++)
            {
                parseBuffer[i] = null;
            }

            foreach (var defaultValue in defaultValues)
            {
                parseBuffer[defaultValue.Key] = defaultValue.Value;
            }
            currentInputCount = 0;
        }
    }
}