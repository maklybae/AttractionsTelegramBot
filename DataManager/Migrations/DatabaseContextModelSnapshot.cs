﻿// <auto-generated />
using System;
using DataManager;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataManager.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DataManager.Models.Chat", b =>
                {
                    b.Property<long>("ChatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("chat_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ChatId"));

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("status");

                    b.HasKey("ChatId");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("DataManager.Models.ChatFile", b =>
                {
                    b.Property<int>("FileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("file_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("FileId"));

                    b.Property<string>("ChatFileId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("telegram_file_id");

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool>("IsSource")
                        .HasColumnType("boolean")
                        .HasColumnName("is_source");

                    b.HasKey("FileId");

                    b.HasIndex("ChatId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("DataManager.Models.Selection", b =>
                {
                    b.Property<int>("SelectionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("selection_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SelectionId"));

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("now()");

                    b.Property<int?>("FileId")
                        .HasColumnType("integer");

                    b.HasKey("SelectionId");

                    b.HasIndex("ChatId");

                    b.HasIndex("FileId");

                    b.ToTable("Selections");
                });

            modelBuilder.Entity("DataManager.Models.SelectionParams", b =>
                {
                    b.Property<int>("SelectionParamsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("selection_params_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SelectionParamsId"));

                    b.Property<int>("Field")
                        .HasColumnType("integer")
                        .HasColumnName("field");

                    b.Property<int>("SelectionId")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.HasKey("SelectionParamsId");

                    b.HasIndex("SelectionId");

                    b.ToTable("SelectionParams");
                });

            modelBuilder.Entity("DataManager.Models.ChatFile", b =>
                {
                    b.HasOne("DataManager.Models.Chat", "Chat")
                        .WithMany()
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");
                });

            modelBuilder.Entity("DataManager.Models.Selection", b =>
                {
                    b.HasOne("DataManager.Models.Chat", "Chat")
                        .WithMany()
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataManager.Models.ChatFile", "File")
                        .WithMany()
                        .HasForeignKey("FileId");

                    b.Navigation("Chat");

                    b.Navigation("File");
                });

            modelBuilder.Entity("DataManager.Models.SelectionParams", b =>
                {
                    b.HasOne("DataManager.Models.Selection", "Selection")
                        .WithMany()
                        .HasForeignKey("SelectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Selection");
                });
#pragma warning restore 612, 618
        }
    }
}