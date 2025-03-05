using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ilsFramework
{
    public class NBTCompound : NBT
    {
        public NBTCompound()
        {
            
        }
        public NBTCompound(string Name)
        {
            this.Name = Name;
        }
        public Dictionary<string,object> children { get; } = new Dictionary<string, object>();
        
        public  void Write(Stream stream)
        {
            WriteString(stream, Name);
            foreach (var nbt in children)
            {
                WriteBaseData(nbt.Key,nbt.Value, stream);
            }
            stream.WriteByte((byte)ENBTType.End);
        }

        public  void Read(Stream stream)
        {
            Name = ReadString(stream);
            children.Clear();
            while (true)
            {
                var value = ReadBaseData(stream,out string key,out bool isEnd);
                if (isEnd)
                {
                    break;
                }
                children[key] = value;
            }
        }

        private void WriteBaseData(string key,object data, Stream stream)
        {
            switch (data)
            {
                case bool value:
                {
                    stream.WriteByte((byte)ENBTType.Bool);
                    WriteString(stream, key);
                    var bytes = BitConverter.GetBytes(value);
                    if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
                    stream.Write(bytes, 0, sizeof(bool));
                }
                    break;
                case byte value:
                {
                    stream.WriteByte((byte)ENBTType.Byte);
                    WriteString(stream, key);
                    var bytes = BitConverter.GetBytes(value);
                    if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
                    stream.Write(bytes, 0, sizeof(byte));
                }
                    break;
                case short value:
                {
                    stream.WriteByte((byte)ENBTType.Short);
                    WriteString(stream, key);
                    var bytes = BitConverter.GetBytes(value);
                    if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
                    stream.Write(bytes, 0, sizeof(short));
                }
                    break;
                case int value:
                {
                    stream.WriteByte((byte)ENBTType.Int);
                    WriteString(stream, key);
                    var bytes = BitConverter.GetBytes(value);
                    if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
                    stream.Write(bytes, 0, sizeof(int));
                }
                    break;
                case long value:
                {
                    stream.WriteByte((byte)ENBTType.Long);
                    WriteString(stream, key);
                    var bytes = BitConverter.GetBytes(value);
                    if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
                    stream.Write(bytes, 0, sizeof(long));
                }
                    break;
                case float value:
                {
                    stream.WriteByte((byte)ENBTType.Float);
                    WriteString(stream, key);
                    var bytes = BitConverter.GetBytes(value);
                    if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
                    stream.Write(bytes, 0, sizeof(float));
                }
                    break;
                case double value:
                {
                    stream.WriteByte((byte)ENBTType.Double);
                    WriteString(stream, key);
                    var bytes = BitConverter.GetBytes(value);
                    if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
                    stream.Write(bytes, 0, sizeof(double));
                }
                    break;
                case string value:
                    stream.WriteByte((byte)ENBTType.String);
                    WriteString(stream, key);
                    WriteString(stream, value);
                    break;
                case NBTCompound compound:
                    stream.WriteByte((byte)ENBTType.Compound);
                    WriteString(stream, key);
                    compound.Write(stream);
                    break;
                default :
                    stream.WriteByte((byte)ENBTType.Empty);
                    break;
            }
        }
        
        private object ReadBaseData(Stream stream,out string key,out bool isEnd)
        {
            isEnd = false;
            key = null;
            var type = (ENBTType)stream.ReadByte();
            if (type != ENBTType.End)
            {
                key = ReadString(stream);
            }
            switch (type)
            {
                case ENBTType.End:
                    isEnd = true;
                    return null;
                case ENBTType.Bool:
                {
                    int length = sizeof(bool);
                    var bytes = new byte[length];
                    stream.Read(bytes, 0, length);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(bytes);
                    }
                    return BitConverter.ToBoolean(bytes, 0);
                }
                case ENBTType.Byte:
                {
                    int length = sizeof(byte);
                    var bytes = new byte[length];
                    stream.Read(bytes, 0, length);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(bytes);
                    }
                    return bytes[0];
                }
                case ENBTType.Short:
                {
                    int length = sizeof(short);
                    var bytes = new byte[length];
                    stream.Read(bytes, 0, length);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(bytes);
                    }
                    return BitConverter.ToInt16(bytes, 0);
                }
                case ENBTType.Int:
                {
                    int length = sizeof(int);
                    var bytes = new byte[length];
                    stream.Read(bytes, 0, length);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(bytes);
                    }
                    return BitConverter.ToInt32(bytes, 0);
                }
                case ENBTType.Long:
                {
                    int length = sizeof(long);
                    var bytes = new byte[length];
                    stream.Read(bytes, 0, length);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(bytes);
                    }
                    return BitConverter.ToInt64(bytes, 0);
                }
                case ENBTType.Float:
                {
                    int length = sizeof(float);
                    var bytes = new byte[length];
                    stream.Read(bytes, 0, length);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(bytes);
                    }
                    return BitConverter.ToSingle(bytes, 0);
                }
                case ENBTType.Double:
                {
                    int length = sizeof(double);
                    var bytes = new byte[length];
                    stream.Read(bytes, 0, length);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(bytes);
                    }
                    return BitConverter.ToDouble(bytes, 0);
                }
                case ENBTType.String:
                    return ReadString(stream);
                case ENBTType.Compound:
                    var compound = new NBTCompound();
                    compound.Read(stream);
                    return compound;
                case ENBTType.Empty:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Set<T>(string key, T value)
        {
            if (CheckInputValue(value, key, out var cValue))
            {
                children[key] = cValue;
            }
        }

        public void SetList<T>(string key, List<T> values)
        {
            var compound = new NBTCompound
            {
                Name = "List"
            };
            compound.Set("Type", typeof(T).FullName);
            compound.Set("IsList", true);
            compound.Set("Count", values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                compound.Set(i.ToString(), values[i]);
            }
            children[key] = compound;
        }

        public bool TryGet<T>(string key, out T value)
        {
            value = default;
            
            if (children.TryGetValue(key,out var o))
            {
                if (o is T value1|| (o is NBTCompound && CheckOutValue<T>(o, out  value1)))
                {
                    value = value1;
                    return true;
                }

            }
            return false;
        }

        public bool TryGetList<T>(string key, out List<T> values)
        {
            values = null;
            if (TryGet(key ,out NBTCompound listCompound) 
                && listCompound.TryGet("Type",out string type) && type == typeof(T).FullName 
                && listCompound.TryGet("IsList", out bool isList) && isList
                && listCompound.TryGet("Count", out int count))
            {
                values = new List<T>();
                for (int i = 0; i < count; i++)
                {
                    if (listCompound.TryGet(i.ToString(), out T value))
                    {
                        values.Add(value);
                    }
                }
            }
            return values != null;
        }
        private bool CheckInputValue<T>(T value,string key,out object cValue)
        {
            cValue = null;
            switch (value)
            {
                case bool or byte or short or int or long or float or double or string or NBTCompound:
                    cValue = value;
                    return true;
                default:
                    var collection = GetSerializerCollection();
                    if (collection.TryGetTagSerializer<T>(out var serializer,out var tagId))
                    {
                        var result = serializer.Serialize(value);
                        cValue = result;
                        return true;
                    }
                    return false;
            }
        }

        private bool CheckOutValue<T>(object value, out T cValue)
        {
            var collection = GetSerializerCollection();
            if (value is NBTCompound compound &&collection.TryGetTagSerializer(typeof(T).FullName,out var serializer))
            {
                if (serializer.DeSerialize(compound) is T target)
                {
                    cValue = target;
                    return true;
                }
            }
            cValue = default;
            return false;
        }
        
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{Name}:");
            foreach (var child in children)
            {
                if (child.Value is NBTCompound compound)
                {
                    sb.Append($"[key: {child.Key}][Type:{nameof(NBTCompound)}] value: {compound.ToString(1)}" );
                }
                else
                {
                    sb.AppendLine($"[key: {child.Key}][Type:{nameof(child.Value)}] value: {child.Value}");
                }
            }
            return sb.ToString();
        }

        public string ToString(int indent)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{Name}:");
            foreach (var child in children)
            {
                if (child.Value is NBTCompound compound)
                {
                    sb.Append(compound.ToString(indent + 1));
                }
                else
                {
                    string indentString = new string('\t', indent);
                    sb.AppendLine($"{indentString}[key: {child.Key}] value: {child.Value}");
                }
            }
            return sb.ToString();
        }
    }
}