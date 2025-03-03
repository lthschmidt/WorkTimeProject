using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkTimeAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSqlInTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Transactions",
                type: "DATE",
                nullable: false,
                defaultValueSql: "DATE('now', 'localtime')",
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldDefaultValueSql: "DATE('now')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Transactions",
                type: "DATE",
                nullable: false,
                defaultValueSql: "DATE('now')",
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldDefaultValueSql: "DATE('now', 'localtime')");
        }
    }
}
