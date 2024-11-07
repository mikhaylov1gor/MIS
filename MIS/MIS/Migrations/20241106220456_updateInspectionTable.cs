using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIS.Migrations
{
    /// <inheritdoc />
    public partial class updateInspectionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "hasChain",
                table: "Inspections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "hasNested",
                table: "Inspections",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hasChain",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "hasNested",
                table: "Inspections");
        }
    }
}
