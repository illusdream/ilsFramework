using System.Collections.Generic;
using UnityEngine;

namespace ilsFramework
{
    public class TimerCollection
    {
        private Dictionary<string, Timer> _timers;

        public TimerCollection()
        {
            _timers = new Dictionary<string, Timer>();
        }

        /// <summary>
        /// 创建一个新的计时器
        /// </summary>
        /// <param name="cycleTime">单次循环时间</param>
        /// <param name="executingTimes">循环次数</param>
        /// <param name="timerKey">存储到Collection中的key</param>
        /// <returns></returns>
        public TimerBuilder CreateTimer(float cycleTime,int executingTimes,string timerKey)
        {
            return new TimerBuilder(cycleTime, executingTimes, this, timerKey);
        }
        /// <summary>
        /// 添加一个新的Timer至Collection中
        /// </summary>
        /// <param name="timerName"></param>
        /// <param name="timer"></param>
        public void AddTimer(string timerName,Timer timer)
        {
            if (_timers.TryGetValue(timerName, out Timer existingTimer))
            {
                TimerManager.Instance.RemoveTimer(existingTimer);
                _timers[timerName] = timer;
            }
            else
            {
                _timers.Add(timerName, timer);
            }
        }
        /// <summary>
        /// 将对应Key的Timer注销
        /// </summary>
        /// <param name="timerName"></param>
        public void RemoveTimer(string timerName)
        {
            if (_timers.TryGetValue(timerName, out Timer timer))
            {
                TimerManager.Instance.RemoveTimer(timer);
            }
        }

        public Timer this[string timerName]
        {
            get
            {
                if (_timers.TryGetValue(timerName, out Timer timer))
                {
                    return timer;
                }
                return null;
            }
            set => AddTimer(timerName, value);
        }
        /// <summary>
        /// 清除所有计时器
        /// </summary>
        public void ClearAllTimers()
        {
            foreach (var timer in _timers.Values)
            {
                TimerManager.Instance.RemoveTimer(timer);
            }
            _timers.Clear();
        }
    }
}