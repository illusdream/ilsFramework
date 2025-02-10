using Sirenix.OdinInspector;
using UnityEngine;

namespace ilsFramework
{
    /// <summary>
    /// 就是个给hire窗口单独查看Manager的类，什么都不用写
    /// </summary>
    public class ManagerContainer : MonoBehaviour
    {
        [ShowInInspector]
        public IManager Manager;
    }
}

