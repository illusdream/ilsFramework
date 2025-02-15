using System;
using UnityEngine;

namespace ilsFramework.BaseTagSerializers
{
    public class Vector3TagSerializer : TagSerializer<Vector3>
    {
        public override NBTCompound Serialize(Vector3 value)
        {
            var nbt = new NBTCompound("Vector3");
            nbt.Set("x", value.x);
            nbt.Set("y", value.y);
            nbt.Set("z", value.z);
            return nbt;
        }

        public override Vector3 Deserialize(NBTCompound value)
        {
            if (value.TryGet("x",out float x) && value.TryGet("y", out float y) && value.TryGet("z", out float z))
            {
                return new Vector3(x, y, z);
            }
            return Vector3.zero;
        }
    }
}