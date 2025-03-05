namespace ilsFramework
{
    public static class AudioUtils
    {
        /// <summary>
        /// 设置主音量
        /// </summary>
        /// <param name="volume">声音大小(0-1间的浮点数)</param>
        public static void SetMainVolume(float volume)
        {
            AudioManager.Instance.SetMainVolume(volume);
        }
        
        /// <summary>
        /// 获取主音量大小
        /// </summary>
        /// <returns></returns>
        public static float GetMainVolume()
        {
            return AudioManager.Instance.GetMainVolume();  
        }
        /// <summary>
        /// 设置声音通道音量大小
        /// </summary>
        /// <param name="channel">具体的声音频道</param>
        /// <param name="volume">声音大小(0-1间的浮点数)</param>
        public static void SetChannelVolume(string channel, float volume)
        {
            AudioManager.Instance.SetChannelVolume(channel, volume);
        }

        /// <summary>
        /// 获取声音通道音量大小
        /// </summary>
        /// <param name="channel">具体的声音频道</param>
        /// <returns></returns>
        public static float GetChannelVolume(string channel)
        {
            return AudioManager.Instance.GetChannelVolume(channel);
        }
        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="channel">要使用的声音通道</param>
        /// <param name="soundData">音频数据</param>
        public static void Play(string channel, SoundData soundData)
        {
            AudioManager.Instance.Play(channel, soundData);
        }
        /// <summary>
        /// 关闭该通道所有声音播放
        /// </summary>
        /// <param name="channel">要关闭的声音通道</param>
        public static void Stop(string channel)
        {
            AudioManager.Instance.Stop(channel);
        }

        /// <summary>
        /// 关闭所有声音通道播放
        /// </summary>
        public static void StopAll()
        {
            AudioManager.Instance.StopAll();
        }
    }
}