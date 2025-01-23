using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AppCore.Migrations
{
    /// <inheritdoc />
    public partial class initialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "profile");

            migrationBuilder.EnsureSchema(
                name: "blog");

            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.CreateTable(
                name: "AppDocuments",
                schema: "profile",
                columns: table => new
                {
                    DocId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DocumentDescription = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DocumentAllowedFormats = table.Column<string[]>(type: "text[]", maxLength: 250, nullable: false),
                    MaxMbFileSize = table.Column<decimal>(type: "numeric", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDocuments", x => x.DocId);
                });

            migrationBuilder.CreateTable(
                name: "PostCategory",
                schema: "blog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    TitleNormalized = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    icon = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "varchar(250)", maxLength: 100, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateLastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsEmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeactivated = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Guid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsSocialLogin = table.Column<bool>(type: "boolean", nullable: false),
                    SocialLoginPlatform = table.Column<string>(type: "varchar(250)", nullable: true),
                    UserProfileImage = table.Column<string>(type: "text", nullable: true),
                    Username = table.Column<string>(type: "text", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    GuestId = table.Column<string>(type: "text", nullable: true),
                    Roles = table.Column<List<string>>(type: "text[]", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.Id);
                    table.UniqueConstraint("AK_UserProfile_Guid", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RoleDescription = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ConnectedApp = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDocument",
                schema: "profile",
                columns: table => new
                {
                    DocId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UserProfileId = table.Column<int>(type: "integer", nullable: false),
                    AppDocumentId = table.Column<int>(type: "integer", nullable: false),
                    DocumentName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FullDocumentNamePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ExtensionType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDocument", x => x.DocId);
                    table.ForeignKey(
                        name: "FK_UserDocument_AppDocuments_AppDocumentId",
                        column: x => x.AppDocumentId,
                        principalSchema: "profile",
                        principalTable: "AppDocuments",
                        principalColumn: "DocId");
                });

            migrationBuilder.CreateTable(
                name: "UserApp",
                schema: "auth",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(100)", nullable: false),
                    AppId = table.Column<string>(type: "text", nullable: true),
                    SocialPlatform = table.Column<string>(type: "text", nullable: true),
                    OAuthIdentity = table.Column<string>(type: "text", nullable: true),
                    UserRole = table.Column<List<string>>(type: "text[]", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserApp", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserApp_UserProfile_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "UserProfile",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_unique_postcategory_title",
                schema: "blog",
                table: "PostCategory",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDocument_AppDocumentId",
                schema: "profile",
                table: "UserDocument",
                column: "AppDocumentId");

            migrationBuilder.CreateIndex(
                name: "ix_UserDocument_UserProfileIdAppDocumentId_CompositeUniqueIndex",
                schema: "profile",
                table: "UserDocument",
                columns: new[] { "UserProfileId", "AppDocumentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_UserProfile_Email_Unique",
                schema: "auth",
                table: "UserProfile",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_UserProfile_Guid_Unique",
                schema: "auth",
                table: "UserProfile",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_UserProfile_Usernname_Unique",
                schema: "auth",
                table: "UserProfile",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostCategory",
                schema: "blog");

            migrationBuilder.DropTable(
                name: "UserApp",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "UserDocument",
                schema: "profile");

            migrationBuilder.DropTable(
                name: "UserRole",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "UserProfile",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "AppDocuments",
                schema: "profile");
        }
    }
}
