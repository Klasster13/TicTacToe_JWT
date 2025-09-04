using Microsoft.EntityFrameworkCore;
using TicTacToe.DataSource.Models;

namespace TicTacToe.DataSource.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserData> Users { get; set; }
    public DbSet<SessionData> Sessions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserData>()
            .HasMany(u => u.AsPlayer1)
            .WithOne(g => g.Player1)
            .HasForeignKey(g => g.Player1Id)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<UserData>()
            .HasMany(u => u.AsPlayer2)
            .WithOne(g => g.Player2)
            .HasForeignKey(g => g.Player2Id)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
