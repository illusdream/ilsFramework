using Sirenix.OdinInspector;
using UnityEngine;

namespace ilsFramework.Core
{
    [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
    public abstract class ConfigScriptObject : ScriptableObject
    {
        [Title("$GetConfigName", titleAlignment: TitleAlignments.Centered)] [HideLabel] [PropertyOrder(int.MinValue)] [ShowInInspector]
        private title configName;

        public abstract string ConfigName { get; }

        public string GetConfigName()
        {
            return ConfigName;
        }

        [DisableInInlineEditors]
        private struct title
        {
        }
    }
}