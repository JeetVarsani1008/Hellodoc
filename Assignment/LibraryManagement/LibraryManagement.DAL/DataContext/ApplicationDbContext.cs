using System;
using System.Collections.Generic;
using LibraryManagement.DAL.DataModels;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.DAL.DataContext;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Borrower> Borrowers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User ID = postgres;Password=1008@jd@2003; Server=localhost;Port=5432;Database=LibraryManagement;Integrated Security=true;Pooling=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Book_pkey");

            entity.Property(e => e.Id).HasIdentityOptions(null, null, null, 100000L, null, null);
        });

        modelBuilder.Entity<Borrower>(entity =>
        {
            entity.HasKey(e => e.BorrowerId).HasName("Borrower_pkey");

            entity.Property(e => e.BorrowerId).HasIdentityOptions(null, null, null, 100000L, null, null);

            entity.HasOne(d => d.BorrowerNavigation).WithOne(p => p.InverseBorrowerNavigation)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Borrower_BorrowerId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
