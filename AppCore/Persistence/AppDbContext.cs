using AppCore.Domain.AppCore.Models;
using AppCore.Domain.AppCore.Models.TransactionModels;
using AppCore.Domain.Blog.Entities;
using AppCore.Persistence.ModelBuilders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCore.Persistence;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        modelBuilder.SeedBuilder();
    }

    #region BLOG
    public DbSet<PostCategory> PostCategories{ get; set; }

    #endregion
    //public DbSet<MessageBox> MessageBoxz { get; set; }

    public DbSet<ResourceAccess> ResourceAccess { get; set; }
    public DbSet<UserProfile> UserProfiles{ get; set; }
    public DbSet<UserApp> UserApps { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    public DbSet<AppDocuments> AppDocumentz { get; set; }
    public DbSet<UserDocument> UserDocuments { get; set; }

    public DbSet<Transactions> Transactions { get; set; }
    public DbSet<TransactionsPlatform> TransactionsPlatforms { get; set; }
}