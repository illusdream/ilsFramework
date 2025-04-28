namespace ilsFramework.Core
{
    [AutoBuildOrLoadConfig("PoolConfig")]
    public class PoolConfig : ConfigScriptObject
    {
        public override string ConfigName => "PoolConfig";

        public EPoolCollectionCheck PoolCollectionCheck;
    }
}