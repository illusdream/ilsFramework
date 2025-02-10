using System.Collections.Generic;
using System.Text;

namespace ilsFramework
{
    public  class ClassGenerator : IStatementGenerator
    {
        
        
        private EAccessType access;
        
        private string className;

        private string inheritanceClassName;
        
        private List<IStatementGenerator> statements;

        public ClassGenerator(EAccessType access, string className, string inheritanceClassName = null, List<IStatementGenerator> statements = null)
        {
            this.access = access;
            this.className = className;
            this.inheritanceClassName = inheritanceClassName;
            this.statements = statements ?? new List<IStatementGenerator>();
        }
        public ClassGenerator(EAccessType access, string className, string inheritanceClassName = null,params IStatementGenerator[] statements)
        {
            this.access = access;
            this.className = className;
            this.inheritanceClassName = inheritanceClassName;
            this.statements = new List<IStatementGenerator>(statements);
        }

        public void Append(IStatementGenerator statement)
        {
            statements.Add(statement);
        }

        public void Generate(StringBuilder builder,string prefix)
        {
            string _access = FileUtils.AccessToString(access);
            string inheritance = inheritanceClassName == null ? string.Empty: $": {inheritanceClassName}";
            builder.AppendLine($"{prefix}{_access} class {className} {inheritance}");
            builder.AppendLine($"{prefix}{{");
            if (statements != null)
            {
                string nextPrefix = prefix + "\t";
                foreach (var iStatement in statements)
                {
                    iStatement.Generate(builder,nextPrefix);
                }
            }
            builder.AppendLine($"{prefix}}}");
            
        }
    }
}