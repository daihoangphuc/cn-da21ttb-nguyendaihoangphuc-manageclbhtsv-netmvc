using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manage_CLB_HTSV.Migrations
{
    public partial class xocotaminhchunghoatdong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinhChung",
                table: "HoatDong");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MinhChung",
                table: "HoatDong",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
