using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ilsFramework
{
    [Serializable]
    [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
    public class AudioChannelData
    {       
        /// <summary>
        /// Channel 名字
        /// </summary>
        [TitleGroup("Channel基础设置")]
        public string Name;
        /// <summary>
        /// Channel 运行时使用的实例类型
        /// </summary>
        [TitleGroup("Channel基础设置")]
        public AudioChannelType AudioChannelType;
        /// <summary>
        /// 该Channel 最大容量
        /// </summary>
        [TitleGroup("Channel基础设置")]
        public int MaxAudioClipCount;
        
        /// <summary>
        /// 该Channel 播放声音输出到哪个MixerGroup，该MixerGroup名字
        /// </summary>
        [TitleGroup("Mixer设置")]
        public string MixerGroupName;
        /// <summary>
        /// 该Channel 对应的MixerGroup 控制声音大小的变量
        /// </summary>
        [TitleGroup("Mixer设置")]
        public string MixerVolumeParameterName;
        
        [PropertyTooltip("这个Channel对应的音量下限，单位dB")]
        [TitleGroup("Mixer设置/音量范围",horizontalLine:false)]
        [LabelText("最小值")]
        public float ChannelVolumeMin = -80;
        [PropertyTooltip("这个Channel对应的音量上限，单位dB")]
        [TitleGroup("Mixer设置/音量范围",horizontalLine:false)]
        [LabelText("最大值")]
        public float ChannelVolumeMax =0;
    }
}