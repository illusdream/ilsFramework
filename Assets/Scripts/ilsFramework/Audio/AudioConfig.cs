using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ilsFramework
{
    [AutoBuildOrLoadConfig("AudioConfig")]
    public class AudioConfig : ConfigScriptObject
    {
        public override string ConfigName => "音频配置";
        
        public string AudioMixerPath ="Mixer/MainAudioMixe";
        
        [PropertyTooltip("主音量对应的音量下限，单位dB")]
        [TitleGroup("主音量范围")]
        [LabelText("最小值")]
        public float MainChannelVolumeMin = -80;
        [PropertyTooltip("主音量对应的音量上限，单位dB")]
        [TitleGroup("主音量范围")]
        [LabelText("最大值")]
        public float MainChannelVolumeMax =0;
        
        [SerializeField]
        [ListDrawerSettings(ShowFoldout = false)]
        [LabelText("Channel设置")]
        public List<AudioChannelData> audioChannelDatas = new List<AudioChannelData>();

        #if UNITY_EDITOR
        [Button("更新配置")]
        private void GenerateAudioSetting()
        {
            ScriptGenerator generator = new ScriptGenerator("ilsFramework");
            ClassGenerator audioNameClassGenerator = new ClassGenerator(EAccessType.Public, "AudioChannelName");
            foreach (var audioChannelData in audioChannelDatas)
            {
                StringFieldGenerator stringFieldGenerator = new StringFieldGenerator(EFieldDeclarationMode.Const,EAccessType.Public,audioChannelData.Name,audioChannelData.Name);
                audioNameClassGenerator.Append(stringFieldGenerator);
            }
            generator.Append(audioNameClassGenerator);

            EnumGenerator ChannelTypeGenerator = new EnumGenerator(EAccessType.Public, "AudioChannelType", description: null);
            var types = Assembly.GetExecutingAssembly().GetTypes();
            //找出对应需要遍历程序集
            foreach (var type in types)
            {
                if (typeof(AudioChannel).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract )
                {
                    ChannelTypeGenerator.Append((type.Name,null));
                }
            }
            generator.Append(ChannelTypeGenerator);
            generator.GenerateScript("AudioSetting");
            AssetDatabase.Refresh();
        }
        #endif
    }
}