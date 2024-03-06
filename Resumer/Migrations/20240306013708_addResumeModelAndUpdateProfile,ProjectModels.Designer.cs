﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Resumer.models;

#nullable disable

namespace resume_builder.Migrations
{
    [DbContext(typeof(ResumeContext))]
    [Migration("20240306013708_addResumeModelAndUpdateProfile,ProjectModels")]
    partial class addResumeModelAndUpdateProfileProjectModels
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("Resumer.models.Education", b =>
                {
                    b.Property<string>("School")
                        .HasColumnType("TEXT");

                    b.Property<string>("Degree")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("AdditionalInformation")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly?>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("FieldOfStudy")
                        .HasColumnType("TEXT");

                    b.Property<float?>("GradePointAverage")
                        .HasColumnType("REAL");

                    b.Property<string>("Location")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProfileEmailAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProfileFirstName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProfileLastName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProfilePhoneNumber")
                        .HasColumnType("TEXT");

                    b.HasKey("School", "Degree", "StartDate");

                    b.HasIndex("ProfileFirstName", "ProfileLastName", "ProfileEmailAddress", "ProfilePhoneNumber");

                    b.ToTable("Education");
                });

            modelBuilder.Entity("Resumer.models.Job", b =>
                {
                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<string>("Company")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly?>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Experience")
                        .HasColumnType("TEXT");

                    b.HasKey("Title", "Company", "StartDate");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("Resumer.models.Profile", b =>
                {
                    b.Property<string>("FirstName")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .HasColumnType("TEXT");

                    b.Property<string>("EmailAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<string>("Certifications")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Interests")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Languages")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Location")
                        .HasColumnType("TEXT");

                    b.Property<string>("MiddleName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Objective")
                        .HasColumnType("TEXT");

                    b.Property<string>("Website")
                        .HasColumnType("TEXT");

                    b.HasKey("FirstName", "LastName", "EmailAddress", "PhoneNumber");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("Resumer.models.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly?>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Link")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateOnly?>("StartDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Resumer.models.Skill", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Name");

                    b.ToTable("Skills");
                });

            modelBuilder.Entity("Resumer.models.Education", b =>
                {
                    b.HasOne("Resumer.models.Profile", null)
                        .WithMany("Education")
                        .HasForeignKey("ProfileFirstName", "ProfileLastName", "ProfileEmailAddress", "ProfilePhoneNumber");
                });

            modelBuilder.Entity("Resumer.models.Profile", b =>
                {
                    b.Navigation("Education");
                });
#pragma warning restore 612, 618
        }
    }
}
