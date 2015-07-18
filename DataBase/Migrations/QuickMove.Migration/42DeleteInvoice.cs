using Migrator.Framework;
using System.Data;
using QuickMove.Migration.Common;

namespace Migrator2008
{
   
    [Migration(42)]
    public class _42DeleteInvoice : Migrator.Framework.Migration
    {
        public override void Up()
        {
            CommonFunctions.RunScriptQueryWithoutTimeout(
                Database,
                "42_DeleteInvoiceSP",
                "Procedures",
                GetType());
        }

        public override void Down()
        {
            CommonFunctions.RunScriptQueryWithoutTimeout(
                Database,
                "42_DeleteInvoiceSPROLLBACK",
                "Procedures",
                GetType());
        }
    }
}
