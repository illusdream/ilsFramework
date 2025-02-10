using System.Text;

namespace ilsFramework
{
    public interface IStatementGenerator
    {
        public void Generate(StringBuilder builder,string prefix);
    }
}