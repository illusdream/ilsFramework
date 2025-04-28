using System;
using UnityEngine;

namespace ilsFramework.Core
{
    public class CommenTranslation : ITranslation
    {
        private readonly Action onTranslationAction;
        private readonly Func<bool> transCondition;

        public CommenTranslation(Func<bool> transCondition, Action onTranslationAction = null, int priority = 0,
            string name = "")
        {
            this.transCondition = transCondition;
            this.onTranslationAction = onTranslationAction;
            Priority = priority;
            Name = name;
        }

        public bool CanTranslate()
        {
            return transCondition.Invoke();
        }

        public void OnTranslate()
        {
            Debug.Log(onTranslationAction);
            onTranslationAction?.Invoke();
        }


        public int Priority { get; }
        public string Name { get; }
    }
}