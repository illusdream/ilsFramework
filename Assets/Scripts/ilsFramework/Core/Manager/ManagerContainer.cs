using Sirenix.OdinInspector;
using UnityEngine;

namespace ilsFramework.Core
{
    /// <summary>
    ///     就是个给hire窗口单独查看Manager的类，什么都不用写
    /// </summary>
    [HideMonoScript]
    public class ManagerContainer : MonoBehaviour
    {
        [ShowInInspector] [HideLabel] [HideReferenceObjectPicker]
        public IManager Manager;
    }
}