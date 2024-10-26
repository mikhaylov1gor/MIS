﻿// <auto-generated />
using System;
using MIS.Models.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MIS.Migrations
{
    [DbContext(typeof(MisDbContext))]
    [Migration("20241025172545_MISDataBase")]
    partial class MISDataBase
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MIS.Models.DB.DbComment", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("DbConsultationid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("authorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("modifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("parentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("id");

                    b.HasIndex("DbConsultationid");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("MIS.Models.DB.DbConsultation", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("inspectionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("specialityid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("id");

                    b.HasIndex("specialityid");

                    b.ToTable("Consultations");
                });

            modelBuilder.Entity("MIS.Models.DB.DbDiagnosis", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("DbInspectionid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("type")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("DbInspectionid");

                    b.ToTable("Diagnosis");
                });

            modelBuilder.Entity("MIS.Models.DB.DbDoctor", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("birthday")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("gender")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("phone")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("MIS.Models.DB.DbIcd10", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Icd10");
                });

            modelBuilder.Entity("MIS.Models.DB.DbInspection", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("anamnesis")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("baseInspectionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("complaints")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("conclusion")
                        .HasColumnType("int");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("date")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("deathDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("doctorid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("nextVisitDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("patientid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("previousInspectionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("treatment")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("doctorid");

                    b.HasIndex("patientid");

                    b.ToTable("Inspections");
                });

            modelBuilder.Entity("MIS.Models.DB.DbPatient", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("birthday")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("gender")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("MIS.Models.DB.DbSpecialty", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Specialties");
                });

            modelBuilder.Entity("MIS.Models.DTO.DoctorModel", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("birthday")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("gender")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("phone")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("DoctorModel");
                });

            modelBuilder.Entity("MIS.Models.DTO.InspectionCommentModel", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("authorid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("modifyTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("parentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("id");

                    b.HasIndex("authorid");

                    b.ToTable("InspectionCommentModel");
                });

            modelBuilder.Entity("MIS.Models.DTO.InspectionConsultationModel", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("DbInspectionid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("commentsNumber")
                        .HasColumnType("int");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("inspectionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("rootCommentid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("specialityid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("id");

                    b.HasIndex("DbInspectionid");

                    b.HasIndex("rootCommentid");

                    b.HasIndex("specialityid");

                    b.ToTable("InspectionConsultationModel");
                });

            modelBuilder.Entity("MIS.Models.DTO.SpecialityModel", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("SpecialityModel");
                });

            modelBuilder.Entity("MIS.Models.DB.DbComment", b =>
                {
                    b.HasOne("MIS.Models.DB.DbConsultation", null)
                        .WithMany("comments")
                        .HasForeignKey("DbConsultationid");
                });

            modelBuilder.Entity("MIS.Models.DB.DbConsultation", b =>
                {
                    b.HasOne("MIS.Models.DB.DbSpecialty", "speciality")
                        .WithMany()
                        .HasForeignKey("specialityid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("speciality");
                });

            modelBuilder.Entity("MIS.Models.DB.DbDiagnosis", b =>
                {
                    b.HasOne("MIS.Models.DB.DbInspection", null)
                        .WithMany("diagnoses")
                        .HasForeignKey("DbInspectionid");
                });

            modelBuilder.Entity("MIS.Models.DB.DbInspection", b =>
                {
                    b.HasOne("MIS.Models.DB.DbDoctor", "doctor")
                        .WithMany()
                        .HasForeignKey("doctorid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MIS.Models.DB.DbPatient", "patient")
                        .WithMany()
                        .HasForeignKey("patientid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("doctor");

                    b.Navigation("patient");
                });

            modelBuilder.Entity("MIS.Models.DTO.InspectionCommentModel", b =>
                {
                    b.HasOne("MIS.Models.DTO.DoctorModel", "author")
                        .WithMany()
                        .HasForeignKey("authorid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("author");
                });

            modelBuilder.Entity("MIS.Models.DTO.InspectionConsultationModel", b =>
                {
                    b.HasOne("MIS.Models.DB.DbInspection", null)
                        .WithMany("consultations")
                        .HasForeignKey("DbInspectionid");

                    b.HasOne("MIS.Models.DTO.InspectionCommentModel", "rootComment")
                        .WithMany()
                        .HasForeignKey("rootCommentid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MIS.Models.DTO.SpecialityModel", "speciality")
                        .WithMany()
                        .HasForeignKey("specialityid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("rootComment");

                    b.Navigation("speciality");
                });

            modelBuilder.Entity("MIS.Models.DB.DbConsultation", b =>
                {
                    b.Navigation("comments");
                });

            modelBuilder.Entity("MIS.Models.DB.DbInspection", b =>
                {
                    b.Navigation("consultations");

                    b.Navigation("diagnoses");
                });
#pragma warning restore 612, 618
        }
    }
}