using Contacts.Data;
using Microsoft.Extensions.Configuration;

namespace Contacts.Logic.Tests.IntegrationTests;

public class ContactManagerTests
{

    private readonly IConfiguration _configuration;
    public ContactManagerTests()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: false)
            .AddEnvironmentVariables()
            .Build();    
    }
    
    [Fact]
    public async void GetContactsAsync_ShouldReturnAllContacts()
    {
        var contactManager = new ContactManager(new ContactRepository(_configuration));
        var contacts = await contactManager.GetContactsAsync();
        Assert.NotNull(contacts);
        Assert.True(contacts.Any());
    }
}