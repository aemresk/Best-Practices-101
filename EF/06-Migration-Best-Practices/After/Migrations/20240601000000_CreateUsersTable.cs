using Microsoft.EntityFrameworkCore.Migrations;

// ✅ İsim açıklayıcı: ne yapıldığı class adından anlaşılıyor
// ✅ Tek sorumluluk: sadece Users tablosunu oluşturuyor
public partial class CreateUsersTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id        = table.Column<int>(nullable: false).Annotation("Sqlite:Autoincrement", true),
                Name      = table.Column<string>(maxLength: 200, nullable: false),
                Email     = table.Column<string>(maxLength: 200, nullable: false),
                CreatedAt = table.Column<DateTime>(nullable: false),
                IsAdmin   = table.Column<bool>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Users");
    }
}
