using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ilsFramework
{
    /// <summary>
    /// 场景有关事件
    /// </summary>
    public partial class SceneEvent
    {
        public const string SceneLoaded = "SceneLoaded";
        public const string SceneUnloaded = "SceneUnloaded";
        public const string SceneChanged = "SceneChanged";

        public class SceneLoadedEventArgs : EventArgs
        {
            /// <summary>
            /// 加载的场景实例
            /// </summary>
            public Scene Scene;
            /// <summary>
            /// 加载模式
            /// </summary>
            public LoadSceneMode LoadSceneMode;

            public SceneLoadedEventArgs(Scene scene, LoadSceneMode loadSceneMode)
            {
                Scene = scene;
                LoadSceneMode = loadSceneMode;
            }
        }

        public class SceneUnloadedEventArgs : EventArgs
        {
            /// <summary>
            /// 卸载的场景实例
            /// </summary>
            public Scene Scene;

            public SceneUnloadedEventArgs(Scene scene)
            {
                Scene = scene;
            }
        }

        public class SceneChangedEventArgs : EventArgs
        {
            /// <summary>
            /// 假设场景切换：A->B
            /// 此字段指 场景A
            /// 由于该实例已卸载，无法获取，使用<see cref="SceneEvent.SceneUnloaded"/>事件
            /// </summary>
            public Scene BeforeScene;
            /// <summary>
            /// 假设场景切换：A->B
            /// 此字段指 场景B
            /// 此实例可用
            /// </summary>
            public Scene AfterScene;

            public SceneChangedEventArgs(Scene beforeScene, Scene afterScene)
            {
                BeforeScene = beforeScene;
                AfterScene = afterScene;
            }
        }
    }
}