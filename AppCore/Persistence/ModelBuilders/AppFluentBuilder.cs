using AppCore.Domain.AppCore.Enums;
using AppCore.Domain.AppCore.Models;
using Microsoft.EntityFrameworkCore;

namespace AppCore.Persistence.ModelBuilders;

public static class AppFluentBuilder
{
    public static ModelBuilder SeedBuilder(this ModelBuilder model)
    {
        model.Entity<UserProfile>(prop =>
        {
            prop.Property(m => m.Email).HasColumnType("varchar(100)");
            prop.Property(m => m.FirstName).HasColumnType("varchar(100)");
            prop.Property(m => m.LastName).HasColumnType("varchar(100)");
            prop.Property(m => m.Password).HasColumnType("varchar(250)");
            prop.HasIndex(m => m.Guid, "ix_UserProfile_Guid_Unique").IsUnique();
            prop.HasIndex(m => m.Email, "ix_UserProfile_Email_Unique").IsUnique();
            prop.HasIndex(m => m.Username, "ix_UserProfile_Usernname_Unique").IsUnique();
        });

        

        //model.Entity<UserRole>(prop =>
        //{
        //    prop.Property(m => m.RoleName).HasColumnType("varchar(100)");
        //    prop.HasIndex(m => m.RoleName, name: "ix_unique_userrole_rolename").IsUnique();
        //    prop.Property(m => m.RoleDescription).HasColumnType("varchar(250)");
        //    prop.Property(m => m.IsActive).HasDefaultValue(true);
        //    prop.Property(m => m.IsDeleted).HasDefaultValue(false);
        //    prop.HasMany<UserApp>(u => u.UserProfileRoles).WithOne(p => p.UserRole).HasForeignKey(f => f.UserRoleId).OnDelete(DeleteBehavior.Cascade);
        //});

        model.Entity<UserProfile>(prop =>
        {
            prop.HasMany<UserApp>(u => u.UserProfileUserApps).WithOne(p => p.UserProfile).HasForeignKey(f => f.UserId).OnDelete(DeleteBehavior.Cascade);
        });
        model.Entity<AppDocuments>(prop =>
        {
            prop.HasMany<UserDocument>(u => u.UserAppDocument).WithOne(p => p.AppUserDocuments).HasForeignKey(f => f.AppDocumentId).OnDelete(DeleteBehavior.NoAction);
        });
        model.Entity<UserDocument>(prop =>
        {
            prop.HasIndex(u => new { u.UserProfileId, u.AppDocumentId }, name: "ix_UserDocument_UserProfileIdAppDocumentId_CompositeUniqueIndex").IsUnique();
        });

        #region Populate database tables

        model.Entity<UserRole>().HasData(
          new UserRole { Id = (int)UserRolesEnum.Member, RoleName = UserRolesEnum.Member.ToString(), RoleDescription = "This is the default role for all users" },
            new UserRole { Id = (int)UserRolesEnum.Admin, RoleName = UserRolesEnum.Admin.ToString(), RoleDescription = "This is the Adminstrative role." }
            );

        #endregion

        return model;
    }
}
