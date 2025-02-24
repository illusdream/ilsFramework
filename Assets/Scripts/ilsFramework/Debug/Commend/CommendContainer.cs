using System;
using System.Collections.Generic;
using System.Linq;

namespace ilsFramework
{
    public class CommendContainer
    {
        private List<CommendInvoker> commendInvokers;
        
        private ICommendSet commendSet;

        public CommendContainer(DebugCommendAttribute commendAttribute, Type targetType)
        {
            commendInvokers = new List<CommendInvoker>();
            
            commendSet = Activator.CreateInstance(targetType) as ICommendSet;
            
            var allMethods = targetType.GetMethods();

            var needMethod = allMethods.Where((m) => m.Name == commendAttribute.CommendTargetMethod);

            foreach (var methodInfo in needMethod)
            {
                commendInvokers.Add(new CommendInvoker(methodInfo));
            }
        }


        public void Execute(string[] commandArgs)
        {
            List<CommendInvoker> currectCommendInvokers = new List<CommendInvoker>(commendInvokers);
            List<CommendInvoker> resultCommendInvokers = new List<CommendInvoker>();
            //开始解析
            for (int i = 0; i < commandArgs.Length; i++)
            {
                var args = commandArgs[i];
                foreach (var invoker in currectCommendInvokers)
                {
                    if (invoker.IsVialdArgsCount(i))
                    {
                        var needType = invoker.GetTargetType(i);
                        //查询解析器并获取结果
                        if (DebugManager.Instance.TryGetParser(needType, out var parser) && parser.TryParse(args,out var result))
                        {
                            invoker.PushArgs(result);
                            resultCommendInvokers.Add(invoker);
                        }
                    }
                }
                (currectCommendInvokers, resultCommendInvokers) = (resultCommendInvokers, currectCommendInvokers);
                resultCommendInvokers.Clear();
            }
            //查找第一个可以执行的
            foreach (var invoker in currectCommendInvokers)
            {
                if (invoker.TryInvoke(commendSet))
                {
                    break;
                }
            }

            foreach (var invoker in commendInvokers)
            {
                invoker.Reset();
            }
        }
    }
}