using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SSSClientWeb.Models;

public partial class SssclientContext : DbContext
{
    public SssclientContext()
    {
    }

    public SssclientContext(DbContextOptions<SssclientContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketStatus> TicketStatuses { get; set; }

    public virtual DbSet<TicketTime> TicketTimes { get; set; }

    public virtual DbSet<VwOpenTicket> VwOpenTickets { get; set; }

    public virtual DbSet<VwTicketTime> VwTicketTimes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=Sawyer-A6;Database=SSSClient;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientRec);

            entity.ToTable("Client");

            entity.Property(e => e.ClientAddr1)
                .HasMaxLength(50)
                .HasColumnName("Client_Addr1");
            entity.Property(e => e.ClientAddr2)
                .HasMaxLength(50)
                .HasColumnName("Client_Addr2");
            entity.Property(e => e.ClientCity)
                .HasMaxLength(50)
                .HasColumnName("Client_City");
            entity.Property(e => e.ClientName)
                .HasMaxLength(50)
                .HasColumnName("Client_Name");
            entity.Property(e => e.ClientState)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("Client_State");
            entity.Property(e => e.ClientZip)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("Client_Zip");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerRec);

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerName)
                .HasMaxLength(50)
                .HasColumnName("Customer_Name");

            entity.HasOne(d => d.ClientRecNavigation).WithMany(p => p.Customers)
                .HasForeignKey(d => d.ClientRec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Customer_Client");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketRec);

            entity.ToTable("Ticket");

            entity.Property(e => e.DateLogged).HasColumnType("datetime");
            entity.Property(e => e.DateResolved).HasColumnType("datetime");
            entity.Property(e => e.StatusRec).HasDefaultValue(1);

            entity.HasOne(d => d.ClientRecNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ClientRec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Client");

            entity.HasOne(d => d.CustomerRecNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.CustomerRec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Customer");

            entity.HasOne(d => d.StatusRecNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.StatusRec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_TicketStatus");
        });

        modelBuilder.Entity<TicketStatus>(entity =>
        {
            entity.HasKey(e => e.StatusRec);

            entity.ToTable("TicketStatus");

            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<TicketTime>(entity =>
        {
            entity.HasKey(e => e.TimeRec);

            entity.ToTable("TicketTime");

            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.TicketRecNavigation).WithMany(p => p.TicketTimes)
                .HasForeignKey(d => d.TicketRec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TicketTime_Ticket");
        });

        modelBuilder.Entity<VwOpenTicket>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_OpenTickets");

            entity.Property(e => e.ClientName)
                .HasMaxLength(50)
                .HasColumnName("Client_Name");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(50)
                .HasColumnName("Customer_Name");
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<VwTicketTime>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_TicketTime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
