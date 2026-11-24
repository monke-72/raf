using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Raf.Migrations
{
    /// <inheritdoc />
    public partial class AddIncelemeler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Incelemeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KullaniciId = table.Column<string>(type: "text", nullable: false),
                    IcerikId = table.Column<int>(type: "integer", nullable: false),
                    Puan = table.Column<int>(type: "integer", nullable: false),
                    Metin = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Tarih = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incelemeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incelemeler_Icerikler_IcerikId",
                        column: x => x.IcerikId,
                        principalTable: "Icerikler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Incelemeler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Incelemeler_IcerikId",
                table: "Incelemeler",
                column: "IcerikId");

            migrationBuilder.CreateIndex(
                name: "IX_Incelemeler_KullaniciId_IcerikId",
                table: "Incelemeler",
                columns: new[] { "KullaniciId", "IcerikId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Incelemeler");
        }
    }
}
