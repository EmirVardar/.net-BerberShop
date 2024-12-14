using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberShop.Migrations
{
    /// <inheritdoc />
    public partial class calisansaat4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalismaSaatleri");

            migrationBuilder.AddColumn<int>(
                name: "HizmetId1",
                table: "CalisanHizmetleri",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CalisanCalismaSaatleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalisanId = table.Column<int>(type: "int", nullable: false),
                    Gun = table.Column<int>(type: "int", nullable: false),
                    BaslangicSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    BitisSaati = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalisanCalismaSaatleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalisanCalismaSaatleri_Calisanlar_CalisanId",
                        column: x => x.CalisanId,
                        principalTable: "Calisanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalisanHizmetleri_HizmetId1",
                table: "CalisanHizmetleri",
                column: "HizmetId1");

            migrationBuilder.CreateIndex(
                name: "IX_CalisanCalismaSaatleri_CalisanId",
                table: "CalisanCalismaSaatleri",
                column: "CalisanId");

            migrationBuilder.AddForeignKey(
                name: "FK_CalisanHizmetleri_Hizmetler_HizmetId1",
                table: "CalisanHizmetleri",
                column: "HizmetId1",
                principalTable: "Hizmetler",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalisanHizmetleri_Hizmetler_HizmetId1",
                table: "CalisanHizmetleri");

            migrationBuilder.DropTable(
                name: "CalisanCalismaSaatleri");

            migrationBuilder.DropIndex(
                name: "IX_CalisanHizmetleri_HizmetId1",
                table: "CalisanHizmetleri");

            migrationBuilder.DropColumn(
                name: "HizmetId1",
                table: "CalisanHizmetleri");

            migrationBuilder.CreateTable(
                name: "CalismaSaatleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalisanId = table.Column<int>(type: "int", nullable: false),
                    BaslangicSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    BitisSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    Gun = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalismaSaatleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalismaSaatleri_Calisanlar_CalisanId",
                        column: x => x.CalisanId,
                        principalTable: "Calisanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalismaSaatleri_CalisanId",
                table: "CalismaSaatleri",
                column: "CalisanId");
        }
    }
}
