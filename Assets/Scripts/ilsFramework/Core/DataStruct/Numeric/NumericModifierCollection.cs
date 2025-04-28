using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace ilsFramework.Core
{
        public class NumericModifierCollection : IDictionary<string,NumericModifier>
        {
                [ShowInInspector]
                private Dictionary<string, NumericModifier> _modifiers = new Dictionary<string, NumericModifier>();
                [ShowInInspector]
                public float Base { get; private set; } = 0;
                [ShowInInspector]
                public float Additive { get; private set; } = 1;
                [ShowInInspector]
                public float Mulitive { get; private set; } = 1;
                [ShowInInspector]
                public float Flat  { get; private set; } = 0;

                public IEnumerator<KeyValuePair<string, NumericModifier>> GetEnumerator()
                {
                        return _modifiers.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                        return GetEnumerator();
                }

                public void Add(KeyValuePair<string, NumericModifier> item)
                {
                        if (_modifiers.TryAdd(item.Key, item.Value))
                        {
                                Base += item.Value.Base;
                                Additive += (item.Value.Additive-1);
                                Mulitive += (item.Value.Mulitive -1);
                                Flat += item.Value.Flat;
                                item.Value.OnBaseChanged += OnBaseChanged;
                                item.Value.OnAdditiveChanged += OnAdditiveChanged;
                                item.Value.OnMultiplicativeChanged += OnMulitiveChanged;
                                item.Value.OnFlatChanged += OnFlatChanged;
                        }
                }

                public void Clear()
                {
                        foreach (var modifier in _modifiers.Values)
                        {
                                modifier.OnBaseChanged -= OnBaseChanged;
                                modifier.OnAdditiveChanged -= OnAdditiveChanged;
                                modifier.OnMultiplicativeChanged -= OnMulitiveChanged;
                                modifier.OnFlatChanged -= OnFlatChanged;
                        }

                        Base = 0;
                        Additive = 1;
                        Mulitive = 1;
                        Flat = 0;
                        _modifiers.Clear();
                }

                public bool Contains(KeyValuePair<string, NumericModifier> item)
                {
                        return _modifiers[item.Key].Equals(item.Value);
                }

                public void CopyTo(KeyValuePair<string, NumericModifier>[] array, int arrayIndex)
                {
                        Add(array[arrayIndex]);
                }

                public bool Remove(KeyValuePair<string, NumericModifier> item)
                {
                        return _modifiers.Remove(item.Key);
                }

                public int Count => _modifiers.Count;
                public bool IsReadOnly => false;
                public void Add(string key, NumericModifier value)
                {
                        if (_modifiers.TryAdd(key, value))
                        {
                                Base += value.Base;
                                Additive += value.Additive-1;
                                Mulitive += value.Mulitive-1;
                                Flat += value.Flat;
                                value.OnBaseChanged += OnBaseChanged;
                                value.OnAdditiveChanged += OnAdditiveChanged;
                                value.OnMultiplicativeChanged += OnMulitiveChanged;
                                value.OnFlatChanged += OnFlatChanged;
                        }
                }

                public bool ContainsKey(string key)
                {
                        return _modifiers.ContainsKey(key);
                }

                public bool Remove(string key)
                {
                        if (_modifiers.TryGetValue(key,out var value))
                        {
                                Base -= value.Base;
                                Additive -= (value.Additive-1);
                                Mulitive -= (value.Mulitive-1);
                                Flat -= value.Flat;
                                value.OnBaseChanged -= OnBaseChanged;
                                value.OnAdditiveChanged -= OnAdditiveChanged;
                                value.OnMultiplicativeChanged -= OnMulitiveChanged;
                                value.OnFlatChanged -= OnFlatChanged;
                                return _modifiers.Remove(key);
                        }
                        return false;
                }

                public bool TryGetValue(string key, out NumericModifier value)
                {
                        return _modifiers.TryGetValue(key, out value);
                }

                public NumericModifier this[string key]
                {
                        get
                        {
                                return _modifiers.TryGetValue(key,out var value) ? value : default(NumericModifier);
                        }
                        set
                        {
                                _modifiers[key] = value;
                        }
                }

                public ICollection<string> Keys => _modifiers.Keys;
                public ICollection<NumericModifier> Values => _modifiers.Values;

                private void OnBaseChanged(float old, float newBase)
                {
                        Base += (newBase - old);
                }

                private void OnAdditiveChanged(float old, float newAdd)
                {
                        Additive += (newAdd - old);
                }

                private void OnMulitiveChanged(float old, float newMul)
                {
                        Mulitive += (newMul - old);
                }

                private void OnFlatChanged(float old, float newFlat)
                {
                        Flat += (newFlat - old);
                }
        
                public float Apply(float baseValue)
                {
                        return (baseValue * Additive + Base) * Mulitive + Flat;
                }
                public override string ToString()
                {
                        return $"Base:{Base} Additive:{Additive} Mulitive:{Mulitive} Flat:{Flat}";
                }
        }
}