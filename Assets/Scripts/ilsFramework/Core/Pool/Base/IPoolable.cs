namespace ilsFramework.Core
{
    public interface IPoolable
    {
        void OnGet();
        void OnRecycle();
        void OnPoolDestroy();
    }
}