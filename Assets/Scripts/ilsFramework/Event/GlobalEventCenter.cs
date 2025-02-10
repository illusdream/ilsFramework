using System;

namespace ilsFramework
{
    public class GlobalEventCenter : ManagerSingleton<GlobalEventCenter>, IManager, IEventCenter
    {

        EventCenterCore _eventCenterCore;

        public int Priority => (int)ManagerPrioritySet.EventCenter;



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


    }
}
