using Unity.Mathematics;

namespace ilsFramework
{
    public static class MathUtils
    {
        public static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            float v = (value - from1) / (to1 - from1);
            return math.lerp( from2, to2,v);
        }
    }
}