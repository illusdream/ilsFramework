namespace ilsFramework.Core
{
    /// <summary>
    /// 主要作用是将管理器的功能内聚，防止在一个地方堆积过多功能
    /// 管理器模块，由每个管理器单独创建并使用，同时外部无法获取到模块，Manager作为中间层用以传递模块功能
    /// </summary>
    public abstract class ManagerModule
    {
        public abstract int Priority { get; }
        
        public bool Enabled { get; set; }

        public virtual void OnEnable()
        {
            
        }

        public virtual void OnDisable()
        {
            
        }
        
        public virtual void OnInit()
        {
            
        }

        public virtual void OnUpdate()
        {
            
        }

        public virtual void OnLateUpdate()
        {
            
        }

        public virtual void OnLogicUpdate()
        {
            
        }

        public virtual void OnFixedUpdate()
        {
            
        }

        public virtual void OnDestroy()
        {
            
        }

        public virtual void OnDrawGizmos()
        {
            
        }

        public virtual void OnDrawGizmosSelected()
        {
            
        }
    }
}