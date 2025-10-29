using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace presupuestoBasadoAPI.Migrations
{
    /// <inheritdoc />
    public partial class ResultadoNonListCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Resultados_ComponenteId",
                table: "Resultados");

            migrationBuilder.CreateIndex(
                name: "IX_Resultados_ComponenteId",
                table: "Resultados",
                column: "ComponenteId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Resultados_ComponenteId",
                table: "Resultados");

            migrationBuilder.CreateIndex(
                name: "IX_Resultados_ComponenteId",
                table: "Resultados",
                column: "ComponenteId");
        }
    }
}
