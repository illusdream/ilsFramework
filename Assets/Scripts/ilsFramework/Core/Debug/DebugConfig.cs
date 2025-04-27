using Sirenix.OdinInspector;

namespace ilsFramework.Core
{
    [AutoBuildOrLoadConfig("DebugConfig")]
    public class DebugConfig : ConfigScriptObject
    {
        [LabelText("Debug控制台Log存储数目")] public int MaxDebugUIStoreLogCount = 60;

        public override string ConfigName => "Debug";
    }
}