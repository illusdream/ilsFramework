using System;

namespace ilsFramework
{
    /// <summary>
    /// 模块管理器基类
    /// </summary>
    [Serializable]
    public abstract class BaseManager
    {

        /// <summary>
        /// 模块的优先级，高优先级优先轮询，调用
        /// </summary>
        public virtual int Priority => 0;

        /// <summary>
        /// 初始化模块
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// 轮询模块
        /// </summary>
        public abstract void Update();


        /// <summary>
        /// 停止并清理模块
        /// </summary>
        public abstract void Shutdown();
    }
}

