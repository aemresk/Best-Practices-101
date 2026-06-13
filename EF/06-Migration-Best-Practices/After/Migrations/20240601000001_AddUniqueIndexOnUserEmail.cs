using Microsoft.EntityFrameworkCore.Migrations;

// ✅ Küçük ve odaklı: sadece index ekliyor
// ✅ Down() doğru şekilde rollback yapıyor — production'da geri alınabilir
public partial class AddUniqueIndexOnUserEmail : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Users_Email",
            table: "Users");
    }
}
