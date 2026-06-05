using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace presupuestoBasadoAPI.Migrations
{
    /// <inheritdoc />
    public partial class Restauracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilaMatriz_MatricesIndicadores_MatrizIndicadoresId",
                table: "FilaMatriz");

            migrationBuilder.AlterColumn<int>(
                name: "MatrizIndicadoresId",
                table: "FilaMatriz",
                type: "int",
                nullable: false,
                defaultValue: 0, 
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FilaMatriz_MatricesIndicadores_MatrizIndicadoresId",
                table: "FilaMatriz",
                column: "MatrizIndicadoresId",
                principalTable: "MatricesIndicadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilaMatriz_MatricesIndicadores_MatrizIndicadoresId",
                table: "FilaMatriz");

            migrationBuilder.AlterColumn<int>(
                name: "MatrizIndicadoresId",
                table: "FilaMatriz",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_FilaMatriz_MatricesIndicadores_MatrizIndicadoresId",
                table: "FilaMatriz",
                column: "MatrizIndicadoresId",
                principalTable: "MatricesIndicadores",
                principalColumn: "Id");
        }
    }
}
