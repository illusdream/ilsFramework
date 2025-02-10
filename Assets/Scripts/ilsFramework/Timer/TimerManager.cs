using System;
using System.Collections.Generic;
using UnityEngine;

namespace ilsFramework
{
    public partial class TimerManager : ManagerSingleton<TimerManager>, IManager
    {
        private Dictionary<int, Timer> timers = new Dictionary<int, Timer>();
        private LinkedList<Timer> timerList = new LinkedList<Timer>();
        private Dictionary<int, Timer> addTimers = new Dictionary<int, Timer>();
        private HashSet<Timer> _needRemove = new HashSet<Timer>();
        
        private int timerIDCounter;
        
        public void Init()
        {
            
        }
        public void Update()
        {
            //加入缓存
            foreach (var addtimer in addTimers)
            {
                timerList.AddLast(addtimer.Value);
                timers.Add(addtimer.Key, addtimer.Value);
            }
            addTimers.Clear();
            if (timerList.Count > 0)
            {
                var currentNode = timerList.First;
                while (currentNode != null)
                {
                    if (currentNode.Value.IsFinish || _needRemove.Contains(currentNode.Value))
                    {
                        var remove = currentNode;
                        currentNode = currentNode.Next;
                        timers.Remove(remove.Value.ID);
                        _needRemove.Remove(remove.Value);
                        timerList.Remove(remove);
                    }
                    else
                    {
                        currentNode.Value.Update(Time.deltaTime);
                        currentNode = currentNode.Next;
                    }
                }
            }

        }
        public void LateUpdate()
        {


        }

        public void FixedUpdate()
        {

        }


        public void OnDestroy()
        {

        }

        public void OnDrawGizmos()
        {
            
        }

        public void OnDrawGizmosSelected()
        {
            
        }

        /// <summary>
        /// 添加一个新的计时器
        /// </summary>
        /// <param name="cycleTime">每次计时的时间</param>
        /// <param name="executingTimes">执行多少次，-1为无限执行次数</param>
        /// <param name="delayTime">延迟多久开启计时器</param>
        /// <param name="isFrameTimer">是否以一次Update更新为单位的计时</param>
        /// <param name="onStart">开启计时器时要做什么</param>
        /// <param name="onCompleted">完成一次计时时要做什么</param>
        /// <param name="onFinish">完成全部计时时要做什么</param>
        /// <param name="onCycling">在计时过程中的每一帧要做什么</param>
        /// <returns>对应的计时器实例</returns>
        public Timer RegisterTimer(float cycleTime, int executingTimes, float delayTime = 0, bool isFrameTimer = false, Action<Timer> onStart = null, Action<Timer> onCompleted = null, Action<Timer> onFinish = null, Action<Timer> onCycling = null)
        {
            int id =timerIDCounter;
            timerIDCounter++;
            Timer result = new Timer(id, cycleTime, delayTime, executingTimes, isFrameTimer, onStart, onCompleted, onFinish, onCycling);
            addTimers.Add(id, result);
            return result;
        }
        /// <summary>
        /// 添加一个新的计时器，如果计时器已存在，覆盖上一个计时器，创建一个新的计时器
        /// </summary>
        /// <param name="timer">计时器实例</param>
        /// <param name="cycleTime">每次计时的时间</param>
        /// <param name="executingTimes">执行多少次，-1为无限执行次数</param>
        /// <param name="delayTime">延迟多久开启计时器</param>
        /// <param name="isFrameTimer">是否以一次Update更新为单位的计时</param>
        /// <param name="onStart">开启计时器时要做什么</param>
        /// <param name="onCompleted">完成一次计时时要做什么</param>
        /// <param name="onFinish">完成全部计时时要做什么</param>
        /// <param name="onCycling">在计时过程中的每一帧要做什么</param>
        public void GetOrResetTimer(ref Timer timer, float cycleTime, int executingTimes, float delayTime = 0, bool isFrameTimer = false, Action<Timer> onStart = null, Action<Timer> onCompleted = null, Action<Timer> onFinish = null, Action<Timer> onCycling = null)
        {
            //取消掉前一个计时器
            if (timer is not null)
                RemoveTimer(timer);
            timer = RegisterTimer(cycleTime, executingTimes, delayTime, isFrameTimer, onStart, onCompleted, onFinish, onCycling);
        }
        /// <summary>
        /// 删除对应的计时器
        /// </summary>
        /// <param name="timer">计时器实例</param>
        public void RemoveTimer(Timer timer)
        {
            if (timer is not null)
                RemoveTimer(timer.ID);
        }
        /// <summary>
        /// 删除对应的计时器
        /// </summary>
        /// <param name="id">计时器对应的ID</param>
        public void RemoveTimer(int id)
        {
            if (timers.TryGetValue(id, out var timer))
            {
                timer.IsFinish = true;
            }
        }
        
        


    }
}
