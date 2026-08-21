using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace Rentz.Intelligence.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyEmbeddings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Vector>(
                name: "Embedding",
                table: "Properties",
                type: "vector(768)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "Properties");
        }
    }
}
