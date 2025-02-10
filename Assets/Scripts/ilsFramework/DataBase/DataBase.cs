using System.IO;
using System.Linq;
using System.Text;
using SQLite4Unity3d;
using UnityEngine;

namespace ilsFramework
{
    public enum DataBaseCopyMode
    {
        StreamingToPersistent,
        Custom
    }
    
    public static class DataBase
    {
        public static SQLiteConnection GetStreamingConnection(string dbName,params string[] folders)
        {
#if UNITY_EDITOR
            if (!Directory.Exists($"{Application.streamingAssetsPath}/DataBases"))
            {
                Directory.CreateDirectory($"{Application.streamingAssetsPath}/DataBases");
            }
#endif
            StringBuilder sb = new StringBuilder($"{Application.streamingAssetsPath}/DataBases/");
            if (folders.Length > 0)
            {
                for (int i = 0; i < folders.Length; i++)
                {
                    sb.Append(folders[i]);
                    sb.Append("/");
                }
            }
            sb.Append(dbName);
            sb.Append(".db");
            return new SQLiteConnection(sb.ToString());
        }
        public static SQLiteConnection GetPersistentConnection(string dbName,params string[] folders)
        {
            if (!Directory.Exists($"{Application.persistentDataPath}/DataBases"))
            {
                Directory.CreateDirectory($"{Application.persistentDataPath}/DataBases");
            }
            StringBuilder sb = new StringBuilder($"{Application.persistentDataPath}/DataBases/");
            if (folders.Length > 0)
            {
                for (int i = 0; i < folders.Length; i++)
                {
                    sb.Append(folders[i]);
                    sb.Append("/");
                }
            }
            sb.Append(dbName);
            sb.Append(".db");
            return new SQLiteConnection(sb.ToString());
        }

        public static void CopyDatabase(string destDbPath, string srcDbPath,DataBaseCopyMode copyMode)
        {
            string cDestDbPath = copyMode switch
            {
                DataBaseCopyMode.StreamingToPersistent => $"{Application.persistentDataPath}/DataBases/{destDbPath}",
                DataBaseCopyMode.Custom => destDbPath,
                _ => destDbPath
            };
            string cSrcDbPath = copyMode switch
            {
                DataBaseCopyMode.StreamingToPersistent => $"{Application.streamingAssetsPath}/DataBases/{srcDbPath}",
                DataBaseCopyMode.Custom => srcDbPath,
                _ => srcDbPath
            };
            if (!File.Exists(cSrcDbPath))
            {
                throw new FileNotFoundException();
            }

            string destDirPath = Path.GetDirectoryName(cDestDbPath);
            if (!Directory.Exists(destDirPath) && destDirPath != null)
            {
               Directory.CreateDirectory(destDirPath);
            }
            
            File.Copy(cSrcDbPath, cDestDbPath, true);
        }

        public static bool CheckTableExists<T>(SQLiteConnection connection)
        {
            var tableAttr = typeof(T).GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
            var tableName = (tableAttr as TableAttribute)?.Name ?? typeof(T).Name;
            var query = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{tableName}';";
            return connection.ExecuteScalar<int>(query) > 0;
        }
        
        public static bool TryGetTable<T>(this SQLiteConnection connection, out TableQuery<T> table) where T : class,new()
        {
            table = null;
            if (CheckTableExists<T>(connection))
            {
                table = connection.Table<T>();
                return true;
            }
            return false;
        }
    }
}