using System;

namespace ilsFramework.Core
{
    public class GlobalEventCenter : ManagerSingleton<GlobalEventCenter>, IEventCenter
    {
        private EventCenterCore _eventCenterCore;

        public void AddListener(string messageType, params Action<EventArgs>[] action)
        {
            _eventCenterCore.AddListener(messageType, action);
        }

        public void BroadcastMessage(string messageType, EventArgs eventArgs)
        {
            _eventCenterCore.BroadcastMessage(messageType, eventArgs);
        }

        public void RemoveListener(string messageType, params Action<EventArgs>[] action)
        {
            _eventCenterCore.RemoveListener(messageType, action);
        }
        public override void OnInit()
        {
            _eventCenterCore = new EventCenterCore();
        }

        public override void OnUpdate()
        {
            
        }

        public override void OnLateUpdate()
        {
            
        }

        public override void OnLogicUpdate()
        {
           
        }
        public override void OnFixedUpdate()
        {
            
        }

        public override void OnDestroy()
        {
            _eventCenterCore.OnDestroy();
        }

        public override void OnDrawGizmos()
        {
            
        }
        public override void OnDrawGizmosSelected()
        {
            
        }
    }
}