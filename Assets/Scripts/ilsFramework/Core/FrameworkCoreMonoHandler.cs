using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ilsFramework.Core
{
    /// <summary>
    /// 框架核心与Unity 更新的对接器
    /// </summary>
    [HideLabel]
    [HideMonoScript]
    public class FrameworkCoreMonoHandler : MonoBehaviour
    {
        [ShowInInspector]
        [HideLabel]
        public FrameworkCore FrameworkCore=> FrameworkCore.Instance;
        
        [RuntimeInitializeOnLoadMethod]
        private static void InitializeFramework()
        {
            //挂载
             var instance = new GameObject("ilsFramework");
            DontDestroyOnLoad(instance);
            instance.AddComponent<FrameworkCoreMonoHandler>();
        }
        
        public void Awake()
        {
            FrameworkCore.frameworkGOBaseTransform = transform;
            Core.FrameworkCore.Instance.Initialize();
        }

        public void Start()
        {
            
        }

        public void Update()
        {
            FrameworkCore.Instance.Update();
        }

        public void LateUpdate()
        {
            FrameworkCore.Instance.LateUpdate();
        }

        public void FixedUpdate()
        {
            FrameworkCore.Instance.FixedUpdate();
        }

        public void OnDestroy()
        {
            FrameworkCore.Instance.OnDestroy();
        }

        public void OnDrawGizmos()
        {
            FrameworkCore.Instance.OnDrawGizmos();
        }

        public void OnDrawGizmosSelected()
        {
            FrameworkCore.Instance.OnDrawGizmosSelected();
        }
    }
}