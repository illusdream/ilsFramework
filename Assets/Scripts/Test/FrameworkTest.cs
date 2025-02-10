using System;
using System.Collections.Generic;
using System.Linq;
using ilsFramework;
using ilsFrameWork;
using Sirenix.OdinInspector;
using SQLite4Unity3d;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Logger = ilsFramework.Logger;
using Object = UnityEngine.Object;

namespace Test
{
    public class TESTTable
    {
        [PrimaryKey]
        public int id { get; set; }
        public int number { get; set; }
    }
    public class FrameworkTest : MonoBehaviour
    {
        
        public Color testColor = Color.red;
        public AudioChannelData AudioChannelData;
        Stack<GameObject> gameObjects = new Stack<GameObject>();
        
        public void Awake()
        {

        }


        public void Start()
        {
            222.LogSelf();
          // //测试同步加载  
           
          // //测试异步加载
          // AssetManager.Instance.LoadAsync(Prefabs.Test1, (o) =>
          // {
          //     Instantiate(o, Vector3.right, Quaternion.identity);
          // });
        }

        public void Update()
        {
            
        }

        public void FixedUpdate()
        {
            
        }

        public void OnDestroy()
        {
            
        }
        
        [Button]
        public  void PlaySound()
        {
            SoundData data = new SoundData()
            {
                clip = (AudioClip)AssetManager.Instance.Load(Audio.ScaryVFX)
            };
            AudioManager.Instance.Play(AudioChannelName.Sound, data);
        }
        [Button]
        public  void StopAllSounds()
        {
            AudioManager.Instance.StopAll();
        }
        [Button]
        public void StopSoundChannel()
        {
           // AudioManager.Instance.Stop(AudioChannelName.Sound);
        }
        [Button]
        public void ChangeVolume(float volume)
        {
           // AudioManager.Instance.SetChannelVolume(AudioChannelName.Sound, volume);
            AudioManager.Instance.SetMainVolume(volume);
        }
        [Button]
        public void ChangeVolume2(float volume)
        {
            // AudioManager.Instance.SetChannelVolume(AudioChannelName.Sound, volume);
            AudioManager.Instance.SetChannelVolume(AudioChannelName.Sound,volume);
        }
        [Button]
        public void ToFristScene()
        {
            SceneManager.LoadScene("Test1");
        }
        [Button]
        public void ToSecondScene()
        {
            SceneManager.LoadScene("Test2");
        }
        [Button]
        public void TestLog()
        {
           "测试".LogSelf(this.gameObject,colorConvert:LogColor.Magenta,showStackTrace:true);
        }
       
#if UNITY_EDITOR
        [Button]
        public void CreateScript()
        {
            ScriptGenerator generator = new ScriptGenerator();
            ClassGenerator classGenerator = new ClassGenerator(EAccessType.Public, "TestG");
            classGenerator.Append(new StringFieldGenerator(EFieldDeclarationMode.Null,EAccessType.Public,"test","测试文本","测试实施事实和"));
            generator.Append(classGenerator);
            EnumGenerator enumGenerator = new EnumGenerator(EAccessType.Public, "TestEnum","测试Enum",("test1","diyige"),("test2","dierge"));
            classGenerator.Append(enumGenerator);
            generator.GenerateScript("TestFile");
            AssetDatabase.Refresh();
        }
#endif


    }
}