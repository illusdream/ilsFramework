using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace ilsFramework
{
    public class AudioManager : ManagerSingleton<AudioManager>,IManager,IAssemblyForeach
    {       
        public const string AudioSettingPath = "Assets/Scripts/ilsFramework/Audio/AudioSetting.cs";
        public const string AudioClipDataConfigsPath = "Assets/Resources/Audio/AudioClipDataConfigs.asset";
        public const string AudioChannelAssetsPath = "Assets/Resources/Audio/AudioChannelAssets.asset";

        public const string AudioChannelAssetsLoadPath = "Audio/AudioChannelAssets";
        public const string AudioClipDataConfigsLoadPath = "Audio/AudioClipDataConfigs";

        
        /// <summary>
        /// 存储独立的audioClip实例，防止重复加载
        /// </summary>
        Dictionary<string,AudioClip> audioclipBuffer;
        
        /// <summary>
        /// audio配置
        /// </summary>
        [ShowInInspector]
        private AudioConfig audioConfig;
        
        /// <summary>
        /// 存储AudioChannel的具体实现类
        /// </summary>
        private Dictionary<AudioChannelType,Type> audioChannelTypeMapping;
        
        
        /// <summary>
        /// 存储AudioChannel的真正实例
        /// </summary>
       [ShowInInspector] private Dictionary<string, AudioChannel> audioChannels;
        
        private float mainVolume;
        public float MainVolume => mainVolume;
        
        private AudioMixer mixer;
        
        public void Init()
        {
            audioConfig = ConfigManager.Instance.GetConfig<AudioConfig>();
            
            audioclipBuffer = new Dictionary<string,AudioClip>();
            
            
            mixer = AssetManager.Instance.Load<AudioMixer>(EAssetLoadMode.Resources,audioConfig.AudioMixerPath);
        }
        public void ForeachCurrentAssembly(Type[] types)
        {        
            audioChannels ??= new Dictionary<string, AudioChannel>();
            audioChannelTypeMapping ??= new Dictionary<AudioChannelType,Type>();
            foreach (var type in types)
            {
                if (typeof(AudioChannel).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                {
                    var Channeltype = (AudioChannelType)Enum.Parse(typeof(AudioChannelType), type.Name);
                    audioChannelTypeMapping[Channeltype] = type;
                }
            }
            
            //添加String与具体的Channel的对应
            foreach (var audioChannelData in audioConfig.audioChannelDatas)
            {
                if (audioChannelTypeMapping.TryGetValue(audioChannelData.AudioChannelType, out var audioChannelType))
                {
                    AudioChannel instance = Activator.CreateInstance(audioChannelType) as AudioChannel;
                    if (instance == null)
                    {
                        continue;
                    }
                    AudioMixerGroup targetMixerGroup = AudioTool.FindCurrentMixerGroup(mixer,audioChannelData.MixerGroupName);
                    instance.Initialize(ContainerObject.transform,targetMixerGroup,audioChannelData,audioChannelData.Name);
                    instance.InitVolume(mixer,1,audioChannelData.ChannelVolumeMin,audioChannelData.ChannelVolumeMax,audioChannelData.MixerVolumeParameterName);
                    audioChannels.Add(audioChannelData.Name,instance);
                }
              
            }
        }
        public void Update()
        {
            foreach (var channel in audioChannels.Values)
            {
                channel.Update();
            }
        }

        public void LateUpdate()
        {
           
        }

        public void FixedUpdate()
        {
            foreach (var channel in audioChannels.Values)
            {
                channel.FixedUpdate();
            }
        }

        public void OnDestroy()
        {
            foreach (var channel in audioChannels.Values)
            {
                channel.OnDestroy();
            }
        }

        public void OnDrawGizmos()
        {

        }

        public void OnDrawGizmosSelected()
        {

        }

        /// <summary>
        /// 设置主音量
        /// </summary>
        /// <param name="volume">声音大小(0-1间的浮点数)</param>
        public void SetMainVolume(float volume)
        {
            mainVolume = Mathf.Clamp01(volume); 
            float volumeValue = AudioTool.RemapVolumeTodB(mainVolume);
            AudioTool.MixerParamterSafeSetFloat(mixer,"Master",volumeValue);
        }
        /// <summary>
        /// 获取主音量大小
        /// </summary>
        /// <returns></returns>
        public float GetMainVolume()
        {
            return mainVolume;  
        }
        /// <summary>
        /// 设置声音通道音量大小
        /// </summary>
        /// <param name="channel">具体的声音通道</param>
        /// <param name="volume">声音大小(0-1间的浮点数)</param>
        public void SetChannelVolume(string channel, float volume)
        {
            if (audioChannels.TryGetValue(channel, out AudioChannel audioChannel))
            {
                audioChannel.SetVolume(mixer,volume);
            }
        }
        /// <summary>
        /// 获取声音通道音量大小
        /// </summary>
        /// <param name="channel">具体的声音通道</param>
        /// <returns></returns>
        public float GetChannelVolume(string channel)
        {
            if (audioChannels.TryGetValue(channel, out AudioChannel audioChannel))
            {
                return audioChannel.GetVolume();
            }
            return 0;
        }

        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="channel">要使用的声音通道</param>
        /// <param name="soundData">音频数据</param>
        public void Play(string channel, SoundData soundData)
        {
            if (audioChannels.TryGetValue(channel,out var _channel))
            {
                _channel.Play(soundData);
            }
        }

        /// <summary>
        /// 关闭该通道所有声音播放
        /// </summary>
        /// <param name="channel">要关闭的声音通道</param>
        public void Stop(string  channel)
        {
            if (audioChannels.TryGetValue(channel,out var _channel))
            {
                _channel.StopAllSounds();
            }
        }

        /// <summary>
        /// 关闭所有声音通道播放
        /// </summary>
        public void StopAll()
        {
            foreach (var channel in audioChannels.Values)
            {
                channel.StopAllSounds();
            }
        }
        
        public void RecycleAudioEmitter(AudioEmitter emitter)
        {
            if (audioChannels.TryGetValue(emitter.ChannelBelongsTo,out var channel))
            {
                channel.Recyle(emitter);
            }
        }
        
    }
}