﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProject.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ItemShippers",
                newName: "ShipperId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShipperId",
                table: "ItemShippers",
                newName: "UserId");
        }
    }
}
