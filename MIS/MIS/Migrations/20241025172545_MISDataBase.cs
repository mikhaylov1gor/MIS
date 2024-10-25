using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MIS.Migrations
{
    /// <inheritdoc />
    public partial class MISDataBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DoctorModel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    birthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    gender = table.Column<int>(type: "int", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorModel", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    birthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    gender = table.Column<int>(type: "int", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Icd10",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Icd10", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    birthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    gender = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SpecialityModel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialityModel", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Specialties",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialties", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "InspectionCommentModel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    parentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    authorid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    modifyTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionCommentModel", x => x.id);
                    table.ForeignKey(
                        name: "FK_InspectionCommentModel_DoctorModel_authorid",
                        column: x => x.authorid,
                        principalTable: "DoctorModel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inspections",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    anamnesis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    complaints = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    treatment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    conclusion = table.Column<int>(type: "int", nullable: false),
                    nextVisitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deathDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    baseInspectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    previousInspectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    patientid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    doctorid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspections", x => x.id);
                    table.ForeignKey(
                        name: "FK_Inspections_Doctors_doctorid",
                        column: x => x.doctorid,
                        principalTable: "Doctors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inspections_Patients_patientid",
                        column: x => x.patientid,
                        principalTable: "Patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consultations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    inspectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    specialityid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultations", x => x.id);
                    table.ForeignKey(
                        name: "FK_Consultations_Specialties_specialityid",
                        column: x => x.specialityid,
                        principalTable: "Specialties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Diagnosis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type = table.Column<int>(type: "int", nullable: false),
                    DbInspectionid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnosis", x => x.id);
                    table.ForeignKey(
                        name: "FK_Diagnosis_Inspections_DbInspectionid",
                        column: x => x.DbInspectionid,
                        principalTable: "Inspections",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "InspectionConsultationModel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    inspectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    specialityid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    rootCommentid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    commentsNumber = table.Column<int>(type: "int", nullable: false),
                    DbInspectionid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionConsultationModel", x => x.id);
                    table.ForeignKey(
                        name: "FK_InspectionConsultationModel_InspectionCommentModel_rootCommentid",
                        column: x => x.rootCommentid,
                        principalTable: "InspectionCommentModel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InspectionConsultationModel_Inspections_DbInspectionid",
                        column: x => x.DbInspectionid,
                        principalTable: "Inspections",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_InspectionConsultationModel_SpecialityModel_specialityid",
                        column: x => x.specialityid,
                        principalTable: "SpecialityModel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    authorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    parentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DbConsultationid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_Comments_Consultations_DbConsultationid",
                        column: x => x.DbConsultationid,
                        principalTable: "Consultations",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_DbConsultationid",
                table: "Comments",
                column: "DbConsultationid");

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_specialityid",
                table: "Consultations",
                column: "specialityid");

            migrationBuilder.CreateIndex(
                name: "IX_Diagnosis_DbInspectionid",
                table: "Diagnosis",
                column: "DbInspectionid");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionCommentModel_authorid",
                table: "InspectionCommentModel",
                column: "authorid");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionConsultationModel_DbInspectionid",
                table: "InspectionConsultationModel",
                column: "DbInspectionid");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionConsultationModel_rootCommentid",
                table: "InspectionConsultationModel",
                column: "rootCommentid");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionConsultationModel_specialityid",
                table: "InspectionConsultationModel",
                column: "specialityid");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_doctorid",
                table: "Inspections",
                column: "doctorid");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_patientid",
                table: "Inspections",
                column: "patientid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Diagnosis");

            migrationBuilder.DropTable(
                name: "Icd10");

            migrationBuilder.DropTable(
                name: "InspectionConsultationModel");

            migrationBuilder.DropTable(
                name: "Consultations");

            migrationBuilder.DropTable(
                name: "InspectionCommentModel");

            migrationBuilder.DropTable(
                name: "Inspections");

            migrationBuilder.DropTable(
                name: "SpecialityModel");

            migrationBuilder.DropTable(
                name: "Specialties");

            migrationBuilder.DropTable(
                name: "DoctorModel");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
