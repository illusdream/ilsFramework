using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ilsFramework.Core
{
    [CreateAssetMenu(fileName = "LogConfig", menuName = "LogConfig")]
    [AutoBuildOrLoadConfig("LogConfigs/LogConfig")]
    public class LogConfig : ConfigScriptObject
    {
        [BoxGroup("Log输出")] public bool DebugLogEnable = true;

        [BoxGroup("Log输出")] public bool WanringLogEnable = true;

        [BoxGroup("Log输出")] public bool ErrorLogEnable = true;

        [BoxGroup("LogColor")] public List<ColorNameBind> ColorNameBinds = new();

        public override string ConfigName => "日志配置";

        public void OnValidate()
        {
        }

        public Dictionary<string, string> BuildConfigColorDic()
        {
            var dic = new Dictionary<string, string>();
            foreach (var colorNameBind in ColorNameBinds) dic.Add(colorNameBind.Name, "#" + ColorUtility.ToHtmlStringRGB(colorNameBind.Color));
            return dic;
        }
#if UNITY_EDITOR
        [BoxGroup("LogColor")]
        [Button("生成LogColor帮助类")]
        public void GenerateColorReference()
        {
            var scriptGenerator = new ScriptGenerator("ilsFramework");
            var classGenerator = new ClassGenerator(EAccessType.Public, "LogColor");
            foreach (var bind in ColorNameBinds)
            {
                var instance = new StringFieldGenerator(EFieldDeclarationMode.Const, EAccessType.Public, bind.Name,
                    $"#{ColorUtility.ToHtmlStringRGB(bind.Color)}");
                classGenerator.Append(instance);
            }

            scriptGenerator.Append(classGenerator);
            scriptGenerator.GenerateScript("LogColor");
            AssetDatabase.Refresh();
        }
#endif
        [Serializable]
        public class ColorNameBind
        {
            public string Name;
            public Color Color = Color.white;
        }
    }
}