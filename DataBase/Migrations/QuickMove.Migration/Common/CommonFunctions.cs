using Migrator.Framework;
using System;
using System.Data;
using System.IO;

namespace QuickMove.Migration.Common
{
    public static class CommonFunctions
    {
        private const string RenameSpSql =
            @"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{0}') 
                    AND type in (N'P', N'PC')) 
                    AND NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{1}') 
                        AND type in (N'P', N'PC')) 
                EXEC sp_rename '{0}', '{1}'";

        private const string DropSprocSql =
            @"
                IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{1}.{0}') 
                    AND type in (N'P', N'PC'))
                DROP PROCEDURE {1}.{0}";

        private const string DropViewSql =
            @"
                IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{1}.{0}') 
                    AND type in (N'V'))
                DROP VIEW {1}.{0}";

        private const string DropTriggerSql =
            @"
                IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{0}') 
                    AND type in (N'TR'))
                DROP TRIGGER {1}.{0}";

        private const string DropFunctionSql =
            @"
                IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{1}.{0}') 
                    AND type in (N'FN', N'IF', N'TF'))
                DROP FUNCTION {1}.{0}";


        /// <summary>
        /// Run Stored Procedure Query
        /// </summary>
        /// <param name="p_provider">Database provider</param>
        /// <param name="p_triggerName">Procedure name</param>
        /// <param name="p_resourceName">Stored procedure filename</param>
        /// <returns></returns>
        public static void RunTrigger(ITransformationProvider p_provider, string p_triggerName,Type typeHoldingScript, string p_resourceName = null, string p_schemaName = "dbo")
        {
            RunScript(p_provider, ScriptType.Triggers, p_triggerName, typeHoldingScript, p_resourceName, p_schemaName);
        }

        /// <summary>
        /// Run Stored Procedure Query
        /// </summary>
        /// <param name="p_provider">Database provider</param>
        /// <param name="p_procedureName">Procedure name</param>
        /// <param name="p_resourceName">Stored procedure filename</param>
        /// <returns></returns>
        public static void RunStoredProcedure(ITransformationProvider p_provider, string p_procedureName,Type typeHoldingScript, string p_resourceName = null, string p_schemaName = "dbo")
        {
            RunScript(p_provider, ScriptType.StoredProcedures, p_procedureName, typeHoldingScript, p_resourceName, p_schemaName);
        }

        /// <summary>
        /// Run View Query
        /// </summary>
        /// <param name="p_provider">Database provider</param>
        /// <param name="p_procedureName">Procedure name</param>
        /// <param name="p_resourceName">Stored procedure filename</param>
        /// <returns></returns>
        public static void RunView(ITransformationProvider p_provider, string p_viewName,Type typeHoldingScript, string p_resourceName = null, string p_schemaName = "dbo")
        {
            RunScript(p_provider, ScriptType.Views, p_viewName, typeHoldingScript, p_resourceName, p_schemaName);
        }

        /// <summary>
        /// Run Function Query
        /// </summary>
        /// <param name="p_provider">Database provider</param>
        /// <param name="p_functionName">Function name</param>
        /// <param name="typeHoldingScript">Assembly name</param>
        /// <param name="p_resourceName">Function's filename</param>
        /// <returns></returns>
        public static void RunFunction(ITransformationProvider p_provider, string p_functionName,Type typeHoldingScript, string p_resourceName = null, string p_schemaName = "dbo")
        {
            RunScript(p_provider, ScriptType.Functions, p_functionName, typeHoldingScript, p_resourceName, p_schemaName);
        }

        public static void RunScriptQueryWithoutTimeout(ITransformationProvider p_provider, string p_resourceName, string p_folder, Type typeHoldingScript)
        {
            // Load Script
            var assembly = typeHoldingScript.Assembly;
            string resourceFullName = string.Format("{0}.Scripts.{1}.{2}.sql", assembly.GetName().Name, p_folder, p_resourceName);
            Console.WriteLine("Resource file name = " + resourceFullName);
            var stream = assembly.GetManifestResourceStream(resourceFullName);
            if (stream == null)
                throw new InvalidOperationException("Cannot load resource");
            var textStreamReader = new StreamReader(stream);


            // Execute script
            ExecuteNonQueryNoTimeout(p_provider, textStreamReader.ReadToEnd());
            textStreamReader.Close();
        }
        public static void ExecuteNonQueryNoTimeout(ITransformationProvider p_provider, string p_sqlStatement)
        {
            var command = p_provider.GetCommand();
            command.CommandType = CommandType.Text;
            // 1-24-13 AMB:  Changed from 600 (10 minutes) to 1800 (30 minutes)
            command.CommandTimeout = 1800;
            command.CommandText = p_sqlStatement;
            Console.WriteLine("Executing: {0}", command.CommandText);
            command.ExecuteNonQuery();
        }

        public static IDataReader ExecuteQuery(ITransformationProvider p_provider, string p_sqlStatement)
        {
            return p_provider.ExecuteQuery(p_sqlStatement);
        }

        /// <summary>
        /// Run Script
        /// </summary>
        /// <param name="p_provider">Database provider</param>
        /// <param name="p_scriptType">Script Type</param>
        /// <param name="p_scriptName">Name of Script</param>
        /// <param name="p_fileName">FileName of Script</param>
        /// <param name="p_schemaName">Schema Name - Default as dbo</param>
        /// <returns></returns>
        private static void RunScript(ITransformationProvider p_provider, ScriptType p_scriptType, string p_scriptName, Type typeHoldingScript,string p_fileName = null, string p_schemaName = "dbo")
        {
            if (string.IsNullOrEmpty(p_fileName))
                p_fileName = string.Format("{0}.sql", p_scriptName);

            // Drop proc/views/trigger if already exists
            p_provider.ExecuteNonQuery(string.Format(GetDropStatement(p_scriptType), p_scriptName, p_schemaName));
        

            // Load Script
            var assembly = typeHoldingScript.Assembly;
            string resourceFullName = string.Format("{0}.Scripts.{1}.{2}.sql", assembly.GetName().Name, p_scriptType, p_fileName);
            Console.WriteLine("Resource file name = " + resourceFullName);
            

            Console.WriteLine(resourceFullName);
            var stream = assembly.GetManifestResourceStream(resourceFullName);
            if (stream == null)
                throw new InvalidOperationException(string.Format("Cannot load resource: {0}", resourceFullName));
            var textStreamReader = new StreamReader(stream);

            // Execute procedure
            // Execute procedure
            // 4-26-13 AMB:  Changing this so we can set the timeout value
            //p_provider.ExecuteNonQuery(textStreamReader.ReadToEnd());
            var command = p_provider.GetCommand();
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 1800;
            command.CommandText = textStreamReader.ReadToEnd();
            command.ExecuteNonQuery();

            Console.WriteLine(command.CommandText);
        }

        /// <summary>
        /// Drop Database object
        /// </summary>
        /// <param name="p_provider">Database provider</param>
        /// <param name="p_scriptType">Script type</param>
        /// <param name="p_objectName">Name of Database Object</param>
        /// <param name="p_schemaName">Name of Schema</param>
        public static void DropObject(ITransformationProvider p_provider, ScriptType p_scriptType, string p_objectName, string p_schemaName = "dbo")
        {
            p_provider.ExecuteNonQuery(string.Format(GetDropStatement(p_scriptType), p_objectName, p_schemaName));
        }

        /// <summary>
        /// Returns drop statement for Sp/view/trigger/function
        /// </summary>
        /// <param name="p_scriptType">ScriptType Enum value</param>
        /// <returns>String</returns>
        private static string GetDropStatement(ScriptType p_scriptType)
        {
            switch (p_scriptType)
            {
                case ScriptType.StoredProcedures:
                    return DropSprocSql;
                case ScriptType.Views:
                    return DropViewSql;
                case ScriptType.Triggers:
                    return DropTriggerSql;
                case ScriptType.Functions:
                    return DropFunctionSql;
            }

            throw new ArgumentException("Invalid ScriptType");
        }

        public enum ScriptType
        {
            StoredProcedures,
            Views,
            Scripts,
            Triggers,
            Functions
        }
    }
}
