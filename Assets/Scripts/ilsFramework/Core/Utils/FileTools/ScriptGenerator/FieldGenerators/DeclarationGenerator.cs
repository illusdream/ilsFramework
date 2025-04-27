using System.Text;

namespace ilsFramework.Core
{
    public abstract class DeclarationGenerator : IStatementGenerator
    {
        private readonly EFieldDeclarationMode _fieldDeclarationMode;
        private readonly EAccessType accessType;
        private readonly string description;
        private readonly string name;

        public DeclarationGenerator(EFieldDeclarationMode fieldDeclarationMode, EAccessType accessType, string name, string description = null)
        {
            _fieldDeclarationMode = fieldDeclarationMode;
            this.accessType = accessType;
            this.name = name;
            this.description = description;
        }

        public void Generate(StringBuilder builder, string prefix)
        {
            var access = FileUtils.AccessToString(accessType);
            var fieldDe = FileUtils.FieldDeclarationToString(_fieldDeclarationMode);
            GetTypeNameBind(out var typeName, out var fieldValue);
            if (description != null)
            {
                builder.AppendLine($"{prefix}/// <summary>");
                builder.AppendLine(prefix + "///" + description);
                builder.AppendLine($"{prefix}/// </summary>");
            }

            builder.AppendLine($"{prefix}{access} {fieldDe}{typeName} {name}{fieldValue};");
        }

        public abstract void GetTypeNameBind(out string typeName, out string value);
    }
}