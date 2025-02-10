namespace ilsFramework
{
    public interface IPoolable
    {
        void OnGet();
        void OnRecycle();
        void OnPoolDestroy();
    }
}