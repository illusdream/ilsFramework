using System;
using System.Collections.Generic;
using System.Text;

namespace ilsFramework
{
    public class EnumGenerator : IStatementGenerator
    {      
        private EAccessType access;
        
        private string enumName;

        private List<EnumGBind> enumValues;
        
        private string description;

        public EnumGenerator(EAccessType access, string enumName, string description =null, params EnumGBind[] enumNames)
        {
            this.access = access;
            this.enumName = enumName;
            this.description = description ?? string.Empty;
            this.enumValues = new List<EnumGBind>(enumNames);
        }
        public EnumGenerator(EAccessType access, string enumName, List<(string,string)> enumNames,string description =null)
        {
            this.access = access;
            this.enumName = enumName;
            this.description = description ?? string.Empty;
            this.enumValues = new List<EnumGBind>();
            foreach (var valueTuple in enumNames)
            {
                this.enumValues.Add(valueTuple);
            }
        }
        public EnumGenerator(EAccessType access, string enumName, List<EnumGBind> enumNames,string description =null)
        {
            this.access = access;
            this.enumName = enumName;
            this.description = description ?? string.Empty;
            this.enumValues = new List<EnumGBind>(enumNames);
        }
        public void Append(EnumGBind enumName)
        {
            enumValues.Add(enumName);
        }

        public void Generate(StringBuilder builder, string prefix)
        {
            string _access = FileUtils.AccessToString(access);
            if (description != null)
            {
                builder.AppendLine($"{prefix}/// <summary>");
                builder.AppendLine(prefix+"///"+description);
                builder.AppendLine($"{prefix}/// </summary>");
            }
            builder.AppendLine($"{prefix}{_access} enum {enumName}");
            builder.AppendLine($"{prefix}{{");
            if (enumValues != null)
            {
                string nextPrefix = prefix + "\t";
                for (int i = 0; i < enumValues.Count; i++)
                {
                    enumValues[i].Generate(builder, nextPrefix);
                    if (i != enumValues.Count- 1)
                    {
                        builder.AppendLine(",");
                    }
                    else
                    {
                        builder.AppendLine();
                    }
                }
            }
            builder.AppendLine($"{prefix}}}");
        }
        public struct EnumGBind
        {
            string enumName;
            string description;
            private int TargetValue;
            private bool hasSetValue;
            
            public static implicit operator EnumGBind((string, string) value)
            {
                return new EnumGBind() {enumName = value.Item1,description = value.Item2};
            }
            
            public static implicit operator EnumGBind((string, string,int) value)
            {
                var instance = new EnumGBind() { enumName = value.Item1, description = value.Item2 };
                instance.SetValue(value.Item3);
                return instance;
            }
            public static implicit operator EnumGBind((string,int) value)
            {
                var instance = new EnumGBind() { enumName = value.Item1};
                instance.SetValue(value.Item2);
                return instance;
            }
            public void SetValue(int value)
            {
                hasSetValue = true;
                TargetValue = value;
            }
            
            public void Generate(StringBuilder builder, string prefix)
            {
                if (description !=null)
                {
                    builder.AppendLine($"{prefix}/// <summary>");
                    builder.AppendLine(prefix+"///"+description);
                    builder.AppendLine($"{prefix}/// </summary>");
                }
                string value = hasSetValue? $" = {TargetValue}" : "";
                builder.Append($"{prefix}{enumName}{value}");
            }
        }
    }


}