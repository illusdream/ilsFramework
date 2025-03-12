using ilsFramework;
using ilsFrameWork;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    [UIPanelSetting(EUILayer.Debug,13,false,EAssetLoadMode.AssetBundle,"test/22/testuipanel/TestUIPanel")]
    public class TestUIPanel : UIPanel
    {
        [ShowInInspector]
        [AutoUIElement("Image")]
         Image testImage { get; set; }
         [AutoUIElement("TEst")]
         Transform testTransform;
         

    }
}