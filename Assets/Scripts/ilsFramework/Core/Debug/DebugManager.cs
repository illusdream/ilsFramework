using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ilsFramework.Core
{
    public class DebugManager : ManagerSingleton<DebugManager>, IAssemblyForeach
    {
        private Dictionary<string, CommendContainer> commands;

        private DebugConfig config;
        private Dictionary<Type, IParser> parsers;

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
                    if (attr != null) commands.Add(attr.CommendTargetMethod, new CommendContainer(attr, type));
                }
            }
        }
        public override void OnInit()
        {
            config = ConfigManager.Instance.GetConfig<DebugConfig>();
            parsers = new Dictionary<Type, IParser>();
            commands = new Dictionary<string, CommendContainer>();
        }

        public override void OnUpdate()
        {
            
        }

        public override void OnLateUpdate()
        {
           
        }

        public override void OnLogicUpdate()
        {
            
        }
        public override void OnFixedUpdate()
        {
           
        }

        public override void OnDestroy()
        {
            
        }
        public override void OnDrawGizmos()
        {
            
        }

        public override void OnDrawGizmosSelected()
        {
           
        }

        public bool TryGetParser(Type type, out IParser parser)
        {
            return parsers.TryGetValue(type, out parser);
        }

        public bool TryGetCommendContainer(string commendName, out CommendContainer commendContainer)
        {
            return commands.TryGetValue(commendName, out commendContainer);
        }

        public bool TryExecuteCommand(string command)
        {
            var commandNameAndArgs = command.Split(' ');
            if (TryGetCommendContainer(commandNameAndArgs[0], out var commendContainer))
            {
                if (commendContainer.Execute(commandNameAndArgs.Skip(1).ToArray()))
                {
                }
                else
                {
                    $"Commend: {command} 执行失败".ErrorSelf();
                }

                return true;
            }

            return false;
        }

        public List<(string, CommendContainer)> GetAllCommands()
        {
            var list = new List<(string, CommendContainer)>();

            foreach (var container in commands) list.Add((container.Key, container.Value));

            return list;
        }
    }
}