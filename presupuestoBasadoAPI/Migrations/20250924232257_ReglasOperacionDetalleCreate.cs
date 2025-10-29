using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace presupuestoBasadoAPI.Migrations
{
    /// <inheritdoc />
    public partial class ReglasOperacionDetalleCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineasBase_Indicadores_IndicadorId",
                table: "LineasBase");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgramacionesMetas_Indicadores_IndicadorId",
                table: "ProgramacionesMetas");

            migrationBuilder.DropIndex(
                name: "IX_LineasBase_IndicadorId",
                table: "LineasBase");

            migrationBuilder.AlterColumn<int>(
                name: "IndicadorId",
                table: "ProgramacionesMetas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "IndicadorId",
                table: "LineasBase",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ReglasOperacionDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SujetoReglasOperacion = table.Column<bool>(type: "bit", nullable: false),
                    OtrosSubsidios = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrestacionServiciosPublicos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProvisionBienesPublicos = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReglasOperacionDetalles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LineasBase_IndicadorId",
                table: "LineasBase",
                column: "IndicadorId",
                unique: true,
                filter: "[IndicadorId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_LineasBase_Indicadores_IndicadorId",
                table: "LineasBase",
                column: "IndicadorId",
                principalTable: "Indicadores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramacionesMetas_Indicadores_IndicadorId",
                table: "ProgramacionesMetas",
                column: "IndicadorId",
                principalTable: "Indicadores",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineasBase_Indicadores_IndicadorId",
                table: "LineasBase");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgramacionesMetas_Indicadores_IndicadorId",
                table: "ProgramacionesMetas");

            migrationBuilder.DropTable(
                name: "ReglasOperacionDetalles");

            migrationBuilder.DropIndex(
                name: "IX_LineasBase_IndicadorId",
                table: "LineasBase");

            migrationBuilder.AlterColumn<int>(
                name: "IndicadorId",
                table: "ProgramacionesMetas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IndicadorId",
                table: "LineasBase",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LineasBase_IndicadorId",
                table: "LineasBase",
                column: "IndicadorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LineasBase_Indicadores_IndicadorId",
                table: "LineasBase",
                column: "IndicadorId",
                principalTable: "Indicadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramacionesMetas_Indicadores_IndicadorId",
                table: "ProgramacionesMetas",
                column: "IndicadorId",
                principalTable: "Indicadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
