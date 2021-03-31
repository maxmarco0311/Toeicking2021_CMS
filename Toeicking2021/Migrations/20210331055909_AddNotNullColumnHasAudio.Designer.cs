﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Toeicking2021.Data;

namespace Toeicking2021.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20210331055909_AddNotNullColumnHasAudio")]
    partial class AddNotNullColumnHasAudio
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Toeicking2021.Models.Administrator", b =>
                {
                    b.Property<int>("AdministratorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Authcode")
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<bool?>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PassWord")
                        .IsRequired()
                        .HasColumnType("varchar(max)");

                    b.Property<string>("ResetPasswordCode")
                        .HasColumnType("nvarchar(12)");

                    b.HasKey("AdministratorId");

                    b.ToTable("Administrators");
                });

            modelBuilder.Entity("Toeicking2021.Models.GA", b =>
                {
                    b.Property<int>("AnalysisId")
                        .HasColumnType("int");

                    b.Property<string>("Analysis")
                        .HasColumnType("nvarchar(400)");

                    b.Property<int>("SentenceId")
                        .HasColumnType("int");

                    b.HasKey("AnalysisId");

                    b.HasIndex("SentenceId");

                    b.ToTable("GAs");
                });

            modelBuilder.Entity("Toeicking2021.Models.Sentence", b =>
                {
                    b.Property<int>("SentenceId")
                        .HasColumnType("int");

                    b.Property<DateTime>("AddedDate")
                        .HasColumnType("date");

                    b.Property<string>("Chinesese")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Context")
                        .IsRequired()
                        .HasColumnType("varchar(2)");

                    b.Property<string>("GrammarCategory")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("HasAudio")
                        .HasColumnType("bit");

                    b.Property<string>("Part")
                        .IsRequired()
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Sen")
                        .IsRequired()
                        .HasColumnType("nvarchar(600)");

                    b.Property<bool>("Synonym")
                        .HasColumnType("bit");

                    b.Property<bool>("WordOrigin")
                        .HasColumnType("bit");

                    b.HasKey("SentenceId");

                    b.ToTable("Sentences");
                });

            modelBuilder.Entity("Toeicking2021.Models.VA", b =>
                {
                    b.Property<int>("AnalysisId")
                        .HasColumnType("int");

                    b.Property<string>("Analysis")
                        .HasColumnType("nvarchar(300)");

                    b.Property<int>("SentenceId")
                        .HasColumnType("int");

                    b.HasKey("AnalysisId");

                    b.HasIndex("SentenceId");

                    b.ToTable("VAs");
                });

            modelBuilder.Entity("Toeicking2021.Models.Vocabulary", b =>
                {
                    b.Property<int>("VocabularyId")
                        .HasColumnType("int");

                    b.Property<string>("Category")
                        .HasColumnType("varchar(4)");

                    b.Property<string>("Chinese")
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("SentenceId")
                        .HasColumnType("int");

                    b.Property<string>("Voc")
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("VocabularyId");

                    b.HasIndex("SentenceId");

                    b.ToTable("Vocabularies");
                });

            modelBuilder.Entity("Toeicking2021.Models.GA", b =>
                {
                    b.HasOne("Toeicking2021.Models.Sentence", "Sentence")
                        .WithMany("GAs")
                        .HasForeignKey("SentenceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sentence");
                });

            modelBuilder.Entity("Toeicking2021.Models.VA", b =>
                {
                    b.HasOne("Toeicking2021.Models.Sentence", "Sentence")
                        .WithMany("Vas")
                        .HasForeignKey("SentenceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sentence");
                });

            modelBuilder.Entity("Toeicking2021.Models.Vocabulary", b =>
                {
                    b.HasOne("Toeicking2021.Models.Sentence", "Sentence")
                        .WithMany("Vocabularies")
                        .HasForeignKey("SentenceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sentence");
                });

            modelBuilder.Entity("Toeicking2021.Models.Sentence", b =>
                {
                    b.Navigation("GAs");

                    b.Navigation("Vas");

                    b.Navigation("Vocabularies");
                });
#pragma warning restore 612, 618
        }
    }
}
