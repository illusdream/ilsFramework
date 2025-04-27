using System.Collections.Generic;
using System.Text;

namespace ilsFramework.Core
{
    public class ClassGenerator : IStatementGenerator
    {
        private readonly EAccessType access;

        private readonly string className;

        private readonly string inheritanceClassName;

        private readonly List<IStatementGenerator> statements;

        public ClassGenerator(EAccessType access, string className, string inheritanceClassName = null, List<IStatementGenerator> statements = null)
        {
            this.access = access;
            this.className = className;
            this.inheritanceClassName = inheritanceClassName;
            this.statements = statements ?? new List<IStatementGenerator>();
        }

        public ClassGenerator(EAccessType access, string className, string inheritanceClassName = null, params IStatementGenerator[] statements)
        {
            this.access = access;
            this.className = className;
            this.inheritanceClassName = inheritanceClassName;
            this.statements = new List<IStatementGenerator>(statements);
        }

        public void Generate(StringBuilder builder, string prefix)
        {
            var _access = FileUtils.AccessToString(access);
            var inheritance = inheritanceClassName == null ? string.Empty : $": {inheritanceClassName}";
            builder.AppendLine($"{prefix}{_access} class {className} {inheritance}");
            builder.AppendLine($"{prefix}{{");
            if (statements != null)
            {
                var nextPrefix = prefix + "\t";
                foreach (var iStatement in statements) iStatement.Generate(builder, nextPrefix);
            }

            builder.AppendLine($"{prefix}}}");
        }

        public void Append(IStatementGenerator statement)
        {
            statements.Add(statement);
        }
    }
}