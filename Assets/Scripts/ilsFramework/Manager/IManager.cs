using System;

namespace ilsFramework
{
    /// <summary>
    /// 管理类的接口，用此接入ilsFrameworkCore
    /// 与<see cref="ManagerSingleton{T}"/>一同使用创建Manger单例类
    /// </summary>
    public interface IManager
    {
        /// <summary>
        /// 管理类的优先级，高优先级优先轮询，调用
        /// </summary>
        public  int Priority => 0;

        /// <summary>
        /// 初始化模块
        /// </summary>
        void Init();
        /// <summary>
        /// 轮询模块
        /// </summary>
        void Update();

        void LateUpdate();

        void FixedUpdate();


        /// <summary>
        /// 停止并清理管理类
        /// </summary>
        void OnDestroy();
        /// <summary>
        /// 
        /// </summary>
        void OnDrawGizmos();
        /// <summary>
        /// 
        /// </summary>
        void OnDrawGizmosSelected();
    }
}
