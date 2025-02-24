using ilsFramework;
using UnityEngine;

namespace Test
{
    [DebugCommend("Add")]
    public class TestCommend_Add : ICommendSet
    {
        public void Add(Vector2 value,Vector2 value2)
        {
            $"ADD 2 个: {value} + {value2} = {value + value2}".LogSelf();
        }
        
        public void Add(int a, int b, int c = 10)
        {
            $"ADD 3 个: {a} + {b} + {c} = {a + b + c}".LogSelf();
        }
    }
}