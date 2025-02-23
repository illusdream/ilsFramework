using System;

namespace ilsFramework
{
    [AttributeUsage(AttributeTargets.Class,Inherited = false, AllowMultiple = false)]
    public class UIPanelSetting : Attribute
    {
        public EUILayer UILayer;
        public int LayerOffest;
        public bool ShoundCached;
        public EAssetLoadMode AssetLoadMode;
        public string LoadAssetStr;


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