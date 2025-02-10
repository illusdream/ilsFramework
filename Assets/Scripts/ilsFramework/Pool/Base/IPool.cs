using System;

namespace ilsFramework
{
    public interface IPool<T>
    {
        /// <summary>
        /// 获取对象池中当前对象数目
        /// </summary>
        /// <returns></returns>
        int GetObjectCount();
        
        /// <summary>
        /// 获取最大容量
        /// </summary>
        /// <returns></returns>
        int GetMaxCapacity();
        
        /// <summary>
        /// 获取对象池中活跃的对象
        /// </summary>
        /// <returns></returns>
        int GetActiveObjectCount();
        
        /// <summary>
        /// 获取对象池中不活跃（可用）的对象
        /// </summary>
        /// <returns></returns>
        int GetDeActiveObjectCount();
        
        /// <summary>
        /// 是否启用集合检查
        /// </summary>
        bool CollectionCheck{get;set;}
        
        /// <summary>
        /// 从对象池中获取一个对象
        /// </summary>
        /// <returns></returns>
        T Get();
        
        /// <summary>
        /// 回收一个对象进入对象池
        /// </summary>
        /// <param name="obj"></param>
        void Recycle(T obj);
        
        /// <summary>
        /// 按要求清理对象池中的对象
        /// </summary>
        /// <param name="func"></param>
        void Clear(Predicate<T> func);
        
        /// <summary>
        /// 清楚对象池中所有的对象
        /// </summary>
        void Clear();
        
        /// <summary>
        /// 销毁对象池时该做什么
        /// </summary>
        void OnDestroy();
        
    }
}