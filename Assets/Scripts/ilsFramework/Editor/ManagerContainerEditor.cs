using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace ilsFramework.Editor
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