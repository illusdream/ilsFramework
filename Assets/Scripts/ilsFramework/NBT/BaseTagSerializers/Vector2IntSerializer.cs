using System;
using UnityEngine;

namespace ilsFramework.BaseTagSerializers
{
    public class Vector2IntSerializer : TagSerializer<Vector2Int>
    {
        public override NBTCompound Serialize(Vector2Int value)
        {
            var instance = new NBTCompound("Vector2Int");
            instance.Set("x", value.x);
            instance.Set("y", value.y);
            return instance;
        }

        public override Vector2Int Deserialize(NBTCompound value)
        {
            if (value.TryGet("x",out int x) && value.TryGet("y", out int y))
            {
                return new Vector2Int(x, y);
            }
            return new Vector2Int(0, 0);
        }
    }
}