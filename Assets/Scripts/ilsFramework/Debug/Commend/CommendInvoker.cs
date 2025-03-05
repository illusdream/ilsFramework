using System;
using System.Reflection;
using System.Text;

namespace ilsFramework
{
    public class CommendInvoker
    {
        /// <summary>
        /// 需要执行的方法
        /// </summary>
        MethodInfo _methodInfo;
        /// <summary>
        /// 输入缓存
        /// </summary>
        ParseBuffer _buffer;


        public CommendInvoker(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
            _buffer = new ParseBuffer(_methodInfo.GetParameters());
        }


        public bool IsVialdArgsCount(int count)
        {
            return _buffer.MaxCanInvokeCount > count;
        }
        
        public Type GetTargetType(int index)
        {
            return _buffer.GetNeedType(index);
        }

        public void PushArgs(object value)
        {
            _buffer.Push(value);
        }

        public void PopArgs()
        {
            _buffer.Pop();
        }

        public void Reset()
        {
            _buffer.Reset();
        }
        
        
        public bool TryInvoke(object instance)
        {
            if (_buffer.CanInvoke())
            {
                _methodInfo.Invoke(instance, _buffer.GetParseBuffer());
                _buffer.Reset();
                return true;
            }
            _buffer.Reset();
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var parameterInfo in _methodInfo.GetParameters())
            {
                string defaultValue = parameterInfo.HasDefaultValue ? $" = {parameterInfo.DefaultValue}" : string.Empty;
                sb.Append($"[{parameterInfo.ParameterType.Name} {parameterInfo.Name}{defaultValue}] ");
            }
            return sb.ToString();
        }
    }
}