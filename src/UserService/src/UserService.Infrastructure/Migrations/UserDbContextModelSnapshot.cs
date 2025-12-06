using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UserService.Infrastructure.Persistence;

#nullable disable

namespace UserService.Infrastructure.Migrations
{
    [DbContext(typeof(UserDbContext))]
    partial class UserDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("UserService.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("name");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("passwordhash");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("UserService.Domain.Entities.UserFavoriteCurrency", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("CurrencyCode")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("character varying(8)")
                        .HasColumnName("currencycode");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("userid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("user_favorites", (string)null);
                });

            modelBuilder.Entity("UserService.Domain.Entities.UserFavoriteCurrency", b =>
                {
                    b.HasOne("UserService.Domain.Entities.User", null)
                        .WithMany("Favorites")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UserService.Domain.Entities.User", b =>
                {
                    b.Navigation("Favorites");
                });
#pragma warning restore 612, 618
        }
    }
}
