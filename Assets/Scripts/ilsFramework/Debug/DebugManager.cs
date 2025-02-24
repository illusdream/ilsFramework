using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ilsFramework
{
    public class DebugManager : ManagerSingleton<DebugManager>,IManager,IAssemblyForeach
    {
        private Dictionary<Type, IParser> parsers;
        
        private Dictionary<string,CommendContainer> commands;
        
        public void Init()
        {
            Application.logMessageReceived += HandleUnityLog;
            
            parsers = new Dictionary<Type, IParser>();
            commands = new Dictionary<string, CommendContainer>();
        }

        public void ForeachCurrentAssembly(Type[] types)
        {
            foreach (var type in types)
            {
                if (typeof(IParser).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                {
                    var instance = Activator.CreateInstance(type) as IParser;
                    parsers.Add(instance.GetTargetType(), instance);
                }

                if (typeof(ICommendSet).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                {
                    var attr = type.GetCustomAttribute<DebugCommendAttribute>();
                    if ( attr != null )
                    {
                        commands.Add(attr.CommendTargetMethod,new CommendContainer(attr,type));
                    }
                }
            }
        }
        
        
        public void Update()
        {
            
        }

        public void LateUpdate()
        {
            
        }

        public void FixedUpdate()
        {
            
        }

        public void OnDestroy()
        {
            
        }

        public void OnDrawGizmos()
        {
            
        }

        public void OnDrawGizmosSelected()
        {
            
        }
        
        private void HandleUnityLog(string log, string stackTrace, LogType type)
        {
            string color = type switch
            {
                LogType.Error => "#FF0000",
                LogType.Warning => "#FFFF00",
                LogType.Exception => "#FF00FF",
                _ => "#FFFFFF"
            };
            log = $"\n{log}";
        }

        public bool TryGetParser(Type type,out IParser parser)
        {
            return parsers.TryGetValue(type, out parser);
        }

        public bool TryGetCommendContainer(string commendName, out CommendContainer commendContainer)
        {
            return commands.TryGetValue(commendName, out commendContainer);
        }

        public void TryExecuteCommand(string command)
        {
            string[] commandNameAndArgs = command.Split(' ');
            if (TryGetCommendContainer(commandNameAndArgs[0], out CommendContainer commendContainer))
            {
                commendContainer.Execute(commandNameAndArgs.Skip(1).ToArray());
            }
        }
    }
}