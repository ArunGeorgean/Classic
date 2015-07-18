using Migrator.Framework;
using System.Data;

namespace QuickMove.Migration._2015._06June
{
    [Migration(1)]
    public class _1AddCompanyTable : Migrator.Framework.Migration
    {
        public override void Up()
        {
            Database.AddTable("Company",
                new Column("CompanyId", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("Name", DbType.AnsiString, 25)
                );
        }

        public override void Down()
        {
            Database.RemoveTable("Company");
        }
    }


    [Migration(22)]
    public class _1AddSalesTable : Migrator.Framework.Migration
    {
        public override void Up()
        {
            Database.AddTable("Sales",
                new Column("ID", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("Sal", DbType.AnsiString, 25)
                );
        }

        public override void Down()
        {
            Database.RemoveTable("Company");
        }
    }
    [Migration(25)]
    public class _1AddSalaryTable : Migrator.Framework.Migration
    {
        public override void Up()
        {
            Database.AddTable("Salary",
                new Column("ID", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("Sal", DbType.AnsiString, 25)
                );
        }

        public override void Down()
        {
            Database.RemoveTable("Salary");
        }
    }
}