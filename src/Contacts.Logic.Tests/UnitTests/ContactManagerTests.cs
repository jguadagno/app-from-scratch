using Contacts.Data;
using Contacts.Domain.Models;
using Microsoft.Extensions.Configuration;

namespace Contacts.Logic.Tests.UnitTests;

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
    public async Task ValidationForGetContacts_FirstNameIsNullOrEmpty_ShouldThrowAnException()
    {
        // Arrange
        var contactManager = new ContactManager(new ContactRepository(_configuration));
        
        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => contactManager.GetContactsAsync(string.Empty, "lastName"));
        
        // Assert
        Assert.Equal("firstName", exception.ParamName);
    }
    
    [Fact]
    public async Task ValidationForGetContacts_LastNameIsNullOrEmpty_ShouldThrowAnException()
    {
        // Arrange
        var contactManager = new ContactManager(new ContactRepository(_configuration));

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => contactManager.GetContactsAsync("firstName", string.Empty));

        // Assert
        Assert.Equal("lastName", exception.ParamName);
    }
    
    [Fact]
    public async Task SaveContact_WithANullContact_ShouldThrowException()
    {
        // Arrange 
        var contactManager = new ContactManager(new ContactRepository(_configuration));

        // Act
        ArgumentNullException ex = await Assert.ThrowsAsync<ArgumentNullException>(() => contactManager.SaveContactAsync(null));

        // Assert
        Assert.Equal("contact", ex.ParamName);
        Assert.Equal("Contact is a required field (Parameter 'contact')", ex.Message);
    }
    
    [Fact]
    public async Task SaveContact_WithEmptyNullFirstName_ShouldThrowException()
    {
        // Arrange 
        var contactManager = new ContactManager(new ContactRepository(_configuration));
        var contact = new Contact {FirstName = "", LastName = "lastName", EmailAddress = "emailAddress"};

        // Act
        ArgumentNullException ex = await Assert.ThrowsAsync<ArgumentNullException>(() => contactManager.SaveContactAsync(contact));

        // Assert
        Assert.Equal("FirstName", ex.ParamName);
        Assert.Equal("FirstName is a required field (Parameter 'FirstName')", ex.Message);
    }

    [Fact]
    public async Task SaveContact_WithEmptyNullLastName_ShouldThrowException()
    {
        // Arrange 
        var contactManager = new ContactManager(new ContactRepository(_configuration));
        var contact = new Contact {FirstName = "FirstName", LastName = "", EmailAddress = "emailAddress"};

        // Act
        ArgumentNullException ex = await Assert.ThrowsAsync<ArgumentNullException>(() => contactManager.SaveContactAsync(contact));

        // Assert
        Assert.Equal("LastName", ex.ParamName);
        Assert.Equal("LastName is a required field (Parameter 'LastName')", ex.Message);
    }
    
    [Fact]
    public async Task SaveContact_WithEmptyNullEmailAddress_ShouldThrowException()
    {
        // Arrange 
        var contactManager = new ContactManager(new ContactRepository(_configuration));
        var contact = new Contact {FirstName = "FirstName", LastName = "lastName", EmailAddress = ""};

        // Act
        ArgumentNullException ex = await Assert.ThrowsAsync<ArgumentNullException>(() => contactManager.SaveContactAsync(contact));

        // Assert
        Assert.Equal("EmailAddress", ex.ParamName);
        Assert.Equal("EmailAddress is a required field (Parameter 'EmailAddress')", ex.Message);
    }
    
    [Fact]
    public async Task SaveContact_WithBirthdayInFuture_ShouldThrowException()
    {
        // Arrange 
        var contactManager = new ContactManager(new ContactRepository(_configuration));

        var futureDate = new DateTime(2030, 12, 31, 23, 59, 59);

        var contact = new Contact
        {
            FirstName = "Joseph",
            LastName = "Guadagno",
            EmailAddress = "jguadagno@hotmail.com",
            Birthday = futureDate
        };

        // Act
        ArgumentOutOfRangeException ex = await
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => contactManager.SaveContactAsync(contact));

        // Assert
        Assert.Equal("Birthday", ex.ParamName);
        Assert.Equal(futureDate, ex.ActualValue);
        Assert.StartsWith("The birthday can not be in the future", ex.Message);
    }
    
    [Fact]
    public async Task SaveContact_WithAnniversaryBeforeBirthdayInFuture_ShouldThrowException()
    {
        // Arrange 
        var contactManager = new ContactManager(new ContactRepository(_configuration));

        var contact = new Contact
        {
            FirstName = "Joseph",
            LastName = "Guadagno",
            EmailAddress = "jguadagno@hotmail.com",
            Birthday = DateTime.Now,
            Anniversary = DateTime.Now.AddDays(-1)
        };

        // Act
        ArgumentOutOfRangeException ex = await
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => contactManager.SaveContactAsync(contact));

        // Assert
        Assert.Equal("Anniversary", ex.ParamName);
        Assert.Equal(contact.Anniversary, ex.ActualValue);
        Assert.StartsWith("The anniversary can not be earlier than the birthday.", ex.Message);
    }
}