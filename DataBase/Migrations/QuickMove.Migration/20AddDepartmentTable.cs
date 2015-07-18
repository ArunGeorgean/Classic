using Migrator.Framework;
using System.Data;

namespace QuickMove.Migration._2015._07July
{
    [Migration(20)]
    public class _20AddDepartmentTable : Migrator.Framework.Migration
    {
        public override void Up()
        {
            Database.AddTable("Department",
                new Column("DepartmentId", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("Name", DbType.AnsiString, 25)
                );
        }

        public override void Down()
        {
            Database.RemoveTable("Department");
        }
    }
}
