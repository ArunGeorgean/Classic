using Migrator.Framework;
using System.Data;

namespace QuickMove.Migration._2015._07July
{
    [Migration(30)]
    public class _30AddCompanyAndDepartmentKeys : Migrator.Framework.Migration
    {
        public override void Up()
        {
            Database.AddColumn("Employee", "DepartmentId", DbType.Int32, ColumnProperty.ForeignKey);

            Database.AddForeignKey("FkEmployeeDepartment", "Employee", "DepartmentId", "Department", "DepartmentId");
        }

        public override void Down()
        {
            Database.RemoveColumn("Employee", "DepartmentId");
        }
    }
}
