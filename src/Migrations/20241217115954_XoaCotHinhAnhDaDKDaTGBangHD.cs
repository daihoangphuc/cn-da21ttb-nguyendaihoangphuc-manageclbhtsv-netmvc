using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manage_CLB_HTSV.Migrations
{
    public partial class XoaCotHinhAnhDaDKDaTGBangHD : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaDangKi",
                table: "HoatDong");

            migrationBuilder.DropColumn(
                name: "DaThamGia",
                table: "HoatDong");

            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "HoatDong");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DaDangKi",
                table: "HoatDong",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DaThamGia",
                table: "HoatDong",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "HoatDong",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
