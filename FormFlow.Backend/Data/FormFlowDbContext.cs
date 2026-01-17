using FormFlow.Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Backend.Data;

public class FormFlowDbContext : IdentityDbContext<FormFlowUser> {
    public FormFlowDbContext(DbContextOptions<FormFlowDbContext> options)
        : base(options) { }

    public DbSet<Video> Videos => Set<Video>();
    public DbSet<Analysis> Analyses => Set<Analysis>();

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        builder.Entity<Video>()
            .HasOne(v => v.User)
            .WithMany(u => u.Videos)
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Analysis>()
            .HasOne(a => a.Video)
            .WithMany(v => v.Analyses)
            .HasForeignKey(a => a.VideoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}