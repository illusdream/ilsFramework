using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace ilsFramework
{
    public class AudioEmitter : MonoBehaviour, IPoolable
    {
        public SoundData SoundData { get;private set; }
        public AudioSource AudioSource { get;private set; } 
        public string ChannelBelongsTo { get; private set; }

        Coroutine playingCoroutine;
        
        private bool _isPlaying;
        public bool IsPlaying { get => _isPlaying; }
        public void OnGet()
        {
            _isPlaying = true;
        }

        public void OnRecycle()
        {
            _isPlaying = false;
        }

        public void OnPoolDestroy()
        {
          
        }
        
        public void Initialize(SoundData data, AudioMixerGroup audioMixerGroup)
        {
            gameObject.name = data.clip.name;
            SoundData = data;
            AudioSource.clip = data.clip;
            AudioSource.outputAudioMixerGroup = audioMixerGroup;
            AudioSource.loop = data.loop;
            AudioSource.playOnAwake = data.playOnAwake;

            AudioSource.mute = data.mute;
            AudioSource.bypassEffects = data.bypassEffects;
            AudioSource.bypassListenerEffects = data.bypassListenerEffects;
            AudioSource.bypassReverbZones = data.bypassReverbZones;

            AudioSource.priority = data.priority;
            AudioSource.volume = data.volume;
            AudioSource.pitch = data.pitch;
            AudioSource.panStereo = data.panStereo;
            AudioSource.spatialBlend = data.spatialBlend;
            AudioSource.reverbZoneMix = data.reverbZoneMix;
            AudioSource.dopplerLevel = data.dopplerLevel;
            AudioSource.spread = data.spread;

            AudioSource.minDistance = data.minDistance;
            AudioSource.maxDistance = data.maxDistance;

            AudioSource.ignoreListenerVolume = data.ignoreListenerVolume;
            AudioSource.ignoreListenerPause = data.ignoreListenerPause;

            AudioSource.rolloffMode = data.rolloffMode;
        }
        
        public void Play()
        {
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
            }

            AudioSource.Play();
            playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        }

        IEnumerator WaitForSoundToEnd()
        {
            yield return new WaitWhile(() => AudioSource.isPlaying);
            Stop();
        }

        public void Stop()
        {
            if (!_isPlaying)
            {
                return;
            }
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
                playingCoroutine = null;
            }
            
            AudioSource.Stop();
            AudioManager.Instance.RecycleAudioEmitter(this);
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
        {
            AudioSource.pitch += Random.Range(min, max);
        }

        public void SetAudioSource(AudioSource audioSource)
        {
            this.AudioSource = audioSource;
        }

        public void SetChannelBelongsTo(string channelName)
        {
            this.ChannelBelongsTo = channelName;
        }
    }
}