using System.Data;
using Migrator.Framework;

namespace QuickMove.Migration._2015._07July
{
    [Migration(10)]
    public class _10EmployeeTable : Migrator.Framework.Migration
    {
        public override void Up()
        {
            Database.AddTable("Employee",
                new Column("EmployeeId", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("Name", DbType.AnsiString, 25)
                );
        }

        public override void Down()
        {
            Database.RemoveTable("Employee");
        }
    }
}
