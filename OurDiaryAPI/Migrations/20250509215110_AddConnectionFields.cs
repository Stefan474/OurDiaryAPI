using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurDiaryAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isConnected",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "partnerEmail",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isConnected",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "partnerEmail",
                table: "Users");
        }
    }
}
