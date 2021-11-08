using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BackendWeb.Migrations
{
    public partial class Student : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    Id_student = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Surname = table.Column<string>(type: "varchar(50)", nullable: true),
                    Name = table.Column<string>(type: "varchar(30)", nullable: true),
                    Patronymic = table.Column<string>(type: "varchar(25)", nullable: true),
                    Date_birth = table.Column<DateTime>(type: "date", nullable: false),
                    Place_residence = table.Column<string>(type: "varchar(50)", nullable: true),
                    Telephone = table.Column<string>(type: "varchar(25)", nullable: true),
                    Email = table.Column<string>(type: "varchar(20)", nullable: true),
                    FullName_father = table.Column<string>(type: "varchar(60)", nullable: true),
                    Father_telephone = table.Column<string>(type: "varchar(25)", nullable: true),
                    FullName_mother = table.Column<string>(type: "varchar(60)", nullable: true),
                    Mother_telephone = table.Column<string>(type: "varchar(25)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Id_student);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Student");
        }
    }
}
