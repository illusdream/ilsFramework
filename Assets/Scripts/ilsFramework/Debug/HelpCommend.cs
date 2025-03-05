namespace ilsFramework
{
    [DebugCommend("Help")]
    public class HelpCommend : ICommendSet
    {
        
        public void Help()
        {
            //打印所有Commend
            foreach (var  command in DebugManager.Instance.GetAllCommands())
            {


                var cs = command.Item2.ToStringList();
                for (int i = 0; i < cs.Count; i++)
                {
                    string final =$"{command.Item1}:"+cs[i] + "\n";
                    final.LogSelf();
                }
            }
        }
    }
}