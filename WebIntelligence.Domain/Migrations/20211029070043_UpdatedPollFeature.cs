using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebIntelligence.Domain.Migrations
{
    public partial class UpdatedPollFeature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserVotes_Polls_PollId",
                table: "UserVotes");

            migrationBuilder.DropColumn(
                name: "CachedFinishingMessage",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "TotalVotes",
                table: "Polls");

            migrationBuilder.AlterColumn<Guid>(
                name: "PollId",
                table: "UserVotes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Finalized",
                table: "Polls",
                type: "bool",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_UserVotes_Polls_PollId",
                table: "UserVotes",
                column: "PollId",
                principalTable: "Polls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserVotes_Polls_PollId",
                table: "UserVotes");

            migrationBuilder.DropColumn(
                name: "Finalized",
                table: "Polls");

            migrationBuilder.AlterColumn<Guid>(
                name: "PollId",
                table: "UserVotes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "CachedFinishingMessage",
                table: "Polls",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalVotes",
                table: "Polls",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_UserVotes_Polls_PollId",
                table: "UserVotes",
                column: "PollId",
                principalTable: "Polls",
                principalColumn: "Id");
        }
    }
}
