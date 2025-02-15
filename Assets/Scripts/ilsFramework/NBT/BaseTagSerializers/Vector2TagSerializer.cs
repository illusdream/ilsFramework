using UnityEngine;

namespace ilsFramework.BaseTagSerializers
{
    public class Vector2TagSerializer: TagSerializer<Vector2>
    {
        public override NBTCompound Serialize(Vector2 value)
        {
            var tag = new NBTCompound("Vector2");
            tag.Set("x", value.x);
            tag.Set("y", value.y);
            return tag;
        }

        public override Vector2 Deserialize(NBTCompound value)
        {
            if (value.TryGet("x",out float x) && value.TryGet("y", out float y))
            {
                return new Vector2(x, y);
            }
            return Vector2.zero;
        }
    }
}