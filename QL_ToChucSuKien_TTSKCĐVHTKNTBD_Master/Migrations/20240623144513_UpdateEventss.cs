using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageBase64",
                table: "Events",
                newName: "ImgUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImgUrl",
                table: "Events",
                newName: "ImageBase64");
        }
    }
}
