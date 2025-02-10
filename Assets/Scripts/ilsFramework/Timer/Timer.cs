using System;
using Unity.Mathematics;

namespace ilsFramework
{
    /// <summary>
    /// 计时器
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// 通过Guid获取单独的id，并以此作为dic的键名
        /// </summary>
        public int ID;
        /// <summary>
        /// 计时字段，每次自增来检测
        /// </summary>
        private float time = 0;
        /// <summary>
        /// 计时字段
        /// </summary>
        public float Time => time;

        /// <summary>
        /// 周期时间，每次两次执行间间隔的时间
        /// </summary>
        private float _cycleTime = 1f;
        /// <summary>
        /// 延迟时间，在延迟之后才会进入计时循环
        /// </summary>
        private float _delayTime = 0;
        /// <summary>
        /// 计时器进度，每次循环过程中的进度
        /// </summary>
        public float Progress => IsFinish ? 1 : math.clamp(time / _cycleTime, 0, 1);
        /// <summary>
        /// 是否循环
        /// </summary>
        public bool IsLoop => _executingTimes < 0;
        /// <summary>
        /// 执行次数,每次执行函数后自动-1，值为0的时候为执行完毕，&lt;0时为循环操作
        /// </summary>
        private int _executingTimes;
        /// <summary>
        /// 是否完成，如果完成了就会进入删除队列,不循环的没必要保留，下次使用的时候再申请一个就行了
        /// </summary>
        public bool IsFinish = false;
        /// <summary>
        /// 是否为帧计时器，采用每帧+1的方式计算
        /// </summary>
        public bool IsFrameTimer;
        /// <summary>
        /// 在计时器开启时调用一次，即delay《=0时
        /// </summary>
        public Action<Timer> OnStart;
        /// <summary>
        /// 每次循环完成调用一次
        /// </summary>
        public Action<Timer> OnCompleted;
        /// <summary>
        /// 整个计时流程完成后调用
        /// </summary>
        public Action<Timer> OnFinish;
        /// <summary>
        /// 在循环过程中每帧调用
        /// </summary>
        public Action<Timer> OnCycling;

        /// <summary>
        /// 不要在这里创建计时器，在<see cref="TimerManager"/>中使用<see href="TimerManager.RegisterTimer"/> 或<see href="TimerManager.GetOrResetTimer"/>创建
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="cycleTime"></param>
        /// <param name="delayTime"></param>
        /// <param name="executingTimes"></param>
        /// <param name="isFrameTimer"></param>
        /// <param name="onStart"></param>
        /// <param name="onCompleted"></param>
        /// <param name="onFinish"></param>
        /// <param name="onCycling"></param>
        public Timer(int iD, float cycleTime, float delayTime, int executingTimes, bool isFrameTimer, Action<Timer> onStart, Action<Timer> onCompleted, Action<Timer> onFinish, Action<Timer> onCycling)
        {
            ID = iD;
            IsFinish = false;
            _cycleTime = cycleTime;
            _delayTime = delayTime;
            _executingTimes = executingTimes;
            IsFrameTimer = isFrameTimer;
            OnStart = onStart;
            OnCompleted = onCompleted;
            OnFinish = onFinish;
            OnCycling = onCycling;
        }

        public void Update(float dt)
        {
            if (IsFinish)
                return;
            if (_delayTime > 0)
            {
                if (IsFrameTimer)
                {
                    _delayTime--;
                }
                else
                {
                    _delayTime -= dt;
                }


                if (_delayTime <= 0)
                {
                    OnStart?.Invoke(this);
                }
                else
                {
                    return;
                }
            }
            if (_executingTimes != 0)
            {
                if (IsFrameTimer)
                {
                    time++;
                }
                else
                {
                    time += dt;
                }
                if (time >= _cycleTime)
                {
                    _executingTimes--;

                    time = 0;

                    OnCompleted?.Invoke(this);
                    return;
                }
                OnCycling?.Invoke(this);
            }
            else
            {
                IsFinish = true;
                OnFinish?.Invoke(this);
            }
        }

        public void Reset(float cycleTime, float delayTime, int executingTimes, bool isFrameTimer, Action<Timer> onStart, Action<Timer> onCompleted, Action<Timer> onFinish, Action<Timer> onCycling)
        {
            IsFinish = false;
            time = 0;
            _cycleTime = cycleTime;
            _delayTime = delayTime;
            _executingTimes = executingTimes;
            IsFrameTimer = isFrameTimer;
            OnStart = onStart;
            OnCompleted = onCompleted;
            OnFinish = onFinish;
            OnCycling = onCycling;
        }


    }
}
