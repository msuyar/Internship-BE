using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserWatchedMoviesChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<Guid>>(
                name: "WatchedMovies",
                table: "Users",
                type: "uuid[]",
                nullable: false,
                oldClrType: typeof(List<Guid>),
                oldType: "uuid[]",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<Guid>>(
                name: "WatchedMovies",
                table: "Users",
                type: "uuid[]",
                nullable: true,
                oldClrType: typeof(List<Guid>),
                oldType: "uuid[]");
        }
    }
}
