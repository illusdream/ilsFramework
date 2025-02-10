using System.Collections.Generic;

namespace ilsFramework
{
    public class TimerCollection
    {
        private Dictionary<string, Timer> _timers;

        public TimerCollection()
        {
            _timers = new Dictionary<string, Timer>();
        }

        public TimerBuilder CreateTimer(float cycleTime,int executingTimes,string timerKey)
        {
            return new TimerBuilder(cycleTime, executingTimes, this, timerKey);
        }
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

        public void ClearAllTimers()
        {
            foreach (var timer in _timers.Values)
            {
                TimerManager.Instance.RemoveTimer(timer);
            }
        }
    }
}