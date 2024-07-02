using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberRegistrations",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberRegistrations",
                table: "Events");
        }
    }
}
