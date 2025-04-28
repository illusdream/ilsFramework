using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace ilsFramework.Core
{
    public class FSM
    {
        [ShowInInspector]
        protected Dictionary<Type, IState> states;
        
        protected IState _currentState;

        public FSM()
        {
                states = new Dictionary<Type, IState>();

        }

        public virtual void AddState<T>() where T : IState
        {
                AddState(typeof(T));
        }
        
        protected virtual void AddState(Type stateType)
        {
                if (Activator.CreateInstance(stateType) is IState procedureNode && states.TryAdd(stateType,procedureNode))
                {
                        procedureNode.Owner = this;
                        procedureNode.OnInit();
                }
        }

        public virtual void RemoveState<T>() where T : IState
        {
                states.Remove(typeof(T));
        }
        
        public virtual void ChangeState<T>() where T : IState
        {
                ChangeState(typeof(T));
        }

        public virtual void ChangeState(Type procedureNodeType)
        {
                if (!states.TryGetValue(procedureNodeType, out IState procedureNode))
                {
                        return;
                }
                if (_currentState != null)
                {
                        _currentState.IsExecuting = false;
                        _currentState.OnExit();
                }
                _currentState = procedureNode;

                _currentState.IsExecuting = true;
                _currentState.OnEnter();

        }

        public virtual void SetCurrentState<T>() where T : IState
        {
                if (!states.ContainsKey(typeof(T)))
                {
                        AddState<T>();
                }
                _currentState = states[typeof(T)];
        }
        
        public virtual void StartState<T>() where T : IState
        {
                if (!states.ContainsKey(typeof(T)))
                {
                        AddState<T>();
                }
                _currentState = states[typeof(T)];
                _currentState.OnEnter();
                _currentState.IsExecuting = true;
        }

        public virtual void Update()
        {
                _currentState?.OnUpdate();
        }

        public virtual void FixedUpdate()
        {
                _currentState?.OnFixedUpdate();
        }

        public virtual void LateUpdate()
        {
                _currentState?.OnLateUpdate();
        }

        public virtual void LogicUpdate()
        {
                _currentState?.OnLogicUpdate();
        }

        public virtual void OnDestroy()
        {
                _currentState?.OnExit();

                foreach (var procedureNode in states.Values)
                {
                        procedureNode.OnDestroy();
                }
                states.Clear();
        }
    }
}