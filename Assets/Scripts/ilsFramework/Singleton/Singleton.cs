namespace ilsFramework
{
    /// <summary>
    /// C#基本单例模式
    /// </summary>
    /// <typeparam name="T">对应的单例类型</typeparam>
    public class Singleton<T> where T : new()
    {
        private static T _instance;
        private static readonly object locker = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (locker)
                    {
                        _instance ??= new T();
                    }
                }
                return _instance;
            }
        }
    }
}
