#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ilsFramework.Core
{
    public static class FileUtils
    {
        public static string AccessToString(EAccessType access)
        {
            return access switch
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

#if UNITY_EDITOR
        public static void AssetFolder_CheckOrCreateFolder(string path)
        {
            var folders = path.Split("/");
            var parentFolder = "Assets";
            foreach (var folder in folders)
            {
                var cFolder = parentFolder + "/" + folder;
                if (!AssetDatabase.IsValidFolder(cFolder)) AssetDatabase.CreateFolder(parentFolder, folder);
                parentFolder = parentFolder + "/" + folder;
            }
        }
#endif
    }
}