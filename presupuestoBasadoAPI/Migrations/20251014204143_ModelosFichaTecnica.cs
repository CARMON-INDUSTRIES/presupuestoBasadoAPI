using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace presupuestoBasadoAPI.Migrations
{
    /// <inheritdoc />
    public partial class ModelosFichaTecnica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatosJSON",
                table: "Fichas");

            migrationBuilder.CreateTable(
                name: "IndicadoresDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FichaIndicadorId = table.Column<int>(type: "int", nullable: false),
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
                    FuenteResultado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FuenteNumerador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FuenteDenominador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineaBaseValor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineaBaseUnidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineaBaseAnio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineaBasePeriodo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndicadoresDetalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndicadoresDetalle_Fichas_FichaIndicadorId",
                        column: x => x.FichaIndicadorId,
                        principalTable: "Fichas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LineasAccion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FichaIndicadorId = table.Column<int>(type: "int", nullable: false),
                    Acuerdo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Objetivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estrategia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineaAccionTexto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ramo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineasAccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineasAccion_Fichas_FichaIndicadorId",
                        column: x => x.FichaIndicadorId,
                        principalTable: "Fichas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MetasProgramadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FichaIndicadorId = table.Column<int>(type: "int", nullable: false),
                    MetaProgramadaNombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cantidad = table.Column<double>(type: "float", nullable: false),
                    PeriodoCumplimiento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    CantidadEsperada = table.Column<double>(type: "float", nullable: false),
                    Alcanzado = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetasProgramadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetasProgramadas_Fichas_FichaIndicadorId",
                        column: x => x.FichaIndicadorId,
                        principalTable: "Fichas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IndicadoresDetalle_FichaIndicadorId",
                table: "IndicadoresDetalle",
                column: "FichaIndicadorId");

            migrationBuilder.CreateIndex(
                name: "IX_LineasAccion_FichaIndicadorId",
                table: "LineasAccion",
                column: "FichaIndicadorId");

            migrationBuilder.CreateIndex(
                name: "IX_MetasProgramadas_FichaIndicadorId",
                table: "MetasProgramadas",
                column: "FichaIndicadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndicadoresDetalle");

            migrationBuilder.DropTable(
                name: "LineasAccion");

            migrationBuilder.DropTable(
                name: "MetasProgramadas");

            migrationBuilder.AddColumn<string>(
                name: "DatosJSON",
                table: "Fichas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
