using System;
using System.IO;

namespace ilsFramework
{
    /// <summary>
    /// 计时器制造类
    /// 链式调用创建计时器
    /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cycleTime">单次循环时间</param>
        /// <param name="executingTimes">循环次数</param>
        /// <param name="timerCollection">需要存放入的timerCollection</param>
        /// <param name="timerCollectionKey">在timerCollection中的Key</param>
        public TimerBuilder(float cycleTime, int executingTimes,TimerCollection timerCollection = null,string timerCollectionKey = null)
        {
            this.cycleTime = cycleTime;
            this.executingTimes = executingTimes;
            _timerCollection = timerCollection;
            this.timerCollectionKey = timerCollectionKey;
        }
        
        /// <summary>
        /// 设置循环时间
        /// </summary>
        /// <param name="cycleTime">循环时间（以秒为单位）</param>
        /// <returns></returns>
        public TimerBuilder SetCycleTime(float cycleTime)
        {
            this.cycleTime = cycleTime;
            return this;
        }
        /// <summary>
        /// 设置延迟开始计时器时间
        /// </summary>
        /// <param name="delayTime">延迟时间（以秒为单位）</param>
        /// <returns></returns>
        public TimerBuilder SetDelayTime(float delayTime)
        {
            this.delayTime = delayTime;
            return this;
        }
        /// <summary>
        /// 设置循环执行次数的时间
        /// </summary>
        /// <param name="count">执行次数</param>
        /// <returns></returns>
        public TimerBuilder SetExecutingTimes(int count)
        {
            this.executingTimes = count;
            return this;
        }
        /// <summary>
        /// 是否使用帧计时器
        /// 如果使用，cycleTime的单位将转换为FixdedUpdate帧
        /// </summary>
        /// <param name="isFrameTimer"></param>
        /// <returns></returns>
        public TimerBuilder SetIsFrameTimer(bool isFrameTimer)
        {
            this.isFrameTimer = isFrameTimer;
            return this;
        }
        /// <summary>
        /// 设置开启计时器时的时间
        /// </summary>
        /// <param name="onStart"></param>
        /// <returns></returns>
        public TimerBuilder SetOnStart(Action<Timer> onStart)
        {
            this.onStart = onStart;
            return this;
        }
        /// <summary>
        /// 设置完成单次计时的事件
        /// </summary>
        /// <param name="onCompleted"></param>
        /// <returns></returns>
        public TimerBuilder SetOnCompleted(Action<Timer> onCompleted)
        {
            this.onCompleted = onCompleted;
            return this;
        }
        /// <summary>
        /// 完成所有循环后的事件
        /// </summary>
        /// <param name="onFinish"></param>
        /// <returns></returns>
        public TimerBuilder SetOnFinish(Action<Timer> onFinish)
        {
            this.onFinish = onFinish;
            return this;
        }
        /// <summary>
        /// 在循环过程中的事件(每次Update都触发)
        /// </summary>
        /// <param name="onCycling"></param>
        /// <returns></returns>
        public TimerBuilder SetOnCycling(Action<Timer> onCycling)
        {
            this.onCycling = onCycling;
            return this;
        }
        /// <summary>
        /// 将计时器注册到系统
        /// </summary>
        /// <returns>此次注册的计时器</returns>
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