namespace ilsFramework
{
    public enum ManagerPrioritySet
    {
        //公共Mono管理器，应该先更新所有Manager，再更新挂载的物体，所以这个优先级应该是最低的
        MonoManager,
        //资源管理器
        ResourceManager,
        //事件中心
        EventCenter,
        //流程管理，第一个流程应该加载基础数据，反正也很重要
        ProduceManager,
    }
}
