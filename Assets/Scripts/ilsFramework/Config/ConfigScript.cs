namespace ilsFramework
{
    //用于规定Configs相关文件位置/其他需要写在C#脚本里的变量
    public static class ConfigScript
    {

        public static readonly string ResoureceFolder = "Assets/Resources/ilsFramework/";
        /// <summary>
        /// 配置文件夹,使用时一定会加载到游戏中的文件，放在Resource作为重要资源
        /// </summary>
        public static readonly string ConfigsFolder = ResoureceFolder+"Configs/";
        
        
        public static readonly string ScriptsFolder = "Assets/Scripts/ilsFramework/Config";
        
        public static readonly string ScriptesEnumsFolder = ScriptsFolder + "Enums";
    }
}
