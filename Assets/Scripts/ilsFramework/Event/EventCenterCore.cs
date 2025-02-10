using System;
using System.Collections.Generic;
using UnityEngine;

namespace ilsFramework
{
    /// <summary>
    /// 这个是事件中心的核心，可以通过创建新实例来给不同的系统搭配事件中心
    /// 目的：不应该所有消息都从总事件中心里过，减少又长又臭的enum
    /// </summary>
    public class EventCenterCore
    {

        //用list防止整一堆相同的委托进去,貌似不能用hashset替换，同一个类型的不同实例的委托的hashcode貌似是一样的？，List.contain不止比对方法，还比较实例

        private Dictionary<string, List<Action<EventArgs>>> eventDic;

        //存储命名空间，减少反射消耗？不确定，以后做测试看看

        public EventCenterCore()
        {
            eventDic = new Dictionary<string, List<Action<EventArgs>>>();
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
        public void BoradCastMessage(string messageType, EventArgs eventArgs)
        {
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
        }
        public void RemoveListener(string messageType, params Action<EventArgs>[] action)
        {
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

