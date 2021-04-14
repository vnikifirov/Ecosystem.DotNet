using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace bankidentificationcode.Migrations
{
    public partial class InitialModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "PZN",
                schema: "dbo",
                columns: table => new
                {
                    VKEY = table.Column<string>(type: "char(8)", nullable: false),
                    CB_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CE_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IMY = table.Column<string>(nullable: true),
                    NAME = table.Column<string>(nullable: true),
                    PZN = table.Column<string>(type: "char(2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PZN", x => x.VKEY);
                    table.UniqueConstraint("AK_PZN_PZN", x => x.PZN);
                });

            migrationBuilder.CreateTable(
                name: "REG",
                schema: "dbo",
                columns: table => new
                {
                    VKEY = table.Column<string>(type: "char(8)", nullable: false),
                    CENTER = table.Column<string>(type: "char(45)", nullable: true),
                    NAME = table.Column<string>(type: "char(45)", nullable: true),
                    NAMET = table.Column<string>(type: "char(45)", nullable: true),
                    RGN = table.Column<string>(type: "char(2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REG", x => x.VKEY);
                    table.UniqueConstraint("AK_REG_RGN", x => x.RGN);
                });

            migrationBuilder.CreateTable(
                name: "TNP",
                schema: "dbo",
                columns: table => new
                {
                    VKEY = table.Column<string>(type: "char(8)", nullable: false),
                    FULLNAME = table.Column<string>(type: "char(45)", nullable: true),
                    SHORTNAME = table.Column<string>(type: "char(16)", nullable: true),
                    TNP = table.Column<string>(type: "char(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TNP", x => x.VKEY);
                    table.UniqueConstraint("AK_TNP_TNP", x => x.TNP);
                });

            migrationBuilder.CreateTable(
                name: "UER",
                schema: "dbo",
                columns: table => new
                {
                    VKEY = table.Column<string>(type: "char(8)", nullable: false),
                    UER = table.Column<string>(type: "char(1)", nullable: false),
                    UERNAME = table.Column<string>(type: "char(80)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UER", x => x.VKEY);
                    table.UniqueConstraint("AK_UER_UER", x => x.UER);
                });

            migrationBuilder.CreateTable(
                name: "BNKSEEK",
                schema: "dbo",
                columns: table => new
                {
                    VKEY = table.Column<string>(type: "char(8)", nullable: false),
                    ADR = table.Column<string>(type: "char(30)", nullable: true),
                    AT1 = table.Column<string>(type: "char(7)", nullable: true),
                    AT2 = table.Column<string>(type: "char(7)", nullable: true),
                    CKS = table.Column<string>(type: "char(6)", nullable: true),
                    DATE_CH = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DATE_IN = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DT_IZM = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DT_IZMR = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IND = table.Column<string>(type: "char(6)", nullable: true),
                    KSNP = table.Column<string>(type: "char(20)", nullable: true),
                    NAMEN = table.Column<string>(type: "char(30)", nullable: false),
                    NAMEP = table.Column<string>(type: "char(45)", nullable: false),
                    NEWKS = table.Column<string>(type: "char(9)", nullable: true),
                    NEWNUM = table.Column<string>(type: "char(9)", nullable: false),
                    NNP = table.Column<string>(type: "char(25)", nullable: true),
                    OKPO = table.Column<string>(type: "char(8)", nullable: true),
                    PERMFO = table.Column<string>(type: "char(6)", nullable: true),
                    PZN = table.Column<string>(type: "char(2)", nullable: true),
                    REAL = table.Column<string>(type: "char(4)", nullable: true),
                    REGN = table.Column<string>(type: "char(9)", nullable: true),
                    RGN = table.Column<string>(type: "char(2)", nullable: false),
                    RKC = table.Column<string>(type: "char(9)", nullable: true),
                    SROK = table.Column<string>(type: "char(2)", nullable: false),
                    TELEF = table.Column<string>(type: "char(25)", nullable: true),
                    TNP = table.Column<string>(type: "char(1)", nullable: true),
                    UER = table.Column<string>(type: "char(1)", nullable: false),
                    VKEYDEL = table.Column<string>(type: "char(9)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BNKSEEK", x => x.VKEY);
                    table.ForeignKey(
                        name: "FK_BNKSEEK_PZN",
                        column: x => x.PZN,
                        principalSchema: "dbo",
                        principalTable: "PZN",
                        principalColumn: "PZN",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BNKSEEK_REG",
                        column: x => x.RGN,
                        principalSchema: "dbo",
                        principalTable: "REG",
                        principalColumn: "RGN",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BNKSEEK_TNP",
                        column: x => x.TNP,
                        principalSchema: "dbo",
                        principalTable: "TNP",
                        principalColumn: "TNP",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BNKSEEK_UER",
                        column: x => x.UER,
                        principalSchema: "dbo",
                        principalTable: "UER",
                        principalColumn: "UER",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BNKSEEK_NEWNUM",
                schema: "dbo",
                table: "BNKSEEK",
                column: "NEWNUM",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BNKSEEK_PZN",
                schema: "dbo",
                table: "BNKSEEK",
                column: "PZN");

            migrationBuilder.CreateIndex(
                name: "IX_BNKSEEK_RGN",
                schema: "dbo",
                table: "BNKSEEK",
                column: "RGN");

            migrationBuilder.CreateIndex(
                name: "IX_BNKSEEK_TNP",
                schema: "dbo",
                table: "BNKSEEK",
                column: "TNP");

            migrationBuilder.CreateIndex(
                name: "IX_BNKSEEK_UER",
                schema: "dbo",
                table: "BNKSEEK",
                column: "UER");

            migrationBuilder.CreateIndex(
                name: "IX_BNKSEEK_VKEYDEL",
                schema: "dbo",
                table: "BNKSEEK",
                column: "VKEYDEL");

            migrationBuilder.CreateIndex(
                name: "IX_PZN_PZN",
                schema: "dbo",
                table: "PZN",
                column: "PZN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_REG_RGN",
                schema: "dbo",
                table: "REG",
                column: "RGN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TNP_TNP",
                schema: "dbo",
                table: "TNP",
                column: "TNP",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UER_UER",
                schema: "dbo",
                table: "UER",
                column: "UER",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BNKSEEK",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PZN",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "REG",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TNP",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UER",
                schema: "dbo");
        }
    }
}
