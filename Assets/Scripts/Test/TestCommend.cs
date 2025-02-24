using ilsFramework;

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