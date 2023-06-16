using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Inventory.Models.DB
{
    public partial class InventoryContext : DbContext
    {
        public InventoryContext()
        {
        }

        public InventoryContext(DbContextOptions<InventoryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Company> Companies { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductCompany> ProductCompanies { get; set; } = null!;
        public virtual DbSet<ProductTag> ProductTags { get; set; } = null!;
        public virtual DbSet<ShoppingList> ShoppingLists { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;
        public virtual DbSet<TypeUnit> TypeUnits { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost; Database=Inventory; Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("Company");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name")
                    .IsFixedLength();

                entity.Property(e => e.Priority).HasColumnName("priority");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CurrentAmount).HasColumnName("current_amount");

                entity.Property(e => e.DefaultCompany).HasColumnName("default_company");

                entity.Property(e => e.Expiration)
                    .HasColumnType("date")
                    .HasColumnName("expiration");

                entity.Property(e => e.Image).HasColumnName("image");

                entity.Property(e => e.MinAmount).HasColumnName("min_amount");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name")
                    .IsFixedLength();

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.ProductAmount)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("product_amount");

                entity.Property(e => e.Required).HasColumnName("required");

                entity.Property(e => e.TypeAmount).HasColumnName("type_amount");

                entity.Property(e => e.TypePrice)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("type_price");

                entity.Property(e => e.UnitAmount).HasColumnName("unit_amount");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("unit_price");

                entity.HasOne(d => d.DefaultCompanyNavigation)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.DefaultCompany)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Company");

                entity.HasOne(d => d.TypeAmountNavigation)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.TypeAmount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Type_Unit");
            });

            modelBuilder.Entity<ProductCompany>(entity =>
            {
                entity.ToTable("Product_Company");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdCompany).HasColumnName("id_company");

                entity.Property(e => e.IdProduct).HasColumnName("id_product");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("price");

                entity.HasOne(d => d.IdCompanyNavigation)
                    .WithMany(p => p.ProductCompanies)
                    .HasForeignKey(d => d.IdCompany)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Company_Company");

                entity.HasOne(d => d.IdProductNavigation)
                    .WithMany(p => p.ProductCompanies)
                    .HasForeignKey(d => d.IdProduct)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Company_Product");
            });

            modelBuilder.Entity<ProductTag>(entity =>
            {
                entity.ToTable("Product_Tag");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdProduct).HasColumnName("id_product");

                entity.Property(e => e.IdTag).HasColumnName("id_tag");

                entity.HasOne(d => d.IdProductNavigation)
                    .WithMany(p => p.ProductTags)
                    .HasForeignKey(d => d.IdProduct)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Tag_Product");

                entity.HasOne(d => d.IdTagNavigation)
                    .WithMany(p => p.ProductTags)
                    .HasForeignKey(d => d.IdTag)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Tag_Tag");
            });

            modelBuilder.Entity<ShoppingList>(entity =>
            {
                entity.ToTable("Shopping_List");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.IdCompany).HasColumnName("id_company");

                entity.Property(e => e.IdProduct).HasColumnName("id_product");

                entity.HasOne(d => d.IdCompanyNavigation)
                    .WithMany(p => p.ShoppingLists)
                    .HasForeignKey(d => d.IdCompany)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Shopping_List_Company");

                entity.HasOne(d => d.IdProductNavigation)
                    .WithMany(p => p.ShoppingLists)
                    .HasForeignKey(d => d.IdProduct)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Shopping_List_Product");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("Tag");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name")
                    .IsFixedLength();

                entity.Property(e => e.Priority).HasColumnName("priority");
            });

            modelBuilder.Entity<TypeUnit>(entity =>
            {
                entity.ToTable("Type_Unit");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name")
                    .IsFixedLength();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Mail)
                    .HasMaxLength(50)
                    .HasColumnName("mail")
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name")
                    .IsFixedLength();

                entity.Property(e => e.Password)
                    .HasMaxLength(512)
                    .HasColumnName("password")
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
