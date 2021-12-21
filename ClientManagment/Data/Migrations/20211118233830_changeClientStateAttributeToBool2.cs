using Microsoft.EntityFrameworkCore.Migrations;

namespace ClientManagment.Data.Migrations
{
    public partial class changeClientStateAttributeToBool2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Clients");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Clients",
                type: "varchar(1)",
                nullable: false,
                defaultValue: "");
        }
    }
}
