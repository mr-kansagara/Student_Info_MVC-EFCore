using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationOfStudent.Migrations
{
    /// <inheritdoc />
    public partial class Add_registration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branch_Student_BranchID",
                table: "Branch");

            migrationBuilder.CreateTable(
                name: "Registration",
                columns: table => new
                {
                    RegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    userConfirmPassword = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registration", x => x.RegistrationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Student_BranchId",
                table: "Student",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Branch_BranchId",
                table: "Student",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "BranchID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_Branch_BranchId",
                table: "Student");

            migrationBuilder.DropTable(
                name: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_Student_BranchId",
                table: "Student");

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Student_BranchID",
                table: "Branch",
                column: "BranchID",
                principalTable: "Student",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
