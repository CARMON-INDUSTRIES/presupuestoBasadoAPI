using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace presupuestoBasadoAPI.Migrations
{
    /// <inheritdoc />
    public partial class AllModelsWuserIdCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Resultados",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ReglasOperacionDetalles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ReglasOperacion",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Ramos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ProgramaSocialCategorias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ProgramaSocial",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Programas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ProgramacionesMetas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PoblacionObjetivo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PadronBeneficiarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Metas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "MatricesIndicadores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "LineasBase",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "JustificacionProgramas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Indicadores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "IdentificacionProblemas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "IdentificacionDescripcionProblemas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "FilaMatriz",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Fichas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "EfectosSuperiores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "DisenoIntervencionPublicas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "DeterminacionJustificacionObjetivo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Componentes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ComponenteObjetivo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Coberturas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ClasificacionesFuncionales",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Antecedentes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AnalisisEntorno",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AnalisisAlternativas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AlternativasEvaluacion",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AlineacionesMunicipio",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Resultados");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ReglasOperacionDetalles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ReglasOperacion");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Ramos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProgramaSocialCategorias");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProgramaSocial");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Programas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProgramacionesMetas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PoblacionObjetivo");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PadronBeneficiarios");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Metas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MatricesIndicadores");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LineasBase");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "JustificacionProgramas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Indicadores");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "IdentificacionProblemas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "IdentificacionDescripcionProblemas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FilaMatriz");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Fichas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EfectosSuperiores");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DisenoIntervencionPublicas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DeterminacionJustificacionObjetivo");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Componentes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ComponenteObjetivo");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Coberturas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ClasificacionesFuncionales");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Antecedentes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AnalisisEntorno");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AnalisisAlternativas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AlternativasEvaluacion");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AlineacionesMunicipio");
        }
    }
}
