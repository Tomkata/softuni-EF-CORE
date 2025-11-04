using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cadastre.Migrations
{
    public partial class DbSetsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Property_District_DistrictId",
                table: "Property");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyCitizen_Citizen_CitizenId",
                table: "PropertyCitizen");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyCitizen_Property_PropertyId",
                table: "PropertyCitizen");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertyCitizen",
                table: "PropertyCitizen");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Property",
                table: "Property");

            migrationBuilder.DropPrimaryKey(
                name: "PK_District",
                table: "District");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Citizen",
                table: "Citizen");

            migrationBuilder.RenameTable(
                name: "PropertyCitizen",
                newName: "PropertiesCitizens");

            migrationBuilder.RenameTable(
                name: "Property",
                newName: "Properties");

            migrationBuilder.RenameTable(
                name: "District",
                newName: "Districts");

            migrationBuilder.RenameTable(
                name: "Citizen",
                newName: "Citizens");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyCitizen_PropertyId",
                table: "PropertiesCitizens",
                newName: "IX_PropertiesCitizens_PropertyId");

            migrationBuilder.RenameIndex(
                name: "IX_Property_DistrictId",
                table: "Properties",
                newName: "IX_Properties_DistrictId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertiesCitizens",
                table: "PropertiesCitizens",
                columns: new[] { "CitizenId", "PropertyId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Properties",
                table: "Properties",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Districts",
                table: "Districts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Citizens",
                table: "Citizens",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Districts_DistrictId",
                table: "Properties",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertiesCitizens_Citizens_CitizenId",
                table: "PropertiesCitizens",
                column: "CitizenId",
                principalTable: "Citizens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertiesCitizens_Properties_PropertyId",
                table: "PropertiesCitizens",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Districts_DistrictId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertiesCitizens_Citizens_CitizenId",
                table: "PropertiesCitizens");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertiesCitizens_Properties_PropertyId",
                table: "PropertiesCitizens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertiesCitizens",
                table: "PropertiesCitizens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Properties",
                table: "Properties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Districts",
                table: "Districts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Citizens",
                table: "Citizens");

            migrationBuilder.RenameTable(
                name: "PropertiesCitizens",
                newName: "PropertyCitizen");

            migrationBuilder.RenameTable(
                name: "Properties",
                newName: "Property");

            migrationBuilder.RenameTable(
                name: "Districts",
                newName: "District");

            migrationBuilder.RenameTable(
                name: "Citizens",
                newName: "Citizen");

            migrationBuilder.RenameIndex(
                name: "IX_PropertiesCitizens_PropertyId",
                table: "PropertyCitizen",
                newName: "IX_PropertyCitizen_PropertyId");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_DistrictId",
                table: "Property",
                newName: "IX_Property_DistrictId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertyCitizen",
                table: "PropertyCitizen",
                columns: new[] { "CitizenId", "PropertyId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Property",
                table: "Property",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_District",
                table: "District",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Citizen",
                table: "Citizen",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Property_District_DistrictId",
                table: "Property",
                column: "DistrictId",
                principalTable: "District",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyCitizen_Citizen_CitizenId",
                table: "PropertyCitizen",
                column: "CitizenId",
                principalTable: "Citizen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyCitizen_Property_PropertyId",
                table: "PropertyCitizen",
                column: "PropertyId",
                principalTable: "Property",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
