using Sirenix.OdinInspector;

namespace ilsFramework
{
    [AutoBuildOrLoadConfig("DebugConfig")]
    public class DebugConfig : ConfigScriptObject
    {
        public override string ConfigName => "Debug";
        
        [LabelText("Debug控制台Log存储数目")]
        public int MaxDebugUIStoreLogCount = 60;
    }
}