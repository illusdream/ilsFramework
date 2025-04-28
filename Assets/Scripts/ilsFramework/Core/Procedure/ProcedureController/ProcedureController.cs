using System;
using System.Collections.Generic;

namespace ilsFramework.Core
{
    public class ProcedureController : FSM
    {
        protected Stack<Type> _procedureUsingStack;

        public ProcedureController() : base()
        {
            _procedureUsingStack = new Stack<Type>();
        }

        protected override void AddState(Type procedureNodeType)
        {
            if (Activator.CreateInstance(procedureNodeType) is IState procedureNode && states.TryAdd(procedureNodeType,procedureNode))
            {
                procedureNode.Owner = this;
                procedureNode.OnInit();
            }
        }
        
        public void ChangeProcedureByPopStack()
        {
            while (_procedureUsingStack.TryPeek(out var type))
            {
                _procedureUsingStack.Pop();
                ChangeState(type);
                return;
            }
        }

        public override void ChangeState(Type procedureNodeType)
        {
            if (!states.TryGetValue(procedureNodeType, out IState procedureNode))
            {
                return;
            }
            if (_currentState != null)
            {
                _currentState.IsExecuting = false;
                _currentState.OnExit();
                _procedureUsingStack.Push(_currentState.GetType());
            }
            _currentState = procedureNode;

            _currentState.IsExecuting = true;
            _currentState.OnEnter();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _procedureUsingStack.Clear();
        }
    }
}