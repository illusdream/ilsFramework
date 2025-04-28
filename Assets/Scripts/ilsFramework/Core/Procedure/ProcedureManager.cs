using System;
using Sirenix.OdinInspector;

namespace ilsFramework.Core
{
    public partial class ProcedureManager : ManagerSingleton<ProcedureManager>,IAssemblyForeach
    {
        [ShowInInspector]
        ProcedureController _procedureController;
    
        ProcedureConfig procedureConfig;
        
        public bool GameProcedureEnabled { get; private set; }
        public override void OnInit()
        {
            procedureConfig = Config.GetConfig<ProcedureConfig>();
            
            GameProcedureEnabled = procedureConfig.EnableCommenProcedure;
        
            _procedureController = new ProcedureController();
        }
        
        
        public void ForeachCurrentAssembly(Type[] types)
        {
            foreach (var type in types)
            {
                if (typeof(ProcedureInitializer).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    if (Activator.CreateInstance(type) is ProcedureInitializer instance)
                    {
                        111.LogSelf();
                        instance.InitializeProcedure(_procedureController);
                        break;
                    }

                }
            }
        }

        public override void OnUpdate()
        {
            if (GameProcedureEnabled)
            {
                _procedureController.Update();
            }
        }

        public override void OnLateUpdate()
        {
            if (GameProcedureEnabled)
            {
                _procedureController.LateUpdate();
            }
        }

        public override void OnLogicUpdate()
        {
            if (GameProcedureEnabled)
            {
                _procedureController.LogicUpdate();
            }
        }

        public override void OnFixedUpdate()
        {
            if (GameProcedureEnabled)
            {
                _procedureController.FixedUpdate();
            }
        }

        public override void OnDestroy()
        {
           _procedureController.OnDestroy();
        }

        public override void OnDrawGizmos()
        {
           
        }

        public override void OnDrawGizmosSelected()
        {
          
        }

    }
}