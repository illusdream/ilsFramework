using System;

namespace ilsFramework.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class UIPanelSetting : Attribute
    {
        public EAssetLoadMode AssetLoadMode;
        public int LayerOffest;
        public string LoadAssetStr;
        public bool ShoundCached;
        public EUILayer UILayer;


        public UIPanelSetting(EUILayer uiLayer, int layerOffest, bool shoundCached, EAssetLoadMode assetLoadMode, string loadAssetStr)
        {
            UILayer = uiLayer;
            LayerOffest = layerOffest;
            ShoundCached = shoundCached;
            AssetLoadMode = assetLoadMode;
            LoadAssetStr = loadAssetStr;
        }
    }
}