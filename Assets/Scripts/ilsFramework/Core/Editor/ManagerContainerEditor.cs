using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace ilsFramework.Core.Editor
{
    [CustomEditor(typeof(ManagerContainer))]
    public class ManagerContainerEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}