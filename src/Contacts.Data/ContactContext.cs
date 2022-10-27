using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Contacts.Data.Models;

namespace Contacts.Data;

public class ContactContext : DbContext
{
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Phone> Phones { get; set; }
    public DbSet<AddressType> AddressTypes { get; set; }
    public DbSet<PhoneType> PhoneTypes { get; set; }

    private readonly string _connectionString;

    public ContactContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer(_connectionString);
}