using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ChnageTableRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_BedTypes_BedTypeId",
                table: "Rooms");

            migrationBuilder.AlterColumn<int>(
                name: "BedTypeId",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_BedTypes_BedTypeId",
                table: "Rooms",
                column: "BedTypeId",
                principalTable: "BedTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_BedTypes_BedTypeId",
                table: "Rooms");

            migrationBuilder.AlterColumn<int>(
                name: "BedTypeId",
                table: "Rooms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_BedTypes_BedTypeId",
                table: "Rooms",
                column: "BedTypeId",
                principalTable: "BedTypes",
                principalColumn: "Id");
        }
    }
}
