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
    [Migration("20241106220456_updateInspectionTable")]
    partial class updateInspectionTable
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

                    b.Property<string>("passwordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("specialtyId")
                        .HasColumnType("uniqueidentifier");

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

                    b.Property<Guid?>("parentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("recordCode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("parentId")
                        .HasDatabaseName("IX_icd10_parentId");

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

                    b.Property<bool>("hasChain")
                        .HasColumnType("bit");

                    b.Property<bool>("hasNested")
                        .HasColumnType("bit");

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

            modelBuilder.Entity("MIS.Models.DB.DbInspectionComment", b =>
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

                    b.ToTable("InspectionComments");
                });

            modelBuilder.Entity("MIS.Models.DB.DbInspectionConsultation", b =>
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

                    b.ToTable("InspetionConsultations");
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

            modelBuilder.Entity("MIS.Models.DB.DbTokenBlackList", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("expirationTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("TokenBlackList");
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
                        .WithMany("inspections")
                        .HasForeignKey("patientid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("doctor");

                    b.Navigation("patient");
                });

            modelBuilder.Entity("MIS.Models.DB.DbInspectionComment", b =>
                {
                    b.HasOne("MIS.Models.DB.DbDoctor", "author")
                        .WithMany()
                        .HasForeignKey("authorid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("author");
                });

            modelBuilder.Entity("MIS.Models.DB.DbInspectionConsultation", b =>
                {
                    b.HasOne("MIS.Models.DB.DbInspection", null)
                        .WithMany("consultations")
                        .HasForeignKey("DbInspectionid");

                    b.HasOne("MIS.Models.DB.DbInspectionComment", "rootComment")
                        .WithMany()
                        .HasForeignKey("rootCommentid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MIS.Models.DB.DbSpecialty", "speciality")
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

            modelBuilder.Entity("MIS.Models.DB.DbPatient", b =>
                {
                    b.Navigation("inspections");
                });
#pragma warning restore 612, 618
        }
    }
}
