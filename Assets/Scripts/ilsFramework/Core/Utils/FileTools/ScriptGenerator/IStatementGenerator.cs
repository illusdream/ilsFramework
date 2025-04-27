using System.Text;

namespace ilsFramework.Core
{
    public interface IStatementGenerator
    {
        public void Generate(StringBuilder builder, string prefix);
    }
}