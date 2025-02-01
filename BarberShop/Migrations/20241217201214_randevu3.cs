using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberShop.Migrations
{
    /// <inheritdoc />
    public partial class randevu3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Randevular_AspNetUsers_KullaniciId",
                table: "Randevular");

            migrationBuilder.DropIndex(
                name: "IX_Randevular_KullaniciId",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "KullaniciId",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "OnaylandiMi",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "RandevuSaati",
                table: "Randevular");

            migrationBuilder.AddColumn<int>(
                name: "Durum",
                table: "Randevular",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HizmetId",
                table: "Randevular",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Randevular",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_HizmetId",
                table: "Randevular",
                column: "HizmetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Randevular_Hizmetler_HizmetId",
                table: "Randevular",
                column: "HizmetId",
                principalTable: "Hizmetler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Randevular_Hizmetler_HizmetId",
                table: "Randevular");

            migrationBuilder.DropIndex(
                name: "IX_Randevular_HizmetId",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "Durum",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "HizmetId",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Randevular");

            migrationBuilder.AddColumn<string>(
                name: "KullaniciId",
                table: "Randevular",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "OnaylandiMi",
                table: "Randevular",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "RandevuSaati",
                table: "Randevular",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_KullaniciId",
                table: "Randevular",
                column: "KullaniciId");

            migrationBuilder.AddForeignKey(
                name: "FK_Randevular_AspNetUsers_KullaniciId",
                table: "Randevular",
                column: "KullaniciId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
