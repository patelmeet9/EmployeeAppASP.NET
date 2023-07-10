using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lane4Task.Migrations
{
    /// <inheritdoc />
    public partial class AddtoDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNumber = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
    name: "Friends",
    columns: table => new
    {
        Id = table.Column<int>(nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
        UserId = table.Column<int>(nullable: false),
        FriendId = table.Column<int>(nullable: false),
        FriendEmail = table.Column<string>(nullable: false) // Add FriendEmail column
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_Friends", x => x.Id);
        table.ForeignKey(
            name: "FK_Friends_Users_FriendId",
            column: x => x.FriendId,
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.NoAction);
        table.ForeignKey(
            name: "FK_Friends_Users_UserId",
            column: x => x.UserId,
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.NoAction);
    });

            migrationBuilder.CreateIndex(
                name: "IX_Friends_FriendId",
                table: "Friends",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_UserId",
                table: "Friends",
                column: "UserId");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friends");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
