using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cadastre.Migrations
{
    public partial class AdditionalUpdatedAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FisrtName",
                table: "Citizens",
                newName: "FirstName");

            migrationBuilder.AlterColumn<int>(
                name: "Area",
                table: "Properties",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "Properties",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Citizens",
                newName: "FisrtName");

            migrationBuilder.AlterColumn<long>(
                name: "Area",
                table: "Properties",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
