using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiLevelEncryptedEshop.Migrations
{
    /// <inheritdoc />
    public partial class addedstoredfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoredFileId",
                table: "ProductMedia",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StoredFile",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SizeMB = table.Column<decimal>(type: "decimal(10,5)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsTemp = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Deletable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredFile", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductMedia_StoredFileId",
                table: "ProductMedia",
                column: "StoredFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductMedia_StoredFile_StoredFileId",
                table: "ProductMedia",
                column: "StoredFileId",
                principalTable: "StoredFile",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductMedia_StoredFile_StoredFileId",
                table: "ProductMedia");

            migrationBuilder.DropTable(
                name: "StoredFile");

            migrationBuilder.DropIndex(
                name: "IX_ProductMedia_StoredFileId",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "StoredFileId",
                table: "ProductMedia");
        }
    }
}
