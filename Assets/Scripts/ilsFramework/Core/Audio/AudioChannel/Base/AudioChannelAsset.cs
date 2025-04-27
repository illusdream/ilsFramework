using System.Collections.Generic;
using UnityEngine;

namespace ilsFramework.Core
{
    public class AudioChannelAsset : ScriptableObject
    {
        public string AudioMixerPath;

        [SerializeField] public List<AudioChannelData> audioChannelDatas = new();
    }
}