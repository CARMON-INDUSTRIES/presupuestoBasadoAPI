using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace presupuestoBasadoAPI.Migrations
{
    /// <inheritdoc />
    public partial class AnalisisAlternativasCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalisisAlternativas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalObtenido = table.Column<int>(type: "int", nullable: false),
                    TotalMaximo = table.Column<int>(type: "int", nullable: false),
                    Probabilidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalisisAlternativas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlternativasEvaluacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnalisisAlternativasId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Facultad = table.Column<int>(type: "int", nullable: false),
                    Presupuesto = table.Column<int>(type: "int", nullable: false),
                    CortoPlazo = table.Column<int>(type: "int", nullable: false),
                    RecursosTecnicos = table.Column<int>(type: "int", nullable: false),
                    RecursosAdministrativos = table.Column<int>(type: "int", nullable: false),
                    CulturalSocial = table.Column<int>(type: "int", nullable: false),
                    Impacto = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlternativasEvaluacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlternativasEvaluacion_AnalisisAlternativas_AnalisisAlternativasId",
                        column: x => x.AnalisisAlternativasId,
                        principalTable: "AnalisisAlternativas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlternativasEvaluacion_AnalisisAlternativasId",
                table: "AlternativasEvaluacion",
                column: "AnalisisAlternativasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlternativasEvaluacion");

            migrationBuilder.DropTable(
                name: "AnalisisAlternativas");
        }
    }
}
