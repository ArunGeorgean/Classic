using Migrator.Framework;
using System.Collections.Generic;
using System.Data;

namespace QuickMove.Migration.Common
{
    public static class DatabaseX
    {
		/// <summary>
		/// Extenstion method to create a database schema
		/// </summary>
		/// <param name="database"></param>
		/// <param name="schemaName">Name of the schema to create</param>
		/// <returns></returns>
		public static int CreateSchema(this ITransformationProvider database, string schemaName)
		{
			return database.ExecuteNonQuery(string.Format("CREATE SCHEMA {0}", schemaName));
		}

		/// <summary>
		/// Extension method to drop a database schema
		/// </summary>
		/// <param name="database"></param>
		/// <param name="schemaName">Name of the schema to drop</param>
		/// <returns></returns>
		public static int DropSchema(this ITransformationProvider database, string schemaName)
		{
			return database.ExecuteNonQuery(string.Format("DROP SCHEMA {0}", schemaName));
		}

        public static bool TableExistsWithSchema(this ITransformationProvider database, string table)
        {
            string tableWithoutBrackets = RemoveBrackets(table);
            string schemaName = GetSchemaName(tableWithoutBrackets);
            string tableName = GetTableName(tableWithoutBrackets);
            using (IDataReader reader 
                = database.ExecuteQuery(string.Format(
                    "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA ='{0}' AND TABLE_NAME='{1}'",
                    schemaName, 
                    tableName)))
            {
                return reader.Read();
            }
        }

        public static void RenameTableWithSchema(this ITransformationProvider database, string oldtable, string newtable)
        {
            if (TableExistsWithSchema(database, oldtable))
                database.ExecuteNonQuery(string.Format("sp_rename '{0}', '{1}'", oldtable, newtable));
        }

        public static void RenameColumnWithSchema(this ITransformationProvider database, string oldtable, string oldcolumn, string newcolumn)
        {
            if (TableExistsWithSchema(database, oldtable))
                database.ExecuteNonQuery(string.Format("sp_rename '{0}.{1}', '{2}', 'COLUMN'", oldtable, oldcolumn, newcolumn));
        }

        public static void RemoveTableWithSchema(this ITransformationProvider database, string table)
        {
            if (TableExistsWithSchema(database, table))
                database.ExecuteNonQuery(string.Format("DROP TABLE {0}", table));
        }

        public static void RemoveColumnWithSchema(this ITransformationProvider database, string table, string column)
        {
            if (!ColumnExistsWithSchema(database, table, column)) return;

            database.DeleteColumnConstraints(table, column);
            database.ExecuteNonQuery(string.Format("ALTER TABLE {0} DROP COLUMN {1} ", table, column));
        }

        public static void ChangeColumnMakeNotNullWithSchema(this ITransformationProvider database, string table, ColumnInfo column)
        {
            if (ColumnExistsWithSchema(database, table, column.Name))
                database.ExecuteNonQuery(string.Format("ALTER TABLE {0} ALTER COLUMN {1} {2} NOT NULL", table, column.Name, column.Type));
        }

        public static void ChangeColumnMakeNullableWithSchema(this ITransformationProvider database, string table, ColumnInfo column)
        {
            if (ColumnExistsWithSchema(database, table, column.Name))
                database.ExecuteNonQuery(string.Format("ALTER TABLE {0} ALTER COLUMN {1} {2} NULL", table, column.Name, column.Type));
        }

        public static void ChangeColumnSizeWithSchema(this ITransformationProvider database, string table, ColumnInfo column)
        {
            if (ColumnExistsWithSchema(database, table, column.Name))
                database.ExecuteNonQuery(string.Format("ALTER TABLE {0} ALTER COLUMN {1} {2}({3})", table, column.Name, column.Type, column.Size));
        }

        public static void ChangeColumnTypeWithSchema(this ITransformationProvider database, string table, ColumnInfo column)
        {
            if (ColumnExistsWithSchema(database, table, column.Name))
                database.ExecuteNonQuery(string.Format("ALTER TABLE {0} ALTER COLUMN {1} {2}", table, column.Name, column.Type));
        }

