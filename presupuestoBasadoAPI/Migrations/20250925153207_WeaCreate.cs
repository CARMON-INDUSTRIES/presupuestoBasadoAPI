using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace presupuestoBasadoAPI.Migrations
{
    /// <inheritdoc />
    public partial class WeaCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcuerdoEstatal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcuerdoEstatal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AcuerdoMunicipal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcuerdoMunicipal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Finalidad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Finalidad", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ObjetivoEstatal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcuerdoEstatalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjetivoEstatal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjetivoEstatal_AcuerdoEstatal_AcuerdoEstatalId",
                        column: x => x.AcuerdoEstatalId,
                        principalTable: "AcuerdoEstatal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjetivoMunicipal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcuerdoMunicipalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjetivoMunicipal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjetivoMunicipal_AcuerdoMunicipal_AcuerdoMunicipalId",
                        column: x => x.AcuerdoMunicipalId,
                        principalTable: "AcuerdoMunicipal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Funcion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinalidadId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Funcion_Finalidad_FinalidadId",
                        column: x => x.FinalidadId,
                        principalTable: "Finalidad",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstrategiaEstatal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObjetivoEstatalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstrategiaEstatal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstrategiaEstatal_ObjetivoEstatal_ObjetivoEstatalId",
                        column: x => x.ObjetivoEstatalId,
                        principalTable: "ObjetivoEstatal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstrategiaMunicipal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObjetivoMunicipalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstrategiaMunicipal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstrategiaMunicipal_ObjetivoMunicipal_ObjetivoMunicipalId",
                        column: x => x.ObjetivoMunicipalId,
                        principalTable: "ObjetivoMunicipal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubFuncion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FuncionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubFuncion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubFuncion_Funcion_FuncionId",
                        column: x => x.FuncionId,
                        principalTable: "Funcion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LineaDeAccionEstatal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstrategiaEstatalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineaDeAccionEstatal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineaDeAccionEstatal_EstrategiaEstatal_EstrategiaEstatalId",
                        column: x => x.EstrategiaEstatalId,
                        principalTable: "EstrategiaEstatal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LineaDeAccionMunicipal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstrategiaMunicipalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineaDeAccionMunicipal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineaDeAccionMunicipal_EstrategiaMunicipal_EstrategiaMunicipalId",
                        column: x => x.EstrategiaMunicipalId,
                        principalTable: "EstrategiaMunicipal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstrategiaEstatal_ObjetivoEstatalId",
                table: "EstrategiaEstatal",
                column: "ObjetivoEstatalId");

            migrationBuilder.CreateIndex(
                name: "IX_EstrategiaMunicipal_ObjetivoMunicipalId",
                table: "EstrategiaMunicipal",
                column: "ObjetivoMunicipalId");

            migrationBuilder.CreateIndex(
                name: "IX_Funcion_FinalidadId",
                table: "Funcion",
                column: "FinalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_LineaDeAccionEstatal_EstrategiaEstatalId",
                table: "LineaDeAccionEstatal",
                column: "EstrategiaEstatalId");

            migrationBuilder.CreateIndex(
                name: "IX_LineaDeAccionMunicipal_EstrategiaMunicipalId",
                table: "LineaDeAccionMunicipal",
                column: "EstrategiaMunicipalId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjetivoEstatal_AcuerdoEstatalId",
                table: "ObjetivoEstatal",
                column: "AcuerdoEstatalId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjetivoMunicipal_AcuerdoMunicipalId",
                table: "ObjetivoMunicipal",
                column: "AcuerdoMunicipalId");

            migrationBuilder.CreateIndex(
                name: "IX_SubFuncion_FuncionId",
                table: "SubFuncion",
                column: "FuncionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineaDeAccionEstatal");

            migrationBuilder.DropTable(
                name: "LineaDeAccionMunicipal");

            migrationBuilder.DropTable(
                name: "SubFuncion");

            migrationBuilder.DropTable(
                name: "EstrategiaEstatal");

            migrationBuilder.DropTable(
                name: "EstrategiaMunicipal");

            migrationBuilder.DropTable(
                name: "Funcion");

            migrationBuilder.DropTable(
                name: "ObjetivoEstatal");

            migrationBuilder.DropTable(
                name: "ObjetivoMunicipal");

            migrationBuilder.DropTable(
                name: "Finalidad");

            migrationBuilder.DropTable(
                name: "AcuerdoEstatal");

            migrationBuilder.DropTable(
                name: "AcuerdoMunicipal");
        }
    }
}
