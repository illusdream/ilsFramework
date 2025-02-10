using UnityEngine;
using UnityEngine.Audio;

namespace ilsFramework
{
    public class SwitchAudioChannel : AudioChannel
    {
        public override AudioEmitter Play(SoundData soundData)
        {
            return null;
        }

        public override void Recyle(AudioEmitter emitter)
        {
           
        }

        public override void StopAllSounds()
        {
            
        }

        public override void Initialize(Transform parentTransform, AudioMixerGroup audioMixerGroup, AudioChannelData audioChannelData, string audioChannelName)
        {
           
        }

        public override void Update()
        {
           
        }

        public override void FixedUpdate()
        {
          
        }

        public override void OnDestroy()
        {
            
        }
    }
}