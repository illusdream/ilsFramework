using Unity.Mathematics;
using UnityEngine;

namespace ilsFramework
{
    public static class MathUtils
    {
        public static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            float v = (value - from1) / (to1 - from1);
            return math.lerp( from2, to2,v);
        }

        public static Vector3 Vec3_xy(this Vector2 value,bool reverse = false)
        {
            return reverse? new Vector3(value.y,value.x,0):new Vector3(value.x,value.y,0);
        }

        public static Vector3 Vec3_xz(this Vector2 value, bool reverse = false)
        {
            return reverse? new Vector3(value.y,0,value.x):new Vector3(value.x,0,value.y);
        }

        public static Vector3 Vec3_yz(this Vector2 value, bool reverse = false)
        {   
            return reverse? new Vector3(0,value.y,value.x):new Vector3(0,value.x,value.y);
        }
    }
}