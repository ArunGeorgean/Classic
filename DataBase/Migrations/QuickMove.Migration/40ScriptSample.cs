using Migrator.Framework;
using System.Data;
using QuickMove.Migration.Common;

namespace Migrator2008
{
    [Migration(40)]
    public class _40ScriptSample : Migrator.Framework.Migration
    {
        public override void Up()
        {
            CommonFunctions.RunScriptQueryWithoutTimeout(
                Database,
                "40AddGetEmployeeSP",
                "Procedures",
                GetType());
        }

        public override void Down()
        {
            CommonFunctions.RunScriptQueryWithoutTimeout(
                Database,
                "40AddGetEmployeeSP_ROLLBACK",
                "Procedures",
                GetType());
        }
    }
}
