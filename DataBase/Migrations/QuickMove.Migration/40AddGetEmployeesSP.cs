using Migrator.Framework;
using QuickMove.Migration.Common;

namespace QuickMove.Migration
{
    [Migration(40)]
    public class _40AddGetEmployeesSP : Migrator.Framework.Migration
    {
        private const string UpMigrationFileName = "40AddGetEmployeeSP";
        private const string DownMigrationFileName = "40AddGetEmployeeSP_ROLLBACK";

        public override void Up()
        {
            CommonFunctions.RunScriptQueryWithoutTimeout(
                Database, 
                UpMigrationFileName, 
                "Procedures", 
                GetType());
        }

        public override void Down()
        {
            CommonFunctions.RunScriptQueryWithoutTimeout(
                Database, 
                DownMigrationFileName, 
                "Procedures", 
                GetType());
        }
    }
}