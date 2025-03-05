using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;

namespace ilsFramework
{
    public static class AudioTool
    {
        public static AudioMixerGroup FindCurrentMixerGroup(AudioMixer mixer,string audioMixerGroupName)
        {
            var result = mixer.FindMatchingGroups(audioMixerGroupName);
            if (result.Length ==0)
            {
                result = mixer.FindMatchingGroups("Master");
            }
            return result[0];
        }

        public static float RemapVolumeTodB(float volume,float min = -80,float max = 0)
        {
            return math.lerp(min, max, volume);
        }

        public static void MixerParamterSafeSetFloat(AudioMixer mixer, string paramName, float value)
        {
            if (!mixer)
            {
                Debug.LogError("mixer为空");
                return;
            }

            if (string.IsNullOrEmpty(paramName))
            {
                return;
            }
            if (!mixer.GetFloat(paramName, out float currentValue))
            {
                Debug.LogError($"参数 {paramName} 不存在!");
                return;
            }

            // 设置参数值
            mixer.SetFloat(paramName, value);
        }
    }
}