using ilsFramework;
using ilsFrameWork;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace Test
{
    [UIPanelSetting(EUILayer.Debug,13,false,EAssetLoadMode.AssetKey,UIPanles.TestUIPanel)]
    public class TestUIPanel : UIPanel
    {
        [ShowInInspector]
        [AutoUIElement("Image")]
         Image testImage { get; set; }
    }
}