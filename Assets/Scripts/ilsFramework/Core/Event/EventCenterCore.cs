using System;
using System.Collections.Generic;
using UnityEngine;

namespace ilsFramework.Core
{
    /// <summary>
    ///     这个是事件中心的核心，可以通过创建新实例来给不同的系统搭配事件中心
    ///     目的：不应该所有消息都从总事件中心里过，减少又长又臭的enum
    /// </summary>
    public class EventCenterCore
    {

        //用list防止整一堆相同的委托进去,貌似不能用hashset替换，同一个类型的不同实例的委托的hashcode貌似是一样的？，List.contain不止比对方法，还比较实例

        private Dictionary<string, List<Action<EventArgs>>> eventDic;

        private List<Action<EventArgs>> needRemoveBuffer;
        
        private List<Action<EventArgs>> needAddBuffer;
        
        private string IsExcutingEvent;
        
        public EventCenterCore()
        {
            eventDic = new Dictionary<string, List<Action<EventArgs>>>();
            
            needRemoveBuffer = new List<Action<EventArgs>>();
            needAddBuffer = new List<Action<EventArgs>>();
        }

        
        private List<Action<EventArgs>> GetEventList(string messageType)
        {
            if (eventDic.TryGetValue(messageType, out var results))
            {
                return results;
            }
            return null;
        }

        public void AddListener(string messageType, params Action<EventArgs>[] action)
        {
            if (messageType == IsExcutingEvent)
            {
                foreach (var a in action)
                {
                    needAddBuffer.Add(a);
                }
                return;
            }
            var actions = GetEventList(messageType);
            if (actions is not null)
            {
                foreach (var a in action)
                {
                    if (!actions.Contains(a))
                        actions.Add(a);
                    else
                        Debug.LogError($"添加重复的Listener,actionName: {action.GetType().Name}");
                }
            }
            else
            {
                eventDic.TryAdd(messageType, new List<Action<EventArgs>>());
                foreach (var a in action)
                {
                    eventDic[messageType].Add(a);
                }
            }
        }
        public void BroadcastMessage(string messageType, EventArgs eventArgs)
        {
            IsExcutingEvent = messageType;
            List<Action<EventArgs>> actions = GetEventList(messageType);

            if (actions is null) return;
            foreach (var action in actions)
            {
                try
                {
                    action?.Invoke(eventArgs);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                    
            }
            IsExcutingEvent = null;
            foreach (var a in needAddBuffer)
            {
                AddListener(messageType,a);
            }
            foreach (var a in needRemoveBuffer)
            {
                RemoveListener(messageType,a);
            }
            needAddBuffer.Clear();
            needRemoveBuffer.Clear();
        }
        public void RemoveListener(string messageType, params Action<EventArgs>[] action)
        {
            if (messageType == IsExcutingEvent)
            {
                foreach (var a in action)
                {
                    needRemoveBuffer.Add(a);
                }
                return;
            }
            List<Action<EventArgs>> actions = GetEventList(messageType);
            if (actions != null)
            {
                foreach (var a in action)
                {
                    actions.Remove(a);
                }

            }
        }
        public void RemoveListener(string messageType)
        {
            List<Action<EventArgs>> actions = GetEventList(messageType);
            if (actions != null)
            {
                actions.Clear();
            }
        }

        public void OnDestroy()
        {
            eventDic.Clear();
            eventDic = null;
        }
    }
}