using Migrator.Framework;
using System.Data;
using QuickMove.Migration.Common;
namespace Migrator2008
{    
    [Migration(41)]
    public class _41InsetData : Migrator.Framework.Migration
    {
        public override void Up()
        {
            CommonFunctions.RunScriptQueryWithoutTimeout(
                Database,
                "41InsertEmpSP",
                "Procedures",
                GetType());
        }

        public override void Down()
        {
            CommonFunctions.RunScriptQueryWithoutTimeout(
                Database,
                "41InsertEmpSP_ROLLBACK",
                "Procedures",
                GetType());
        }
    }
}
