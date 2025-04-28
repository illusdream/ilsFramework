using System;

namespace ilsFramework.Core
{
    public interface IManagerSingleton
    {
        public int ManagerUpdateIndex { get; set; }

        public void CreateAllNeedModules(Type[] needModules);
    }
}