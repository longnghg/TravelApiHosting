using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Travel.Context.Migrations.Travel
{
    public partial class addColumnIntoTourBooking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<long>(
                name: "CheckIn",
                table: "TourBookings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CheckOut",
                table: "TourBookings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckIn",
                table: "TourBookings");

            migrationBuilder.DropColumn(
                name: "CheckOut",
                table: "TourBookings");

          
        }
    }
}
