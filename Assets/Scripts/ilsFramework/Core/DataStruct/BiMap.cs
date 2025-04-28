using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace ilsFramework.Core
{
        /// <summary>
        /// 双向哈希表
        /// </summary>
        public class BiMap<TLeft, TRight> : IEnumerable<KeyValuePair<TLeft, TRight>>
        {
                [ShowInInspector]
                private Dictionary<TLeft, TRight> leftMap;
                private Dictionary<TRight, TLeft> rightMap;

                public BiMap()
                {
                        leftMap  = new Dictionary<TLeft, TRight>();
                        rightMap = new Dictionary<TRight, TLeft>();
                }

                public void Add(TLeft left, TRight right)
                {
                        if (leftMap.ContainsKey(left))
                        {
                                throw new ArgumentException("左值已存在",nameof(left));
                        }

                        if (rightMap.ContainsKey(right))
                        {
                                throw new ArgumentException("右值已存在", nameof(right));
                        }
                        leftMap.Add(left, right);
                        rightMap.Add(right, left);
                }

                public bool TryAdd(TLeft left, TRight right)
                {
                        if (leftMap.ContainsKey(left) || rightMap.ContainsKey(right))
                        {
                                return false;
                        }
                        leftMap.Add(left, right);
                        rightMap.Add(right, left);
                        return true;
                }

                public TRight GetRight(TLeft left)
                {
                        return leftMap[left];
                }

                public bool TryGetRight(TLeft left, out TRight right)
                {
                        return leftMap.TryGetValue(left, out right);
                }

                public TLeft GetLeft(TRight right)
                {
                        return rightMap[right];
                }

                public bool TryGetLeft(TRight right, out TLeft left)
                {
                        return rightMap.TryGetValue(right, out left);
                }

                public bool RemoveByLeft(TLeft left)
                {
                        if (leftMap.TryGetValue(left, out TRight right))
                        {
                                return leftMap.Remove(left) && rightMap.Remove(right);
                        }

                        return false;
                }

                public bool RemoveByRight(TRight right)
                {
                        if (rightMap.TryGetValue(right,out TLeft left))
                        {
                                return leftMap.Remove(left) && rightMap.Remove(right);
                        }
                        return false;
                }

                public bool ContainsLeft(TLeft left)
                {
                        return leftMap.ContainsKey(left);
                }

                public bool ContainsRight(TRight right)
                {
                        return rightMap.ContainsKey(right);
                }

                public void Clear()
                {
                        leftMap.Clear();
                        rightMap.Clear();
                }

                public IEnumerator<KeyValuePair<TLeft, TRight>> GetEnumerator()
                {
                        return leftMap.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                        return GetEnumerator();
                }
        }
}