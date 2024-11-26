using Microsoft.EntityFrameworkCore;


namespace Persistance;
public class AppDbContext : DbContext
{
    

    public DbSet<Action> Actions { get; set; }
    public DbSet<ButtonData> ButtonDatas { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<Profile> Profiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var localFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        string dbPath = System.IO.Path.Combine(localFolder, "a1.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Action>()
        .HasOne(a => a.ButtonData)        
        .WithMany(b => b.Actions)         
        .HasForeignKey(a => new { a.ButtonID, a.ProfileID }); 

        modelBuilder.Entity<ButtonData>()
            .HasKey(b => new { b.ButtonID, b.ProfileID });  

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

        modelBuilder.Entity<ButtonData>()
            .Property(b => b.DeviceID)
            .IsRequired(false);
        modelBuilder.Entity<ButtonData>()
            .Property(b => b.Image)
            .IsRequired(false);
        modelBuilder.Entity<ButtonData>()
            .Property(b => b.Color)
            .IsRequired(false);

        modelBuilder.Entity<Device>()
            .Property(d => d.DeviceID)
            .HasMaxLength(16);

        modelBuilder.Entity<Profile>()
            .Property(p => p.ProfileID)
            .HasMaxLength(16);
    }

}


