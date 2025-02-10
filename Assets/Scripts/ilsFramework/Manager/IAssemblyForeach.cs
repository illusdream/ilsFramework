using System;

namespace ilsFramework
{
    /// <summary>
    /// 需要与<see cref="IManager"/>一同使用
    /// </summary>
    public interface IAssemblyForeach
    {
        /// <summary>
        /// 获取到正在遍历的程序集中的单个type
        /// </summary>
        /// <param name="types"></param>
        void ForeachCurrentAssembly(Type[] types);
    }
}

