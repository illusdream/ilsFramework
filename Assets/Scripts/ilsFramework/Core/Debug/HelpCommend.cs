namespace ilsFramework.Core
{
    [DebugCommend("Help")]
    public class HelpCommend : ICommendSet
    {
        public void Help()
        {
            //打印所有Commend
            foreach (var command in DebugManager.Instance.GetAllCommands())
            {
                var cs = command.Item2.ToStringList();
                for (var i = 0; i < cs.Count; i++)
                {
                    var final = $"{command.Item1}:" + cs[i] + "\n";
                    final.LogSelf();
                }
            }
        }
    }
}