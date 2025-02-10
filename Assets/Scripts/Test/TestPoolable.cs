using ilsFramework;
using UnityEngine;

namespace Test
{
    public class TestPoolable : MonoBehaviour,IPoolable
    {
        public void OnGet()
        {
            Debug.Log("OnGet");
        }

        public void OnRecycle()
        {
           Debug.Log("OnRecycle");
        }

        public void OnPoolDestroy()
        {
           Debug.Log("OnPoolDestroy");
        }
    }
}