using System;
using System.Collections.Generic;

namespace ilsFramework.Core
{
    public class ReferencePool
    {
        public bool CollectionCheck { get; set; }
        
        protected Queue<IPoolable> _pool;

        protected HashSet<IPoolable> contains;

        protected int activeObjectCount;
        
        protected int deactiveObjectCount;
        
        protected int allocatedObjectCount;
        protected virtual Type referenceType {get; set;}

        protected List<IPoolable> clearBuffer;
        
        public ReferencePool()
        {
            _pool = new Queue<IPoolable>();
            contains = new HashSet<IPoolable>();
            clearBuffer = new List<IPoolable>();
        }
        
        public int GetObjectCount()
        {
            return _pool.Count;
        }
        
        public int GetActiveObjectCount()
        {
            return activeObjectCount;
        }

        public int GetDeActiveObjectCount()
        {
           return deactiveObjectCount;
        }

        public int GetAllObjectCount()
        {
            return allocatedObjectCount;
        }

        public IPoolable Get()
        {
            if (_pool.Count > 0 )
            {
                activeObjectCount++;
                deactiveObjectCount--;
                var _instance = _pool.Dequeue();
                contains.Remove(_instance);
                _instance.OnGet();
                return _instance;
            }
            
            activeObjectCount++;
            allocatedObjectCount++;
            var instance = (IPoolable)Activator.CreateInstance(referenceType);
            instance.OnGet();
            return instance;
            
        }

        public void Recycle(IPoolable obj)
        {
            if (CollectionCheck && contains.Contains(obj))
            {
                $"引用池{referenceType}中已存在对应实例，无法二次回收".ErrorSelf();
            }
            
            obj.OnRecycle();
            _pool.Enqueue(obj);
            contains.Add(obj);
            
            activeObjectCount--;
            deactiveObjectCount++;
        }

        public void Clear()
        {
           _pool.Clear();
           contains.Clear();
           
           deactiveObjectCount = 0;
           allocatedObjectCount = 0;
        }

        public void OnDestroy()
        {
            foreach (var poolable in _pool)
            {
                poolable.OnPoolDestroy();
            }
            _pool.Clear();
            contains.Clear();
            clearBuffer.Clear();
            
            _pool = null;
            contains = null;
            clearBuffer = null;
        }
    }

    public class ReferencePool<T> : ReferencePool,IPool<T> where T : class,IPoolable
    {
        protected override Type referenceType => typeof(T);

        public int GetMaxCapacity()
        {
            return int.MaxValue;
        }

        public T Get()
        {
            if (CollectionCheck && (referenceType.IsAbstract || typeof(IPoolable).IsAssignableFrom(referenceType)))
            {
                $"类型{referenceType}无法实例化或者未继承{typeof(IPoolable)}接口".ErrorSelf();
                return null;
            }
            return (T)base.Get();
        }

        public void Recycle(T obj)
        {
            if (CollectionCheck && (referenceType.IsAbstract || typeof(IPoolable).IsAssignableFrom(referenceType)))
            {
                $"类型{referenceType}无法实例化或者未继承{typeof(IPoolable)}接口".ErrorSelf();
                return;
            }
            base.Recycle(obj);
        }

        public void Clear(Predicate<T> func)
        {
            if (func == null)
            {
                Clear();
                return;
            }
            
            clearBuffer.Clear();
            foreach (var poolable in _pool)
            {
                if (func((T)poolable))
                {
                    clearBuffer.Add(poolable);
                }
            }

            foreach (var poolable in clearBuffer)
            {
                _pool.Enqueue(poolable);
                contains.Remove(poolable);
                deactiveObjectCount--;
                allocatedObjectCount--;
            }
        }
    }
}