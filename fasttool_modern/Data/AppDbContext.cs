using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.EntityFrameworkCore;


public class AppDbContext : DbContext
{
    

    public DbSet<Action> Actions { get; set; }
    public DbSet<ButtonData> ButtonDatas { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<Profile> Profiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        string dbPath = Path.Combine("C:\\Users\\Kacper\\Desktop\\TestDB", "a1.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");


    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Action>()
        .HasOne(a => a.ButtonData)          // Jedna akcja ma jeden ButtonData
        .WithMany(b => b.Actions)           // Jeden ButtonData może mieć wiele akcji
        .HasForeignKey(a => new { a.ButtonID, a.ProfileID });  // Klucz obcy składający się z ButtonID i ProfileID

        // Pozostałe konfiguracje
        modelBuilder.Entity<ButtonData>()
            .HasKey(b => new { b.ButtonID, b.ProfileID });  // Złożony klucz dla ButtonData

        modelBuilder.Entity<Device>()
            .HasKey(d => d.DeviceID);

        modelBuilder.Entity<Profile>()
            .HasKey(p => p.ProfileID);

        modelBuilder.Entity<Action>()
            .Property(a => a.ActionID)
            .HasMaxLength(16);

        modelBuilder.Entity<ButtonData>()
            .Property(b => b.ButtonID)
            .HasMaxLength(16);

        modelBuilder.Entity<Device>()
            .Property(d => d.DeviceID)
            .HasMaxLength(16);

        modelBuilder.Entity<Profile>()
            .Property(p => p.ProfileID)
            .HasMaxLength(16);
    }
}


