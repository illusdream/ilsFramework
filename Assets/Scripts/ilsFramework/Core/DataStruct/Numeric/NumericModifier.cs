using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ilsFramework.Core
{
    [Serializable]
    public struct NumericModifier : IEquatable<NumericModifier>
    {
        public NumericModifier(float @base = 0, float additive = 1, float mulitive = 1, float flat = 0)
        {
            Base = @base;
            Additive = additive;
            Mulitive = mulitive;
            Flat = flat;
            OnBaseChanged = null;
            OnAdditiveChanged = null;
            OnMultiplicativeChanged = null;
            OnFlatChanged = null;
        }

        public event Action<float, float> OnBaseChanged;
        [ShowInInspector]
        public float Base { get; private set; }

        public event Action<float,float> OnAdditiveChanged; 
        [ShowInInspector]
        public float Additive { get; private set; }

        public event Action<float, float> OnMultiplicativeChanged;
        [ShowInInspector]
        public float Mulitive { get; private set; }
    
        public event Action<float, float> OnFlatChanged;
        [ShowInInspector]
        public float Flat { get; private set; }

        public NumericModifier SetBase(float @base)
        {
            OnBaseChanged?.Invoke(Base, @base);
            Base = @base;
            return this;
        }
        public NumericModifier SetAdditive(float additive)
        {
            OnAdditiveChanged?.Invoke(Additive, additive);
            Additive = additive;
            return this;
        }
        public NumericModifier SetMulitive(float mulitive)
        {
            OnMultiplicativeChanged?.Invoke(Mulitive, mulitive);
            Mulitive = mulitive;
            return this;
        }
        public NumericModifier SetFlat(float flat)
        {
            OnFlatChanged?.Invoke(Flat, flat);
            Flat = flat;
            return this;
        }

        public static NumericModifier operator +(NumericModifier x, NumericModifier y)
        {
            return new NumericModifier(x.Base + y.Base, x.Additive + y.Additive, x.Mulitive + y.Mulitive, x.Flat + y.Flat);
        }
    
        public float Apply(float baseValue)
        {
            return (baseValue * Additive + Base) * Mulitive + Flat;
        }

        public override bool Equals(object obj)
        {
            if (obj is NumericModifier numeric)
            {
                if (Mathf.Approximately(Base, numeric.Base) && Mathf.Approximately(Additive, numeric.Additive) && Mathf.Approximately(Mulitive, numeric.Mulitive) && Mathf.Approximately(Flat, numeric.Flat))
                    return true;
                return false;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("Base:{0} Additive:{0} Mulitive:{0} Flat:{0}", Base, Additive, Mulitive, Flat);
        }

        public bool Equals(NumericModifier other)
        {
            return Equals(OnBaseChanged, other.OnBaseChanged) && Equals(OnAdditiveChanged, other.OnAdditiveChanged) && Equals(OnMultiplicativeChanged, other.OnMultiplicativeChanged) && Equals(OnFlatChanged, other.OnFlatChanged) && Base.Equals(other.Base) && Additive.Equals(other.Additive) && Mulitive.Equals(other.Mulitive) && Flat.Equals(other.Flat);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OnBaseChanged, OnAdditiveChanged, OnMultiplicativeChanged, OnFlatChanged, Base, Additive, Mulitive, Flat);
        }
    }
}