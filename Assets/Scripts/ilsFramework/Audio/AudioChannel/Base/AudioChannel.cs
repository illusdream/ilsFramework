using UnityEngine;
using UnityEngine.Audio;

namespace ilsFramework
{
    public abstract class AudioChannel
    {
        public float Volume;
        private float MinVolume;
        private float MaxVolume;
        private string ChannelVolumeParamterNames;
        public abstract AudioEmitter Play(SoundData soundData);

        public abstract void Recyle(AudioEmitter emitter);
        
        public abstract void StopAllSounds();
        
        public abstract void  Initialize(Transform parentTransform,AudioMixerGroup audioMixerGroup,AudioChannelData audioChannelData,string audioChannelName);
        
        public abstract  void Update();
        
        public abstract  void FixedUpdate();
        
        public abstract void OnDestroy();

        public virtual void SetVolume(AudioMixer mixer,float volume)
        {
            float volumeValue = Mathf.Clamp01(volume);
            var _volumeValue = MathUtils.Remap(volumeValue, 0, 1, MinVolume, MaxVolume);
            AudioTool.MixerParamterSafeSetFloat(mixer,ChannelVolumeParamterNames,_volumeValue);
        }

        public virtual float GetVolume()
        {
            return Volume; 
        }

        public virtual void InitVolume(AudioMixer mixer,float volume,float minVolume,float maxVolume,string channelVolumeParamterNames)
        {
            Volume = volume;
            MinVolume = minVolume;
            MaxVolume = maxVolume;
            ChannelVolumeParamterNames = channelVolumeParamterNames;
            SetVolume(mixer,volume);
        }
    }
}