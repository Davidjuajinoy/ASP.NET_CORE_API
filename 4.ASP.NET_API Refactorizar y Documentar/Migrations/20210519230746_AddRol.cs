using Microsoft.EntityFrameworkCore.Migrations;

namespace ASP.NET_API.Migrations
{
    public partial class AddRol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Insert into AspNetRoles (Id, [name], [NormalizedName])
            values ('af207a62-e683-48d6-9023-6acc163f7dd4', 'Admin', 'Admin')
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"delete AspNetRoles
             where id = 'af207a62-e683-48d6-9023-6acc163f7dd4'");
        }
    }
}
