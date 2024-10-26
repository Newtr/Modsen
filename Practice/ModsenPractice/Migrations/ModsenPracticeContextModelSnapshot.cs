﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ModsenPractice.Data;

#nullable disable

namespace ModsenPractice.Migrations
{
    [DbContext(typeof(ModsenPracticeContext))]
    partial class ModsenPracticeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("MemberMyEvent", b =>
                {
                    b.Property<int>("EventMembersId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MemberEventsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("EventMembersId", "MemberEventsId");

                    b.HasIndex("MemberEventsId");

                    b.ToTable("EventsAndMembers", (string)null);
                });

            modelBuilder.Entity("ModsenPractice.Entity.EventImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EventId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ImagePath")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.ToTable("EventImages");
                });

            modelBuilder.Entity("ModsenPractice.Entity.Member", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Surname")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("ModsenPractice.Entity.MyEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateOfEvent")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("EventCategory")
                        .HasColumnType("TEXT");

                    b.Property<string>("EventLocation")
                        .HasColumnType("TEXT");

                    b.Property<int>("MaxMember")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("MemberMyEvent", b =>
                {
                    b.HasOne("ModsenPractice.Entity.Member", null)
                        .WithMany()
                        .HasForeignKey("EventMembersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ModsenPractice.Entity.MyEvent", null)
                        .WithMany()
                        .HasForeignKey("MemberEventsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ModsenPractice.Entity.EventImage", b =>
                {
                    b.HasOne("ModsenPractice.Entity.MyEvent", "MyEvent")
                        .WithMany("EventImages")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MyEvent");
                });

            modelBuilder.Entity("ModsenPractice.Entity.MyEvent", b =>
                {
                    b.Navigation("EventImages");
                });
#pragma warning restore 612, 618
        }
    }
}
