using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ilsFramework.Core
{
    [Serializable]
    public class RandomSeed
    {
        public RandomSeed(int seed)
        {
            this.seed = seed;
        }
        [SerializeField]
        private int seed;
        public int Seed => seed;

        public RandomSeed SetSeed(int seed)
        {
            this.seed = seed;
            return this;
        }

        public void ApplySeed()
        {
            Random.InitState(Seed);
        }

        public static RandomSeed CreateSeed()
        {
            int seed =(int)math.lerp(int.MinValue, int.MaxValue, Random.value);
            return new RandomSeed(seed);
        }

        public static RandomSeed CreateSeed(int seed)
        {
            return new RandomSeed(seed);
        }

        public static RandomSeed CreateSeedByNow()
        {
            return new RandomSeed(DateTime.Now.Millisecond);
        }
    }
}