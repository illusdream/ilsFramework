using System;
using System.Collections.Generic;
using System.Linq;

namespace ilsFramework.Core
{
        public static class RandomExtension
        {
                public static bool RandomBool(this float probability,RandomSeed randomSeed = null)
                {
                        randomSeed?.ApplySeed();
                        return UnityEngine.Random.value < probability;
                }
                public static int RandomRange(this (int, int) range,RandomSeed randomSeed = null)
                {
                        randomSeed?.ApplySeed();
                        return UnityEngine.Random.Range(range.Item1, range.Item2);
                }

                public static float RandomRange(this (float, float) range,RandomSeed randomSeed = null)
                {
                        randomSeed?.ApplySeed();
                        return UnityEngine.Random.Range(range.Item1, range.Item2);
                }
        
                public static float RandomRange(this (int, float) range,RandomSeed randomSeed = null)
                {
                        randomSeed?.ApplySeed();
                        return UnityEngine.Random.Range(range.Item1, range.Item2);
                }
        
                public static float RandomRange(this (float, int) range,RandomSeed randomSeed = null)
                {
                        randomSeed?.ApplySeed();
                        return UnityEngine.Random.Range(range.Item1, range.Item2);
                }

                public static T[] Shuffle<T>(this T[] array,RandomSeed randomSeed = null)
                {
                        randomSeed?.ApplySeed();
                        int n = array.Length;
                        var newArray = (T[])array.Clone();
                        for (int i = n-1; i >=0; i--)
                        {
                                int k  = UnityEngine.Random.Range(0, n - i);
                                (newArray[i], newArray[k]) = (newArray[k], newArray[i]);
                        }
                        return newArray;
                }

                public static void SelfShuffle<T>(this T[] array,RandomSeed randomSeed = null)
                {
                        randomSeed?.ApplySeed();
                        int n = array.Length;
                        for (int i = n-1; i >=0; i--)
                        {
                                int k  = UnityEngine.Random.Range(0, n - i);
                                (array[i], array[k]) = (array[k], array[i]);
                        }
                }
        

                public static List<T> Shuffle<T>(this List<T> list,RandomSeed randomSeed = null)
                {
                        randomSeed?.ApplySeed();
                        int n = list.Count;
                        var newlist = (List<T>)list.ConvertAll((res)=>res);
                        for (int i = n-1; i >=0; i--)
                        {
                                int k  = UnityEngine.Random.Range(0, n - i);
                                (newlist[i], newlist[k]) = (newlist[k], newlist[i]);
                        }
                        return newlist;
                }

                public static void SelfShuffle<T>(this List<T> list,RandomSeed randomSeed = null)
                {
                        randomSeed?.ApplySeed();
                        int n = list.Count;
                        for (int i = n-1; i >=0; i--)
                        {
                                int k = UnityEngine.Random.Range(0, n - i);
                                (list[i], list[k]) = (list[k], list[i]);
                        }
                }
                public static IEnumerable<T> ReservoirSampling<T>(this ICollection<T> array,int samplingCount,RandomSeed randomSeed = null)
                {
                        randomSeed?.ApplySeed();
                        //超出数量直接报错得了
                        if (samplingCount > array.Count)
                        {
                                throw new ArgumentOutOfRangeException(nameof(samplingCount), samplingCount, $"尝试从{array.Count}中抽取{samplingCount}个结果，你要jb干嘛");
                        }
                        var res = array.ToArray().Shuffle().Take(samplingCount);
                        return res;
                }

                public static IEnumerable<T> NoOverflowReservoirSampling<T>(this ICollection<T> array,int samplingCount,RandomSeed randomSeed = null)
                {
                        randomSeed?.ApplySeed();
                        //全弄过来
                        if (samplingCount > array.Count)
                        {
                                samplingCount = array.Count;
                        }
                        var res = array.ToArray().Shuffle().Take(samplingCount);
                        return res;
                }
        }
}