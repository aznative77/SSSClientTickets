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

    public virtual DbSet<Site> Sites { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketStatus> TicketStatuses { get; set; }

    public virtual DbSet<TicketTime> TicketTimes { get; set; }

    public virtual DbSet<VwOpenTicket> VwOpenTickets { get; set; }

    public virtual DbSet<VwTicketTime> VwTicketTimes { get; set; }

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
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("Customer_Email");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("Customer_Phone");
            entity.Property(e => e.Mobile)
                .HasMaxLength(20)
                .HasColumnName("Customer_Mobile");

            entity.HasOne(d => d.ClientRecNavigation).WithMany(p => p.Customers)
                .HasForeignKey(d => d.ClientRec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Customer_Client");
        });

        modelBuilder.Entity<Site>(entity =>
        {
            entity.HasKey(e => e.SiteRec);

            entity.ToTable("Site");

            entity.Property(e => e.SiteAddress1)
                .HasMaxLength(50)
                .HasColumnName("Site_Address1");
            entity.Property(e => e.SiteAddress2)
                .HasMaxLength(50)
                .HasColumnName("Site_Address2");
            entity.Property(e => e.SiteCity)
                .HasMaxLength(50)
                .HasColumnName("Site_City");
            entity.Property(e => e.SiteName)
                .HasMaxLength(255)
                .HasColumnName("Site_Name");
            entity.Property(e => e.SiteState)
                .HasMaxLength(2)
                .HasColumnName("Site_State");
            entity.Property(e => e.SiteZip)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("Site_Zip");

            entity.HasOne(d => d.ClientRecNavigation).WithMany(p => p.Sites)
                .HasForeignKey(d => d.ClientRec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Site_Client");
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

            entity.HasOne(d => d.SiteRecNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.SiteRec)
                .HasConstraintName("FK_Ticket_Site");

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

            // Seed initial ticket statuses
            entity.HasData(
                new TicketStatus { StatusRec = 1, Status = "Open" },
                new TicketStatus { StatusRec = 2, Status = "In Progress" },
                new TicketStatus { StatusRec = 3, Status = "Waiting for Client" },
                new TicketStatus { StatusRec = 4, Status = "Resolved" }
            );
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
