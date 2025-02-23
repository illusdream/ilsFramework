using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
            UIManager.Instance.GetUIPanel<TestUIPanel>().Open();
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
        [Button]
        public void NBTTestWrite()
        {
            
                NBTCompound compound = new NBTCompound();
                compound.Name = "TestNBT";
                compound.Set("Bool",false);
                compound.Set("Byte",(byte)1);
                compound.Set("Short",(short)2);
                compound.Set("Int",(int)3);
                compound.Set("Long",2131);
                compound.Set("Float",1.4f);
                compound.Set("Double",1.5d);
                compound.Set("String","测试String");
                compound.Set("Vector",new Vector3(1,2));
                NBTCompound testCompound = new NBTCompound();
                testCompound.Name = "testCompound";
                testCompound.Set("1",1);
                compound.Set("Compound",testCompound);

                var list = new List<Vector2>
                {
                    new Vector2(1,1),
                    new Vector2(2,2),
                    new Vector2(3,3),
                    new Vector2(4,4),
                    new Vector2(5,5),
                };
                compound.SetList("List",list);
            NBT.SaveNBTFile(compound,"F:\\Unity\\ilsFramework\\1111\\NBTTest");

        }
        [Button]
        public void NBTTestRead()
        {
            Stopwatch sw = new Stopwatch();
            // 打开流并读取。
            
            sw.Start();
            var nbt = NBT.OpenNBTFile("F:\\Unity\\ilsFramework\\1111\\NBTTest");
            
            nbt.LogSelf();

            if (nbt.TryGet("Vector", out Vector3 v))
            {
                v.LogSelf();
            }
            
            if (nbt.TryGetList("List", out List<Vector2> list))
            {
                foreach (var vector2 in list)
                {
                   //vector2.LogSelf();
                }
            }
            
            
            sw.Stop();
            sw.ElapsedMilliseconds.LogSelf();
            sw.Elapsed.LogSelf();
            
        }
        [Button]
        public void TestGenerateClass()
        {
            Stopwatch sw = new Stopwatch();
            Type type = typeof(Test);
            ConstructorInfo constructors;
            constructors = type.GetConstructor(Type.EmptyTypes);
            sw.Start();
            for (int i = 0; i < 10000; i++)
            {
                _ = new Test();
            }
            sw.Stop();
            sw.Elapsed.TotalMilliseconds.LogSelf();
            
            sw.Reset();
            
            sw.Start();
            for (int i = 0; i < 10000; i++)
            {
                _ = Activator.CreateInstance(type);
            }
            sw.Stop();
            sw.Elapsed.TotalMilliseconds.LogSelf();
            
            sw.Reset();
            sw.Start();
            for (int i = 0; i < 10000; i++)
            {
                _ = constructors.Invoke(Array.Empty<object>());
            }
            sw.Stop();
            sw.Elapsed.TotalMilliseconds.LogSelf();
        }

        [Button]
        public void CloseUITest()
        {
            UIManager.Instance.GetUIPanel<TestUIPanel>().Close();
        }
        private class Test
        {
            
        }
    }
}