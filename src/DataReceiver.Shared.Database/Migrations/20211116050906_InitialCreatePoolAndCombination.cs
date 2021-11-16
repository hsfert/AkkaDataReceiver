using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataReceiver.Shared.Database.Migrations
{
    public partial class InitialCreatePoolAndCombination : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "combination",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pool_id = table.Column<int>(type: "int", nullable: false),
                    combination_id = table.Column<short>(type: "smallint", nullable: false),
                    sales = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    liability = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    odds = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    investment_number = table.Column<long>(type: "bigint", nullable: false),
                    odds_number = table.Column<long>(type: "bigint", nullable: false),
                    create_time = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_combination", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pool",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    game_id = table.Column<int>(type: "int", nullable: false),
                    instance_name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    create_time = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pool", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "combination_with_pool_id",
                table: "combination",
                columns: new[] { "pool_id", "combination_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "combination");

            migrationBuilder.DropTable(
                name: "pool");
        }
    }
}
