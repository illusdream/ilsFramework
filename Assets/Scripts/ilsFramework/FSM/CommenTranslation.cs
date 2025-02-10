using System;
using UnityEngine;

namespace ilsFramework
{
    public class CommenTranslation : ITranslation
    {
        private Func<bool> transCondition;
        private Action onTranslationAction;

        public CommenTranslation(Func<bool> transCondition, Action onTranslationAction = null, int priority = 0,
            string name = "")
        {
            this.transCondition = transCondition;
            this.onTranslationAction = onTranslationAction;
            this.Priority = priority;
            this.Name = name;
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