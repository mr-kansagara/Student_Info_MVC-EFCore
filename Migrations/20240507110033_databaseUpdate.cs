using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationOfStudent.Migrations
{
    /// <inheritdoc />
    public partial class databaseUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Student_BranchID",
                table: "Branch",
                column: "BranchID",
                principalTable: "Student",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branch_Student_BranchID",
                table: "Branch");
        }
    }
}
