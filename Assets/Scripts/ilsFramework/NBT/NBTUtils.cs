using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Cysharp.Threading.Tasks;
using SQLite4Unity3d;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ilsFramework
{
    public partial class NBT
    {
        public const string NBTDataBasePath = "NBT";
        
        public static HashSet<string> BaseNBTTypes = new HashSet<string>()
        {
            typeof(NBTCompound).FullName,
        };
        public static byte[] Serialize(NBTCompound root)
        {
            using (var ms = new MemoryStream())
            {
                root.Write(ms);
                return ms.ToArray();
            }
        }
        

        public static NBTCompound Deserialize(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                var root = new NBTCompound();
                root.Read(ms);
                return root;
            }
        }
        public static void WriteString(Stream stream, string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var length = BitConverter.GetBytes((ushort)bytes.Length);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(length);
            }
            stream.Write(length, 0, sizeof(ushort));
            stream.Write(bytes, 0, bytes.Length);
        }

        public static string ReadString(Stream stream)
        {
            var lengths = new byte[sizeof(ushort)];
            stream.Read(lengths, 0, sizeof(ushort));
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lengths);
            }
            int length = BitConverter.ToUInt16(lengths, 0);
            
            var bytes = new byte[length];
            stream.Read(bytes, 0,length);
            return Encoding.UTF8.GetString(bytes);
        }
        

        public static SerializerCollection GetSerializerCollection()
        {
            SerializerCollection instance = null;
            if (!Application.isPlaying)
            {
#if UNITY_EDITOR
                instance = new SerializerCollection();
                var  allNBTType = TypeCache.GetTypesDerivedFrom(typeof(TagSerializer<>));
                instance.FillSerializers(allNBTType.ToList());
#endif
            }
            else
            {
                instance = NBTManager.Instance.GetSerializers();
            }
            return instance;
        }

        public static NBTCompound OpenNBTFile(string path)
        {
            try
            {
                var sr = File.ReadAllBytes(path + ".Lnbt");
                var nbt = NBT.Deserialize(sr);
                return nbt;
            }
            catch (Exception e)
            {
                throw;
            }
            
        }

        public static async UniTask<NBTCompound> OpenNBTFileAsync(string path)
        {
            try
            {
                var sr =await File.ReadAllBytesAsync(path + ".Lnbt");
                return NBT.Deserialize(sr);
            }
            catch (Exception e)
            {
                throw;
            }
        }
        

        public static void SaveNBTFile(NBTCompound root, string path)
        {
            using (FileStream fs = File.Create(path + ".Lnbt"))
            {
                var nbt = Serialize(root);
                fs.Write(nbt, 0, nbt.Length);
            }
        }

        public static async UniTask SaveNBTFileAsync(NBTCompound root, string path)
        {
            await using FileStream fs = File.Create(path + ".Lnbt");
            var nbt = Serialize(root);
            await fs.WriteAsync(nbt, 0, nbt.Length);
        }
    }
}