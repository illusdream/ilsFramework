using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace ilsFramework
{
    public class CommenAudioChannel : AudioChannel
    {
        public const int InitAudioEmitterCount = 8;


        private GameObject AudioChannelContainer;
        private AudioMixerGroup AudioMixerGroup;
        GameObjectPool audioPool;
        private Dictionary<GameObject,AudioEmitter> audioEmitters ;
        
        public string Name;
        
        public override  AudioEmitter Play(SoundData soundData)
        {
            GameObject AudioEmitterInstance = audioPool.Get();
            if (!AudioEmitterInstance)
            {
                return null;    
            }
            if (audioEmitters.TryGetValue(AudioEmitterInstance, out AudioEmitter audioEmitter))
            {
                audioEmitter.Initialize(soundData, AudioMixerGroup);
                audioEmitter.Play();
                return audioEmitter;
            }
            return null;
        }

        public override void Recyle(AudioEmitter emitter)
        {
           audioPool.Recycle(emitter.gameObject);
        }

        public override void StopAllSounds()
        {
            foreach (var emitter in audioEmitters.Values)
            {
                if (emitter.enabled)
                {
                    emitter.Stop();
                }
            }
        }

        public override void Initialize(Transform parentTransform, AudioMixerGroup audioMixerGroup, AudioChannelData audioChannelData,string audioChannelName)
        {
            audioEmitters = new Dictionary<GameObject,AudioEmitter>();
            
            AudioChannelContainer = new GameObject(audioChannelData.Name);
        
            audioPool=  GameObjectPoolFactory
                .Create()
                .SetName(audioChannelData.Name + "AudioPool")
                .SetInitialCapacity(InitAudioEmitterCount)
                .SetMaxCapacity(audioChannelData.MaxAudioClipCount)
                .SetCollectionCheck(true)
                .SetGameObjectParent(AudioChannelContainer.transform)
                .SetCreateObjectFunc(CreateAudioEmitter)
                .SetActionOnGet(PoolGetAudioEmitter)
                .SetActionOnRecycle(PoolRecyleAudioEmitter)
                .SetActionOnDestroy(PoolDestroyAudioEmitter)
                .Register();
            
            
            
            AudioMixerGroup = audioMixerGroup;
            Name = audioChannelData.Name;
            AudioChannelContainer.transform.parent = parentTransform;
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

        private GameObject CreateAudioEmitter(GameObjectPool pool)
        {
            var go = new GameObject("AudioEmitter");
            var audioEmitter = go.AddComponent<AudioEmitter>();
            var audioSource = go.AddComponent<AudioSource>();
            
            audioEmitter.SetAudioSource(audioSource) ;
            audioEmitter.SetChannelBelongsTo(Name);
            
            audioEmitters.Add(go,audioEmitter);
            go.transform.SetParent(pool.PoolViewer.transform);
            go.SetActive(false);
            return go;
        }

        private void PoolGetAudioEmitter(GameObject go)
        {
            go.SetActive(true);
            
        }

        private void PoolRecyleAudioEmitter(GameObject go)
        {
            go.SetActive(false);
        }

        private void PoolDestroyAudioEmitter(GameObject go)
        {
            audioEmitters.Remove(go);   
            GameObject.Destroy(go);
        }
    }
}