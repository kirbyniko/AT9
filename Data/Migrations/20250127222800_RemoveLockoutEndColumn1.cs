using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AT9.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLockoutEndColumn1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "AspNetUsers");
        }
    }
}
