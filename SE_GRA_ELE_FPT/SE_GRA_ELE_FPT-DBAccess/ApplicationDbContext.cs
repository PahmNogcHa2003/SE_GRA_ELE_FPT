
using SE_GRA_ELE_FPT_DBAccess.Repositories.RepositoryBase;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SE_GRA_ELE_FPT_DBAccess.Entities;

namespace SE_GRA_ELE_FPT_DBAccess
{
    public class ApplicationDbContext : IdentityDbContext<Users, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

       
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
        
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //    //SeedRoles(builder);
        //    // Remove prefix name of table
        //    foreach (var entityType in builder.Model.GetEntityTypes())
        //    {
        //        var tableName = entityType.GetTableName();
        //        if (!string.IsNullOrEmpty(tableName) && tableName.StartsWith("AspNet"))
        //        {
        //            entityType.SetTableName(tableName[6..]);
        //        }
        //    }
        //}

        //private void SeedRoles(ModelBuilder builder)
        //{
        //    builder.Entity<IdentityRole<int>>().HasData(
        //            new IdentityRole<int>() { Id = 1, Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "ADMIN" },
        //            new IdentityRole<int>() { Id = 2, Name = "User", ConcurrencyStamp = "2", NormalizedName = "USER" },
        //            new IdentityRole<int>() { Id = 3, Name = "Member", ConcurrencyStamp = "3", NormalizedName = "MEMBER" }
        //            );
        //}

    }
}
