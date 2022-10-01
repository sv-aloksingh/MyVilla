using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyVilla_WebAPI.Migrations
{
    public partial class AddVillaTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Villas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Details = table.Column<string>(nullable: true),
                    Rate = table.Column<double>(nullable: false),
                    Sqft = table.Column<int>(nullable: false),
                    Occupancy = table.Column<int>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    Amenity = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Villas", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Villas");
        }
    }
}
