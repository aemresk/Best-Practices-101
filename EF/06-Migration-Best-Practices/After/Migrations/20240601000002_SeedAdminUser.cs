using Microsoft.EntityFrameworkCore.Migrations;

// ✅ Veri migration'ı şema migration'ından ayrı — sorumluluklar karışmıyor
// ✅ Sql() ile veri değişikliği: sürüm kontrolünde kayıtlı, tekrar çalışmaz
// ✅ Down() seed verisini geri alıyor
public partial class SeedAdminUser : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            INSERT INTO Users (Name, Email, CreatedAt, IsAdmin)
            VALUES ('Admin', 'admin@example.com', datetime('now'), 1)
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DELETE FROM Users WHERE Email = 'admin@example.com'
            """);
    }
}
