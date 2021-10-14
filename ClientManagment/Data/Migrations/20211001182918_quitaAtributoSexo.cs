using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClientManagment.Data.Migrations
{
    public partial class quitaAtributoSexo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sex",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Usucre",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "fchcre",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "fchmod",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "flgeli",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "usumod",
                table: "Clients");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sex",
                table: "Clients",
                type: "varchar(1)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Usucre",
                table: "Clients",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "fchcre",
                table: "Clients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "fchmod",
                table: "Clients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "flgeli",
                table: "Clients",
                type: "varchar(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "usumod",
                table: "Clients",
                type: "varchar(100)",
                nullable: true);
        }
    }
}
