using System;

namespace ilsFramework
{
    public static class FileUtils
    {
        public static string AccessToString(EAccessType access)
        {
            return  access switch
            {
                EAccessType.Private => "private",
                EAccessType.Protected => "protected",
                EAccessType.Public => "public",
                EAccessType.ProtectedInternal => "protected internal",
                EAccessType.Internal => "internal",
                _ => "private"
            };
        }

        public static string FieldDeclarationToString(EFieldDeclarationMode fieldDeclaration)
        {
            return fieldDeclaration switch
            {
                EFieldDeclarationMode.Const => "const ",
                EFieldDeclarationMode.Static => "static ",
                EFieldDeclarationMode.ReadOnly => "readonly ",
                EFieldDeclarationMode.StaticReadOnly => "static readonly ",
                EFieldDeclarationMode.Null => "",
                _ => ""
            };
        }
    }
}