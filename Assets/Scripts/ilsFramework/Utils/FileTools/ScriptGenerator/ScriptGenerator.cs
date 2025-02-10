using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ilsFramework
{
    /// <summary>
    /// 脚本生成器，用于快速cs文件，减少每次都写一坨代码
    /// 总之就是一种偷懒
    /// 主要弄那些要从配置文件（SO）中转换成需要的东西
    /// </summary>
    public class ScriptGenerator
    {
        private string nameSpace;
        
        private List<IStatementGenerator> statementGenerators;

        private ScriptGenerator()
        {
            
        }

        public ScriptGenerator(List<IStatementGenerator> statementGenerators,string nameSpace = null)
        {
            this.statementGenerators = statementGenerators;
            this.nameSpace = nameSpace;
        }
        public ScriptGenerator(string nameSpace = null,params IStatementGenerator[] statementGenerators)
        {
            this.statementGenerators = new List<IStatementGenerator>(statementGenerators);
            this.nameSpace = nameSpace;
        }

        public void Append(IStatementGenerator statementGenerator)
        {
            statementGenerators.Add(statementGenerator);
        }
        

        /// <summary>
        /// 生成脚本
        /// </summary>
        /// <param name="scriptPath">指定位置</param>
        public void GenerateScript(string fileName,string scriptPath = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("//该文件是自动生成的，请勿手动修改！\n");
            if (nameSpace != null)
            {
                sb.Append($"namespace {nameSpace}\n");
                sb.Append("{\n");
            }

            string prefix = nameSpace != null ?"\t" : string.Empty;
            foreach (IStatementGenerator statementGenerator in statementGenerators)
            {
                statementGenerator.Generate(sb,prefix);
                sb.Append("\n");
            }

            if (nameSpace != null)
            {
                sb.Append("}");
            }

            string path;
            if (scriptPath == null)
            {
                StackTrace st  = new StackTrace(1,true);
                path=   Path.GetDirectoryName(st.GetFrame(0).GetFileName()) + @$"\{fileName}.cs";
            }
            else
                path = scriptPath + @$"\{fileName}.cs";

            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            File.WriteAllText(path, sb.ToString());
        }
    }
}