using System;
using System.IO;

namespace ilsFramework
{
    public class TimerBuilder
    {
        private TimerCollection _timerCollection;
        private string timerCollectionKey;
        private float cycleTime;
        private float delayTime =0;
        private int executingTimes;
        private bool isFrameTimer =false;
        private Action<Timer> onStart = null;
        private Action<Timer> onCompleted =null;
        private Action<Timer> onFinish =null;
        private Action<Timer> onCycling =null;

        public TimerBuilder(float cycleTime, int executingTimes,TimerCollection timerCollection = null,string timerCollectionKey = null)
        {
            this.cycleTime = cycleTime;
            this.executingTimes = executingTimes;
            _timerCollection = timerCollection;
            this.timerCollectionKey = timerCollectionKey;
        }
        

        public TimerBuilder SetCycleTime(float cycleTime)
        {
            this.cycleTime = cycleTime;
            return this;
        }

        public TimerBuilder SetDelayTime(float delayTime)
        {
            this.delayTime = delayTime;
            return this;
        }

        public TimerBuilder SetExecutingTimes(int count)
        {
            this.executingTimes = count;
            return this;
        }

        public TimerBuilder SetIsFrameTimer(bool isFrameTimer)
        {
            this.isFrameTimer = isFrameTimer;
            return this;
        }

        public TimerBuilder SetOnStart(Action<Timer> onStart)
        {
            this.onStart = onStart;
            return this;
        }

        public TimerBuilder SetOnCompleted(Action<Timer> onCompleted)
        {
            this.onCompleted = onCompleted;
            return this;
        }

        public TimerBuilder SetOnFinish(Action<Timer> onFinish)
        {
            this.onFinish = onFinish;
            return this;
        }

        public TimerBuilder SetOnCycling(Action<Timer> onCycling)
        {
            this.onCycling = onCycling;
            return this;
        }
        
        public Timer Register()
        {
            var result =  TimerManager.Instance.RegisterTimer(cycleTime,executingTimes,delayTime,isFrameTimer,onStart,onCompleted,onFinish,onCycling);
            if (_timerCollection != null && timerCollectionKey != null)
            {
                _timerCollection.AddTimer(timerCollectionKey, result);
            }
            return result;
        }
    }
}