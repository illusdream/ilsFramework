namespace ilsFramework.Core
{
    public class ProcedureNode : FSMState
    {
        
        public virtual void ChangeStateByPopStack()
        {
            ((ProcedureController)Owner).ChangeProcedureByPopStack(); 
        }
    }
}