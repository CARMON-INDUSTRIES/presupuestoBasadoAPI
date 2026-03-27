using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace presupuestoBasadoAPI.Migrations
{
    /// <inheritdoc />
    public partial class Intento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComponentesObjetivo_ArbolObjetivos_ArbolObjetivosId",
                table: "ComponentesObjetivo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComponentesObjetivo",
                table: "ComponentesObjetivo");

            migrationBuilder.RenameTable(
                name: "ComponentesObjetivo",
                newName: "ComponenteObjetivo");

            migrationBuilder.RenameIndex(
                name: "IX_ComponentesObjetivo_ArbolObjetivosId",
                table: "ComponenteObjetivo",
                newName: "IX_ComponenteObjetivo_ArbolObjetivosId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComponenteObjetivo",
                table: "ComponenteObjetivo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ComponenteObjetivo_ArbolObjetivos_ArbolObjetivosId",
                table: "ComponenteObjetivo",
                column: "ArbolObjetivosId",
                principalTable: "ArbolObjetivos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComponenteObjetivo_ArbolObjetivos_ArbolObjetivosId",
                table: "ComponenteObjetivo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComponenteObjetivo",
                table: "ComponenteObjetivo");

            migrationBuilder.RenameTable(
                name: "ComponenteObjetivo",
                newName: "ComponentesObjetivo");

            migrationBuilder.RenameIndex(
                name: "IX_ComponenteObjetivo_ArbolObjetivosId",
                table: "ComponentesObjetivo",
                newName: "IX_ComponentesObjetivo_ArbolObjetivosId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComponentesObjetivo",
                table: "ComponentesObjetivo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ComponentesObjetivo_ArbolObjetivos_ArbolObjetivosId",
                table: "ComponentesObjetivo",
                column: "ArbolObjetivosId",
                principalTable: "ArbolObjetivos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
