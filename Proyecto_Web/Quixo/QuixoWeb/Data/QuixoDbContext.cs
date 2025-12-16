using Microsoft.EntityFrameworkCore;
using QuixoWeb.Models;

namespace QuixoWeb.Data
{
    public class QuixoDbContext : DbContext
    {
        public QuixoDbContext(DbContextOptions<QuixoDbContext> options)
            : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Move> Moves { get; set; }
        public DbSet<BoardState> BoardStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    //
    // TEAM → PLAYER1 / PLAYER2
    //
    modelBuilder.Entity<Team>()
        .HasOne(t => t.Player1)
        .WithMany()
        .HasForeignKey(t => t.Player1Id)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Team>()
        .HasOne(t => t.Player2)
        .WithMany()
        .HasForeignKey(t => t.Player2Id)
        .OnDelete(DeleteBehavior.Restrict);

    //
    // GAME → WINNER PLAYER / WINNER TEAM
    //
    modelBuilder.Entity<Game>()
        .HasOne(g => g.WinnerPlayer)
        .WithMany()
        .HasForeignKey(g => g.WinnerPlayerId)
        .OnDelete(DeleteBehavior.SetNull);

    modelBuilder.Entity<Game>()
        .HasOne(g => g.WinnerTeam)
        .WithMany()
        .HasForeignKey(g => g.WinnerTeamId)
        .OnDelete(DeleteBehavior.SetNull);

    //
    // GAME → MOVES  (1:N)
    //
    modelBuilder.Entity<Game>()
        .HasMany(g => g.Moves)
        .WithOne(m => m.Game)
        .HasForeignKey(m => m.GameId)
        .OnDelete(DeleteBehavior.Cascade);

    //
    // MOVE → PLAYER (N:1)
    //
    modelBuilder.Entity<Move>()
        .HasOne(m => m.Player)
        .WithMany(p => p.Moves)
        .HasForeignKey(m => m.PlayerId)
        .OnDelete(DeleteBehavior.Restrict);

    //
    // GAME → BOARDSTATES (1:N)  ***RELACIÓN CORRECTA***
    //
    modelBuilder.Entity<Game>()
        .HasMany(g => g.BoardStates)
        .WithOne(bs => bs.Game)
        .HasForeignKey(bs => bs.GameId_FK)
        .OnDelete(DeleteBehavior.Cascade);

    //
    // BOARDSTATE → MOVE (1:1)
    //
    modelBuilder.Entity<BoardState>()
        .HasOne(bs => bs.Move)
        .WithOne()
        .HasForeignKey<BoardState>(bs => bs.MoveId)
        .OnDelete(DeleteBehavior.Cascade);
}

    }
}
