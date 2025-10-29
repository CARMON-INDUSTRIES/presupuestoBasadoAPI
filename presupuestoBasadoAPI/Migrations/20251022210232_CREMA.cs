using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace presupuestoBasadoAPI.Migrations
{
    /// <inheritdoc />
    public partial class CREMA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Entidad_EntidadId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Crema",
                table: "IndicadoresDetalle",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Entidad_EntidadId",
                table: "AspNetUsers",
                column: "EntidadId",
                principalTable: "Entidad",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Entidad_EntidadId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Crema",
                table: "IndicadoresDetalle");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Entidad_EntidadId",
                table: "AspNetUsers",
                column: "EntidadId",
                principalTable: "Entidad",
                principalColumn: "Id");
        }
    }
}
