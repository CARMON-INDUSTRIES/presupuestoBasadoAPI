using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace presupuestoBasadoAPI.Migrations
{
    /// <inheritdoc />
    public partial class MatrizCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "AlternativasEvaluacion",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateTable(
                name: "MatricesIndicadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnidadResponsable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnidadPresupuestal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgramaSectorial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgramaPresupuestario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponsableMIR = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatricesIndicadores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FilaMatriz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nivel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResumenNarrativo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Indicadores = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Medios = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Supuestos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatrizIndicadoresId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilaMatriz", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilaMatriz_MatricesIndicadores_MatrizIndicadoresId",
                        column: x => x.MatrizIndicadoresId,
                        principalTable: "MatricesIndicadores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilaMatriz_MatrizIndicadoresId",
                table: "FilaMatriz",
                column: "MatrizIndicadoresId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilaMatriz");

            migrationBuilder.DropTable(
                name: "MatricesIndicadores");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "AlternativasEvaluacion",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
