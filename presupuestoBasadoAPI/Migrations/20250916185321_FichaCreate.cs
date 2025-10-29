using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace presupuestoBasadoAPI.Migrations
{
    /// <inheritdoc />
    public partial class FichaCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fichas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnidadResponsable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnidadPresupuestal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClaveIndicador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgramaPresupuestario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoIndicador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponsableMIR = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fichas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Indicadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nivel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResultadoEsperado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dimension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sentido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Definicion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RangoValor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FrecuenciaMedicion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cobertura = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Numerador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Denominador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fuentes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FichaIndicadorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indicadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Indicadores_Fichas_FichaIndicadorId",
                        column: x => x.FichaIndicadorId,
                        principalTable: "Fichas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LineasBase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Periodo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IndicadorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineasBase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineasBase_Indicadores_IndicadorId",
                        column: x => x.IndicadorId,
                        principalTable: "Indicadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Metas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MetaProgramada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PeriodoCumplimiento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IndicadorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Metas_Indicadores_IndicadorId",
                        column: x => x.IndicadorId,
                        principalTable: "Indicadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramacionesMetas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Alcanzado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Semaforo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IndicadorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramacionesMetas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramacionesMetas_Indicadores_IndicadorId",
                        column: x => x.IndicadorId,
                        principalTable: "Indicadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Indicadores_FichaIndicadorId",
                table: "Indicadores",
                column: "FichaIndicadorId");

            migrationBuilder.CreateIndex(
                name: "IX_LineasBase_IndicadorId",
                table: "LineasBase",
                column: "IndicadorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Metas_IndicadorId",
                table: "Metas",
                column: "IndicadorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesMetas_IndicadorId",
                table: "ProgramacionesMetas",
                column: "IndicadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineasBase");

            migrationBuilder.DropTable(
                name: "Metas");

            migrationBuilder.DropTable(
                name: "ProgramacionesMetas");

            migrationBuilder.DropTable(
                name: "Indicadores");

            migrationBuilder.DropTable(
                name: "Fichas");
        }
    }
}
