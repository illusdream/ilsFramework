using System;

namespace ilsFramework.Core
{
    public class GlobalEventCenter : ManagerSingleton<GlobalEventCenter>, IManager, IEventCenter
    {
        private EventCenterCore _eventCenterCore;

        public void AddListener(string messageType, params Action<EventArgs>[] action)
        {
            _eventCenterCore.AddListener(messageType, action);
        }

        public void BoradCastMessage(string messageType, EventArgs eventArgs)
        {
            _eventCenterCore.BoradCastMessage(messageType, eventArgs);
        }

        public void RemoveListener(string messageType, params Action<EventArgs>[] action)
        {
            _eventCenterCore.RemoveListener(messageType, action);
        }


        public void Init()
        {
            _eventCenterCore = new EventCenterCore();
        }

        public void Update()
        {
        }

        public void LateUpdate()
        {
        }

        public void LogicUpdate()
        {
        }

        public void FixedUpdate()
        {
        }

        public void OnDestroy()
        {
            _eventCenterCore.OnDestroy();
        }

        public void OnDrawGizmos()
        {
        }

        public void OnDrawGizmosSelected()
        {
        }
    }
}