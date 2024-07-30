using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraphQL.Migrations
{
    /// <inheritdoc />
    public partial class SpeakerJobType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobType",
                table: "Speakers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobType",
                table: "Speakers");
        }
    }
}
