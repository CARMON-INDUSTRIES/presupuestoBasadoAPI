using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace presupuestoBasadoAPI.Migrations
{
    /// <inheritdoc />
    public partial class EntidadTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EntidadId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Entidad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entidad", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EntidadId",
                table: "AspNetUsers",
                column: "EntidadId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Entidad_EntidadId",
                table: "AspNetUsers",
                column: "EntidadId",
                principalTable: "Entidad",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Entidad_EntidadId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Entidad");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EntidadId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EntidadId",
                table: "AspNetUsers");
        }
    }
}
