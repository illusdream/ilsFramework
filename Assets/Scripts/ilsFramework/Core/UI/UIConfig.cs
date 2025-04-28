using UnityEngine;

namespace ilsFramework.Core
{
    [AutoBuildOrLoadConfig("UIConfig")]
    public class UIConfig : ConfigScriptObject
    {
        public override string ConfigName => "UIConfig";

        public GameObject UIEventHandler;
    }
}