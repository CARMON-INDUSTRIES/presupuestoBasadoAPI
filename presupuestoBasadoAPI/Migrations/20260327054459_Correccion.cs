using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace presupuestoBasadoAPI.Migrations
{
    /// <inheritdoc />
    public partial class Correccion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComponenteObjetivo_ArbolObjetivos_ArbolObjetivosId",
                table: "ComponenteObjetivo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComponenteObjetivo",
                table: "ComponenteObjetivo");

            migrationBuilder.DropColumn(
                name: "Crema",
                table: "IndicadoresDetalle");

            migrationBuilder.RenameTable(
                name: "ComponenteObjetivo",
                newName: "ComponentesObjetivo");

            migrationBuilder.RenameIndex(
                name: "IX_ComponenteObjetivo_ArbolObjetivosId",
                table: "ComponentesObjetivo",
                newName: "IX_ComponentesObjetivo_ArbolObjetivosId");

            migrationBuilder.AddColumn<int>(
                name: "IndicadorDetalleId",
                table: "MetasProgramadas",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComponentesObjetivo",
                table: "ComponentesObjetivo",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MetasProgramadas_IndicadorDetalleId",
                table: "MetasProgramadas",
                column: "IndicadorDetalleId");

            migrationBuilder.CreateIndex(
                name: "IX_AcuerdoMunicipal_EntidadId",
                table: "AcuerdoMunicipal",
                column: "EntidadId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcuerdoMunicipal_Entidad_EntidadId",
                table: "AcuerdoMunicipal",
                column: "EntidadId",
                principalTable: "Entidad",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ComponentesObjetivo_ArbolObjetivos_ArbolObjetivosId",
                table: "ComponentesObjetivo",
                column: "ArbolObjetivosId",
                principalTable: "ArbolObjetivos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MetasProgramadas_IndicadoresDetalle_IndicadorDetalleId",
                table: "MetasProgramadas",
                column: "IndicadorDetalleId",
                principalTable: "IndicadoresDetalle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcuerdoMunicipal_Entidad_EntidadId",
                table: "AcuerdoMunicipal");

            migrationBuilder.DropForeignKey(
                name: "FK_ComponentesObjetivo_ArbolObjetivos_ArbolObjetivosId",
                table: "ComponentesObjetivo");

            migrationBuilder.DropForeignKey(
                name: "FK_MetasProgramadas_IndicadoresDetalle_IndicadorDetalleId",
                table: "MetasProgramadas");

            migrationBuilder.DropIndex(
                name: "IX_MetasProgramadas_IndicadorDetalleId",
                table: "MetasProgramadas");

            migrationBuilder.DropIndex(
                name: "IX_AcuerdoMunicipal_EntidadId",
                table: "AcuerdoMunicipal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComponentesObjetivo",
                table: "ComponentesObjetivo");

            migrationBuilder.DropColumn(
                name: "IndicadorDetalleId",
                table: "MetasProgramadas");

            migrationBuilder.DropColumn(
                name: "EntidadId",
                table: "AcuerdoMunicipal");

            migrationBuilder.RenameTable(
                name: "ComponentesObjetivo",
                newName: "ComponenteObjetivo");

            migrationBuilder.RenameIndex(
                name: "IX_ComponentesObjetivo_ArbolObjetivosId",
                table: "ComponenteObjetivo",
                newName: "IX_ComponenteObjetivo_ArbolObjetivosId");

            migrationBuilder.AddColumn<string>(
                name: "Crema",
                table: "IndicadoresDetalle",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComponenteObjetivo",
                table: "ComponenteObjetivo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ComponenteObjetivo_ArbolObjetivos_ArbolObjetivosId",
                table: "ComponenteObjetivo",
                column: "ArbolObjetivosId",
                principalTable: "ArbolObjetivos",
                principalColumn: "Id");
        }
    }
}
