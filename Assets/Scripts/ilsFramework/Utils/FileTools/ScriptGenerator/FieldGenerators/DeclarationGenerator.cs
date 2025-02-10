using System;
using System.Text;

namespace ilsFramework
{
    public abstract class DeclarationGenerator : IStatementGenerator
    {
        private EFieldDeclarationMode _fieldDeclarationMode;
        private EAccessType accessType;
        private string name;
        private string description;

        public DeclarationGenerator(EFieldDeclarationMode fieldDeclarationMode, EAccessType accessType, string name, string description = null)
        {
            _fieldDeclarationMode = fieldDeclarationMode;
            this.accessType = accessType;
            this.name = name;
            this.description = description;
        }

        public void Generate(StringBuilder builder,string prefix)
        {
            string access = FileUtils.AccessToString(accessType);
            string fieldDe = FileUtils.FieldDeclarationToString(_fieldDeclarationMode);
            GetTypeNameBind(out string typeName, out string fieldValue );
            if (description !=null)
            {
                builder.AppendLine($"{prefix}/// <summary>");
                builder.AppendLine(prefix+"///"+description);
                builder.AppendLine($"{prefix}/// </summary>");
            }
            builder.AppendLine($"{prefix}{access} {fieldDe}{typeName} {name}{fieldValue};");
        }

        public abstract void GetTypeNameBind(out string typeName, out string value);
    }
}