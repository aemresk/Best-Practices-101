using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

// Bu dosya EF Core araçları tarafından otomatik üretilir — elle düzenlenmez.
// dotnet ef migrations add çalıştırıldığında güncellenir.
[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

        modelBuilder.Entity("User", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER");

            b.Property<DateTime>("CreatedAt")
                .HasColumnType("TEXT");

            b.Property<string>("Email")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("TEXT");

            b.Property<bool>("IsAdmin")
                .HasColumnType("INTEGER");

            b.Property<string>("Name")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.HasIndex("Email").IsUnique();

            b.ToTable("Users");
        });
#pragma warning restore 612, 618
    }
}
