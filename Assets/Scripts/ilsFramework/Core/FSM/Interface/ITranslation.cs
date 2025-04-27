namespace ilsFramework.Core
{
    /// <summary>
    ///     转换条件
    /// </summary>
    public interface ITranslation
    {
        int Priority { get; }

        string Name { get; }

        /// <summary>
        ///     在什么条件下可以触发转换
        /// </summary>
        /// <returns></returns>
        bool CanTranslate();

        /// <summary>
        ///     在条件触发时要对外界做出什么反应（回调）
        /// </summary>
        void OnTranslate();
    }
}