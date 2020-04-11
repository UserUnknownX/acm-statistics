﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AcmStatisticsBackend.Migrations
{
    public partial class AddAcHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UsernamesInCrawlers",
                table: "DefaultQueries",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "AcHistories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    MainUsername = table.Column<string>(nullable: false),
                    Submission = table.Column<int>(nullable: false),
                    Solved = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcHistories_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OjCrawlers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CrawlerName = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OjCrawlers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AcWorkerHistories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AcHistoryId = table.Column<long>(nullable: false),
                    OjCrawlerId = table.Column<int>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true),
                    Submission = table.Column<int>(nullable: false),
                    Solved = table.Column<int>(nullable: false),
                    HasSolvedList = table.Column<bool>(nullable: false),
                    SolvedListJson = table.Column<string>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcWorkerHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcWorkerHistories_AcHistories_AcHistoryId",
                        column: x => x.AcHistoryId,
                        principalTable: "AcHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcWorkerHistories_OjCrawlers_OjCrawlerId",
                        column: x => x.OjCrawlerId,
                        principalTable: "OjCrawlers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcHistories_UserId",
                table: "AcHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AcWorkerHistories_AcHistoryId",
                table: "AcWorkerHistories",
                column: "AcHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AcWorkerHistories_OjCrawlerId",
                table: "AcWorkerHistories",
                column: "OjCrawlerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcWorkerHistories");

            migrationBuilder.DropTable(
                name: "AcHistories");

            migrationBuilder.DropTable(
                name: "OjCrawlers");

            migrationBuilder.AlterColumn<string>(
                name: "UsernamesInCrawlers",
                table: "DefaultQueries",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
