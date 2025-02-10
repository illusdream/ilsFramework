using Sirenix.OdinInspector;

namespace ilsFramework
{
    public partial class AssetConfig
    {
#if UNITY_EDITOR
        [Button]
        public void OpenDetailAssetKeyWindow()
        {
            AssetManagerWindow.OpenWindow(this);
            
        }
        [Button]
        public void OpenChangeLogWindow()
        {
            AssetChangeLogWindow.OpenWindow();
        }
#endif
    }
}