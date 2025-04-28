namespace ilsFramework.Core
{
    public enum ETimerType
    {
        /// <summary>
        /// 受到Time.Scale影响的计时器
        /// </summary>
        TimeScale,
        /// <summary>
        /// 帧计时器，以每次Update作为一帧(这个应该要改改，弄成以逻辑帧为单位好点)
        /// </summary>
        FramedByUpdate,
        /// <summary>
        /// 按真实时间计时的计时器(不受Time.Scale影响)
        /// </summary>
        RealTime
    }
}