using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateNewTableBedType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BedTypeId",
                table: "Rooms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BedTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BedTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_BedTypeId",
                table: "Rooms",
                column: "BedTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_BedTypes_BedTypeId",
                table: "Rooms",
                column: "BedTypeId",
                principalTable: "BedTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_BedTypes_BedTypeId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "BedTypes");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_BedTypeId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "BedTypeId",
                table: "Rooms");
        }
    }
}
