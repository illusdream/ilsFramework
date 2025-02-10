using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ilsFramework
{
    public class AudioChannelAsset : ScriptableObject
    {
        public string AudioMixerPath;
        [SerializeField]
        public List<AudioChannelData> audioChannelDatas = new List<AudioChannelData>();
    }
}