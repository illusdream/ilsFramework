using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ilsFramework.Core
{
    /// <summary>
    ///     通用有限状态机
    /// </summary>
    public class OldFSM<T> where T : IEquatable<T>
    {
        /// <summary>
        ///     从任意状态转换的状态转换列表
        /// </summary>
        private readonly TranslationList _anyStateTranslations;

        // T:对应的StateID
        //List<(ITranslation,T): 
        //      ITranslation 转换的集合
        //      T : 转换至的状态
        private readonly Dictionary<T, TranslationList> _translations;

        [ShowInInspector] private IState _currentState;

        private T _currentStateID;


        /// <summary>
        ///     包含状态->状态转换与any ->状态转换的 运行时状态转换
        /// </summary>
        [ShowInInspector] private TranslationList _currentTranslations;

        [ShowInInspector] private Dictionary<T, IState> _states;

        private T DefaultStateID;


        public OldFSM()
        {
            _states = new Dictionary<T, IState>();
            _translations = new Dictionary<T, TranslationList>();
            _anyStateTranslations = new TranslationList();
        }

        public bool IsRunning { get; private set; }

        public OldFSM<T> SetDefaultState(T initialState)
        {
            DefaultStateID = initialState;
            return this;
        }

        public OldFSM<T> Init()
        {
            foreach (var state in _states.Values) state.OnInit();
            return this;
        }

        public OldFSM<T> Start()
        {
            IsRunning = true;
            ChangeState(DefaultStateID);
            return this;
        }

        public OldFSM<T> Pause()
        {
            IsRunning = false;
            if (_currentState is OldFSM<T> subFsm) subFsm.Pause();
            return this;
        }

        public OldFSM<T> Continue()
        {
            IsRunning = true;
            UpdateCurrentTranslations();
            if (_currentState is OldFSM<T> subFsm) subFsm.Continue();
            return this;
        }

        public virtual void Update()
        {
            if (_currentState == null || _currentStateID == null || !IsRunning) return;
            CheckTransitions();

            _currentState.OnUpdate();
        }

        public virtual void FixedUpdate()
        {
            if (_currentState == null || _currentStateID == null || !IsRunning) return;
            _currentState.OnFixedUpdate();
        }


        public virtual void OnDestroy()
        {
        }

        public void Destroy()
        {
            OnDestroy();
            foreach (var state in _states) state.Value.OnDestroy();

            _states.Clear();
            _translations.Clear();
        }


        /// <summary>
        ///     检查转换条件，看是否需要转换
        /// </summary>
        private void CheckTransitions()
        {
            if (_currentTranslations.CanTranslateTo(out var targetStateID)) ChangeState(targetStateID);
        }

        public IState GetState(T stateID)
        {
            if (!_states.TryGetValue(stateID, out var state))
            {
#if UNITY_EDITOR
                Debug.LogError($"状态机获取状态失败,检查TargetID：{stateID.ToString()}对应添加状态代码");
#endif
            }

            return state;
        }

        public void ChangeState(T TargetID)
        {
            var nextState = GetState(TargetID);

            if (nextState == null) return;
            _currentState?.OnExit();

            _currentState = nextState;
            _currentStateID = TargetID;
            UpdateCurrentTranslations();

            _currentState.OnEnter();
        }


        public OldFSM<T> AddState(T StateID, IState state)
        {
            if (!_states.TryAdd(StateID, state))
            {
#if UNITY_EDITOR
                Debug.LogError("状态添加失败");
#endif
            }

            return this;
        }

        public OldFSM<T> RemoveState(T StateID)
        {
            if (IsRunning && StateID.Equals(_currentStateID)) ChangeState(DefaultStateID);
            _states.Remove(StateID);
            return this;
        }

        public OldFSM<T> AddTranslation(T startStateID, ITranslation translation, T toStateID)
        {
            if (_translations.TryGetValue(startStateID, out var translationList))
                translationList.AddTranslation(translation, toStateID);
            else
                _translations.Add(startStateID, new TranslationList().AddTranslation(translation, toStateID));

            if (IsRunning && startStateID.Equals(_currentStateID)) UpdateCurrentTranslations();
            return this;
        }

        public OldFSM<T> RemoveTranslation(T startStateID, string translationName, T toStateID)
        {
            if (_translations.TryGetValue(startStateID, out var translationList)) translationList.RemoveTranslation(translationName, toStateID);

            if (IsRunning && startStateID.Equals(_currentStateID)) UpdateCurrentTranslations();
            return this;
        }


        public OldFSM<T> AddAnyStateTranslation(ITranslation translation, T toStateID)
        {
            _anyStateTranslations.AddTranslation(translation, toStateID);

            if (IsRunning) UpdateCurrentTranslations();
            return this;
        }

        public OldFSM<T> RemoveAnyStateTranslation(string translationName, T toStateID)
        {
            _anyStateTranslations.RemoveTranslation(translationName, toStateID);
            if (IsRunning) UpdateCurrentTranslations();
            return this;
        }

        private void UpdateCurrentTranslations()
        {
            if (_translations.TryGetValue(_currentStateID, out var translationList)) _currentTranslations = translationList.Combine(_anyStateTranslations);
        }

        private struct TranslationData : IComparable
        {
            private readonly ITranslation translation;
            private T targetStateID;

            public TranslationData(ITranslation translation, T stateID)
            {
                this.translation = translation;
                targetStateID = stateID;
            }

            public int CompareTo(object obj)
            {
                return translation.Priority.CompareTo(((TranslationData)obj).translation.Priority);
            }

            public bool CanTransitionTo(out T targetStateID)
            {
                targetStateID = this.targetStateID;
                if (translation.CanTranslate())
                {
                    translation.OnTranslate();
                    return true;
                }

                return false;
            }

            public override bool Equals(object obj)
            {
                if (obj is not TranslationData)
                    return false;
                return translation.Name == ((TranslationData)obj).translation.Name && targetStateID.Equals(((TranslationData)obj).targetStateID);
            }

            public bool Equal(string translationName, T targetStateID)
            {
                return translation.Name == translationName && this.targetStateID.Equals(targetStateID);
            }
        }

        private class TranslationList
        {
            public readonly List<TranslationData> translations;

            public TranslationList()
            {
                translations = new List<TranslationData>();
            }

            public TranslationList AddTranslation(ITranslation translation, T stateID)
            {
                var tData = new TranslationData(translation, stateID);
                translations.Add(tData);
                translations.Sort();
                return this;
            }

            public TranslationList RemoveTranslation(string translationName, T stateID)
            {
                translations.RemoveAll(t => t.Equal(translationName, stateID));
                return this;
            }

            private void AddTranslationRange(List<TranslationData> translations)
            {
                this.translations.AddRange(translations);
            }

            private void SortAllTranslations()
            {
                translations.Sort();
            }

            public bool CanTranslateTo(out T targetStateID)
            {
                for (var i = 0; i < translations.Count; i++)
                {
                    var translation = translations[i];
                    if (translation.CanTransitionTo(out targetStateID)) return true;
                }

                targetStateID = default;
                return false;
            }

            public TranslationList Combine(TranslationList other)
            {
                var result = new TranslationList();
                result.AddTranslationRange(translations);
                result.AddTranslationRange(other.translations);
                result.SortAllTranslations();
                return result;
            }
        }
    }
}