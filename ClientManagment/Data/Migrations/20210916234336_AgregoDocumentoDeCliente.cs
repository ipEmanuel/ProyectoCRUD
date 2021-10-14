using Microsoft.EntityFrameworkCore.Migrations;

namespace ClientManagment.Data.Migrations
{
    public partial class AgregoDocumentoDeCliente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentNumber",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentNumber",
                table: "Clients");
        }
    }
}
