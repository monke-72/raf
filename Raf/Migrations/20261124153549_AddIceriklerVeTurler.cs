using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Raf.Migrations
{
    /// <inheritdoc />
    public partial class AddIceriklerVeTurler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Icerikler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Baslik = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MedyaTuru = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CikisYili = table.Column<int>(type: "integer", nullable: true),
                    Aciklama = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    GorselUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    HariciKaynak = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    HariciId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Icerikler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Turler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IcerikTurleri",
                columns: table => new
                {
                    IcerikId = table.Column<int>(type: "integer", nullable: false),
                    TurId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IcerikTurleri", x => new { x.IcerikId, x.TurId });
                    table.ForeignKey(
                        name: "FK_IcerikTurleri_Icerikler_IcerikId",
                        column: x => x.IcerikId,
                        principalTable: "Icerikler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IcerikTurleri_Turler_TurId",
                        column: x => x.TurId,
                        principalTable: "Turler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Icerikler",
                columns: new[] { "Id", "Aciklama", "Baslik", "CikisYili", "GorselUrl", "HariciId", "HariciKaynak", "MedyaTuru" },
                values: new object[,]
                {
                    { 1, "Haksız yere ömür boyu hapse mahkûm edilen bir bankacının, yıllar süren hapis hayatı boyunca umudunu ve dostluğunu korumasını anlatan film.", "Esaretin Bedeli", 1994, null, null, null, "Film" },
                    { 2, "İnsanların rüyalarına girerek fikir çalan bir ekibin, bu kez bir fikri birinin zihnine yerleştirmeye çalıştığı bilim kurgu filmi.", "Başlangıç", 2010, null, null, null, "Film" },
                    { 3, "Ruhlar dünyasına sürüklenen küçük bir kızın ailesini kurtarmak için verdiği mücadeleyi anlatan animasyon filmi.", "Ruhların Kaçışı", 2001, null, null, null, "Film" },
                    { 4, "Canavar avcısı Geralt'ın, evlatlığı Ciri'yi aradığı açık dünya rol yapma oyunu.", "The Witcher 3: Wild Hunt", 2015, null, null, null, "Oyun" },
                    { 5, "Böceklerin yaşadığı terk edilmiş bir yeraltı krallığında geçen, keşif odaklı iki boyutlu aksiyon platform oyunu.", "Hollow Knight", 2017, null, null, null, "Oyun" },
                    { 6, "Dedesinden kalan çiftliği yeniden canlandıran bir karakterin hikâyesini anlatan çiftlik ve yaşam simülasyonu oyunu.", "Stardew Valley", 2016, null, null, null, "Oyun" },
                    { 7, "Zaman, para ve akıl sağlığı gibi temaları kesintisiz bir bütün olarak işleyen progresif rock albümü.", "Pink Floyd – The Dark Side of the Moon", 1973, null, null, null, "Album" },
                    { 8, "Teknoloji ve modern yaşamın yabancılaştırıcı etkisini işleyen alternatif rock albümü.", "Radiohead – OK Computer", 1997, null, null, null, "Album" },
                    { 9, "Sanatçının en çok bilinen şarkılarından bazılarını içeren pop albümü.", "Sezen Aksu – Gülümse", 1991, null, null, null, "Album" }
                });

            migrationBuilder.InsertData(
                table: "Turler",
                columns: new[] { "Id", "Ad" },
                values: new object[,]
                {
                    { 1, "Dram" },
                    { 2, "Bilim Kurgu" },
                    { 3, "Animasyon" },
                    { 4, "Fantastik" },
                    { 5, "Aksiyon" },
                    { 6, "Macera" },
                    { 7, "Rol Yapma (RPG)" },
                    { 8, "Platform" },
                    { 9, "Simülasyon" },
                    { 10, "Rock" },
                    { 11, "Pop" },
                    { 12, "Alternatif" }
                });

            migrationBuilder.InsertData(
                table: "IcerikTurleri",
                columns: new[] { "IcerikId", "TurId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 2, 5 },
                    { 3, 3 },
                    { 3, 4 },
                    { 4, 6 },
                    { 4, 7 },
                    { 5, 5 },
                    { 5, 8 },
                    { 6, 9 },
                    { 7, 10 },
                    { 8, 10 },
                    { 8, 12 },
                    { 9, 11 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Icerikler_HariciKaynak_HariciId",
                table: "Icerikler",
                columns: new[] { "HariciKaynak", "HariciId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IcerikTurleri_TurId",
                table: "IcerikTurleri",
                column: "TurId");

            migrationBuilder.CreateIndex(
                name: "IX_Turler_Ad",
                table: "Turler",
                column: "Ad",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IcerikTurleri");

            migrationBuilder.DropTable(
                name: "Icerikler");

            migrationBuilder.DropTable(
                name: "Turler");
        }
    }
}
