using System;
using System.IO;

namespace ilsFramework
{
    public interface ITagSerializer
    {
        public Type TargetType { get; }
        
        public  NBTCompound Serialize(object value);
        
        public  object DeSerialize(NBTCompound value);
    }
    
    public abstract class TagSerializer<T> : ITagSerializer
    {
        public Type TargetType =>typeof(T);
        public abstract NBTCompound Serialize(T value);
        

        public NBTCompound Serialize(object value)
        {
            return Serialize((T)value);
        }
        
        public abstract T Deserialize(NBTCompound value);
        
        public object DeSerialize(NBTCompound value)
        {
            return Deserialize(value);
        }
    }
}