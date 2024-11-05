using Microsoft.EntityFrameworkCore;
using EmergencyCallServer.utils;
public class VolunteersDB : DbContext
{
    public DbSet<Volunteer> Volunteers { get; set; }

    public VolunteersDB(DbContextOptions<VolunteersDB> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Replace with your actual connection string
        optionsBuilder.UseSqlServer("workstation id=Volunteers.mssql.somee.com;packet size=4096;user id=DanGolan_SQLLogin_1;pwd=p6fy5c6dlc;data source=Volunteers.mssql.somee.com;persist security info=False;initial catalog=Volunteers;TrustServerCertificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Volunteer>().ToTable("Volunteers"); // Maps the Volunteer class to the Volunteers table
    }
}
