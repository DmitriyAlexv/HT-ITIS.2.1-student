using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LogicServer.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    HitPoints = table.Column<int>(type: "integer", nullable: false),
                    AttackModifier = table.Column<int>(type: "integer", nullable: false),
                    AttackPerRound = table.Column<int>(type: "integer", nullable: false),
                    MinDamage = table.Column<int>(type: "integer", nullable: false),
                    MaxDamage = table.Column<int>(type: "integer", nullable: false),
                    DamageModifier = table.Column<int>(type: "integer", nullable: false),
                    Weapon = table.Column<int>(type: "integer", nullable: false),
                    ArmorClass = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entities", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Entities",
                columns: new[] { "Id", "ArmorClass", "AttackModifier", "AttackPerRound", "DamageModifier", "HitPoints", "MaxDamage", "MinDamage", "Name", "Weapon" },
                values: new object[,]
                {
                    { 1, 19, 18, 1, 4, 52, 8, 1, "Hellwasp", 0 },
                    { 2, 12, 18, 2, 4, 59, 6, 2, "Griffon", 0 },
                    { 3, 14, 12, 1, 1, 42, 8, 2, "Vampirate", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entities");
        }
    }
}
