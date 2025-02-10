namespace ilsFramework
{
    public class StringFieldGenerator : DeclarationGenerator
    {
        private string _value;
        public StringFieldGenerator(EFieldDeclarationMode fieldDeclarationMode, EAccessType accessType, string name, string value, string description = null) : base(fieldDeclarationMode, accessType,  name,  description)
        {
            this._value = value;
        }

        public override void GetTypeNameBind(out string typeName, out string value)
        {
            typeName = "string";
            value = _value == null ? string.Empty :$" = \"{_value}\"" ;
        }
    }
}