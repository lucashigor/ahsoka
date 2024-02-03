using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ahsoka.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePipelineStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FinalDate",
                schema: "ahsoka",
                table: "Configuration",
                newName: "ExpireDate");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "ahsoka",
                table: "Configuration",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "ahsoka",
                table: "Configuration");

            migrationBuilder.RenameColumn(
                name: "ExpireDate",
                schema: "ahsoka",
                table: "Configuration",
                newName: "FinalDate");
        }
    }
}