        public static void RemoveConstraintWithSchema(this Migrator.Framework.ITransformationProvider database, string table, string constraint)
        {
            if (ConstraintExistsWithSchema(database, constraint, table))
                database.ExecuteNonQuery(string.Format("ALTER TABLE {0} DROP CONSTRAINT {1}", table, constraint));
        }

        public static bool ColumnExistsWithSchema(this ITransformationProvider database, string table, string column)
        {
            if (!TableExistsWithSchema(database, table))
                return false;

            using (IDataReader reader =
                database.ExecuteQuery(string.Format("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{0}' AND COLUMN_NAME='{1}'", GetTableName(RemoveBrackets(table)), column)))
            {
                return reader.Read();
            }
        }

        private static bool ConstraintExistsWithSchema(this ITransformationProvider database, string constraint, string table)
        {
            if (!TableExistsWithSchema(database, table))
                return false;

            using (IDataReader reader =
                database.ExecuteQuery(string.Format("SELECT TOP 1 * FROM sysobjects WHERE name = '{0}' AND parent_obj = object_id('{1}')", constraint, table)))
            {
                return reader.Read();
            }
        }

        private static string GetSchemaName(string longTableName)
        {
            var splitTable = SplitTableName(longTableName);
            return splitTable.Length > 1 ? splitTable[0] : "dbo";
        }

        private static string[] SplitTableName(string longTableName)
        {
            return longTableName.Split('.');
        }

        private static string GetTableName(string longTableName)
        {
            var splitTable = SplitTableName(longTableName);
            return splitTable.Length > 1 ? splitTable[1] : longTableName;
        }

        private static string RemoveBrackets(string stringWithBrackets)
        {
            return stringWithBrackets.Replace("[", "").Replace("]", "");
        }


        // Deletes all constraints linked to a column. Sql Server
        // doesn't seems to do this.
        public static void DeleteColumnConstraints(this ITransformationProvider database, string table, string column)
        {
            string sqlContrainte = FindConstraints(table, column);
            List<string> constraints = new List<string>();
            using (IDataReader reader = database.ExecuteQuery(sqlContrainte))
            {
                while (reader.Read())
                {
                    constraints.Add(reader.GetString(0));
                }
            }
            // Can't share the connection so two phase modif
            foreach (string constraint in constraints)
            {
                database.RemoveForeignKey(table, constraint);
            }
        }


        // FIXME: We should look into implementing this with INFORMATION_SCHEMA if possible
        // so that it would be usable by all the SQL Server implementations
        private static string FindConstraints(string table, string column)
        {
            return string.Format(
                "SELECT cont.name FROM SYSOBJECTS cont, SYSCOLUMNS col, SYSCONSTRAINTS cnt  "
                + "WHERE cont.parent_obj = col.id AND cnt.constid = cont.id AND cnt.colid=col.colid "
                + "AND col.name = '{1}' AND col.id = object_id('{0}')",
                table, column);
        }

        public static void RefreshViewsDependantOnTable(this ITransformationProvider database, string tableName)
        {
            var dependantViewsQuery = string.Format(
                "SELECT DISTINCT ss.name + '.' + so.name FROM sys.objects AS so "
                + "INNER JOIN sys.sql_expression_dependencies AS sed ON so.object_id = sed.referencing_id "
                + "JOIN sys.schemas AS ss on ss.schema_id = so.schema_id "
                + "WHERE so.type = 'V' AND sed.referenced_id = OBJECT_ID('{0}')",
                tableName);

            var dependantViewsList = new List<string>();
            using (IDataReader reader = database.ExecuteQuery(dependantViewsQuery))
            {
                while (reader.Read())
                {
                    dependantViewsList.Add(reader.GetString(0));
                }
            }
            dependantViewsList.ForEach(view => database.ExecuteNonQuery(string.Format("EXEC sp_refreshview '{0}'", view)));
        }
    }
}