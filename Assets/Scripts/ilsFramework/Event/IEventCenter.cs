using System;

namespace ilsFramework
{
    /// <summary>
    /// 事件中心的接口，总之就是懒，这个更像模板，防止我忘了格式
    /// </summary>
    public interface IEventCenter
    {
        public void AddListener(string messageType, params Action<EventArgs>[] action);
        public void BoradCastMessage(string messageType, EventArgs eventArgs);
        public void RemoveListener(string messageType, params Action<EventArgs>[] action);
    }
}
