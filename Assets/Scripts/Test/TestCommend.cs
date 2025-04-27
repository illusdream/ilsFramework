using ilsFramework;
using ilsFramework.Core;

namespace Test
{
    [DebugCommend("LogTest")]
    public class TestCommend : ICommendSet
    {
        public void LogTest()
        {
            "测试".LogSelf();
        }
    }
}