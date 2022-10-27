using Contacts.Domain.Interfaces;
using Contacts.Domain.Models;
using Moq;
using Range = Moq.Range;

namespace Contacts.Logic.Tests.MockTests
{
    public class ContactManagerTests
    {
        [Fact]
        public async Task GetContact_WithAnInvalidId_ShouldReturnNull()
        {
            // Arrange 
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                contactRepository.GetContactAsync(It.IsInRange(int.MinValue, 0, Range.Inclusive))
            ).ReturnsAsync(() => null);

            var contactManager = new ContactManager(mockContactRepository.Object);

            // Act
            var contact = await contactManager.GetContactAsync(-1); // Any number less than zero

            // Assert
            Assert.Null(contact);
        }

        [Fact]
        public async Task GetContact_WithAValidId_ShouldReturnContact()
        {
            // Arrange 
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                contactRepository.GetContactAsync(It.IsInRange(1, int.MaxValue, Range.Inclusive))
            ).ReturnsAsync((int contactId) => new Contact
            {
                ContactId = contactId
            });

            var contactManager = new ContactManager(mockContactRepository.Object);
            const int requestedContactId = 1;

            // Act
            // Assumes that a contact record exists with the ContactId of 1
            var contact = await contactManager.GetContactAsync(requestedContactId);

            // Assert
            Assert.NotNull(contact);
            Assert.Equal(requestedContactId, contact.ContactId);
        }

        [Fact]
        public async Task GetContacts_ShouldReturnLists()
        {
            // Arrange
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                contactRepository.GetContactsAsync()
            ).ReturnsAsync(new List<Contact>
            {
                new Contact { ContactId = 1 }, new Contact { ContactId = 2 }
            });

            var contactManager = new ContactManager(mockContactRepository.Object);

            // Act
            var contacts = await contactManager.GetContactsAsync();

            // Assert
            Assert.NotNull(contacts);
            Assert.Equal(2, contacts.Count);
        }

        [Fact]
        public async Task GetContacts_WithNullFirstName_ShouldThrowException()
        {
            // Arrange 
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                contactRepository.GetContactsAsync(null, It.IsAny<string>()));

            var contactManager = new ContactManager(mockContactRepository.Object);
            // Act
            ArgumentNullException ex = await
                Assert.ThrowsAsync<ArgumentNullException>(() => contactManager.GetContactsAsync(null, "Guadagno"));

            // Assert
            Assert.Equal("firstName", ex.ParamName);
            Assert.Equal("FirstName is a required field (Parameter 'firstName')", ex.Message);
        }

        [Fact]
        public async Task GetContacts_WithNullLastName_ShouldThrowException()
        {
            // Arrange 
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                contactRepository.GetContactsAsync(It.IsAny<string>(), null));

            var contactManager = new ContactManager(mockContactRepository.Object);

            // Act
            ArgumentNullException ex =
                await Assert.ThrowsAsync<ArgumentNullException>(() => contactManager.GetContactsAsync("Joseph", null));

            // Assert
            Assert.Equal("lastName", ex.ParamName);
            Assert.Equal("LastName is a required field (Parameter 'lastName')", ex.Message);
        }

        [Fact]
        public async Task GetContacts_WithValidParameters_ShouldReturnLists()
        {
            // Arrange 
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                contactRepository.GetContactsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(
                (string firstName, string lastName) => new List<Contact>
                {
                    new Contact { ContactId = 1, FirstName = firstName, LastName = lastName }
                });

            var contactManager = new ContactManager(mockContactRepository.Object);
            const string requestedFirstName = "Joseph";
            const string requestedLastname = "Guadagno";

            // Act
            var contacts = await contactManager.GetContactsAsync(requestedFirstName, requestedLastname);

            // Assert
            Assert.NotNull(contacts);
            Assert.True(contacts.Count > 0);
            Assert.Equal(requestedFirstName, contacts[0].FirstName);
            Assert.Equal(requestedLastname, contacts[0].LastName);
        }

        [Fact]
        public async Task SaveContact_WithANullContact_ShouldThrowException()
        {
            // Arrange 
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                contactRepository.SaveContactAsync(null));
            var contactManager = new ContactManager(mockContactRepository.Object);

            // Act
            ArgumentNullException ex =
                await Assert.ThrowsAsync<ArgumentNullException>(() => contactManager.SaveContactAsync(null));

            // Assert
            Assert.Equal("contact", ex.ParamName);
            Assert.Equal("Contact is a required field (Parameter 'contact')", ex.Message);
        }

        [Fact]
        public async Task SaveContact_WithNullFirstName_ShouldThrowException()
        {
            // Arrange 
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                contactRepository.SaveContactAsync(It.IsAny<Contact>()));
            var contactManager = new ContactManager(mockContactRepository.Object);

            var contact = new Contact();

            // Act
            ArgumentNullException ex =
                await Assert.ThrowsAsync<ArgumentNullException>(() => contactManager.SaveContactAsync(contact));

            // Assert
            Assert.Equal("FirstName", ex.ParamName);
            Assert.Equal("FirstName is a required field (Parameter 'FirstName')", ex.Message);
        }

        [Fact]
        public async Task SaveContact_WithNullLastName_ShouldThrowException()
        {
            // Arrange 
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                contactRepository.SaveContactAsync(It.IsAny<Contact>()));
            var contactManager = new ContactManager(mockContactRepository.Object);

            var contact = new Contact
            {
                FirstName = "Joseph"
            };

            // Act
            ArgumentNullException ex =
                await Assert.ThrowsAsync<ArgumentNullException>(() => contactManager.SaveContactAsync(contact));

            // Assert
            Assert.Equal("LastName", ex.ParamName);
            Assert.Equal("LastName is a required field (Parameter 'LastName')", ex.Message);
        }

        [Fact]
        public async Task SaveContact_WithNullEmailAddress_ShouldThrowException()
        {
            // Arrange 
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                contactRepository.SaveContactAsync(It.IsAny<Contact>()));
            var contactManager = new ContactManager(mockContactRepository.Object);

            var contact = new Contact
            {
                FirstName = "Joseph",
                LastName = "Guadagno"
            };

            // Act
            ArgumentNullException ex =
                await Assert.ThrowsAsync<ArgumentNullException>(() => contactManager.SaveContactAsync(contact));

            // Assert
            Assert.Equal("EmailAddress", ex.ParamName);
            Assert.Equal("EmailAddress is a required field (Parameter 'EmailAddress')", ex.Message);
        }

        [Fact]
        public async Task SaveContact_WithBirthdayInFuture_ShouldThrowException()
        {
            // Arrange 
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                contactRepository.SaveContactAsync(It.IsAny<Contact>()));
            var contactManager = new ContactManager(mockContactRepository.Object);

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
        public async Task SaveContact_WithAnniversaryInFuture_ShouldThrowException()
        {
            // Arrange 
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                contactRepository.SaveContactAsync(It.IsAny<Contact>()));
            var contactManager = new ContactManager(mockContactRepository.Object);

            var futureDate = new DateTime(2030, 12, 31, 23, 59, 59);
            var contact = new Contact
            {
                FirstName = "Joseph",
                LastName = "Guadagno",
                EmailAddress = "jguadagno@hotmail.com",
                Birthday = DateTime.Now.AddDays(-1),
                Anniversary = futureDate
            };

            // Act
            ArgumentOutOfRangeException ex = await
                Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => contactManager.SaveContactAsync(contact));

            // Assert
            Assert.Equal("Anniversary", ex.ParamName);
            Assert.Equal(futureDate, ex.ActualValue);
            Assert.StartsWith("The anniversary can not be in the future", ex.Message);
        }

        [Fact]
        public async Task SaveContact_WithAnniversaryBeforeBirthday_ShouldThrowException()
        {
            // Arrange 
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                contactRepository.SaveContactAsync(It.IsAny<Contact>()));
            var contactManager = new ContactManager(mockContactRepository.Object);

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

        [Fact]
        public async Task SaveContact_WithValidContact_ShouldReturnTrue()
        {
            // Arrange 
            var contact = new Contact
            {
                ContactId = 1,
                FirstName = "Joseph",
                LastName = "Guadagno",
                EmailAddress = "jguadagno@hotmail.com",
                Birthday = DateTime.Now.AddDays(-10),
                Anniversary = DateTime.Now.AddDays(-1)
            };

            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository => contactRepository.SaveContactAsync(It.IsAny<Contact>()))
                .ReturnsAsync(() => contact);

            var contactManager = new ContactManager(mockContactRepository.Object);

            // Act
            var savedContact = await contactManager.SaveContactAsync(contact);

            // Assert
            Assert.NotNull(savedContact);
            Assert.Equal(contact.ContactId, savedContact.ContactId);
        }

        [Fact]
        public async Task DeleteContact_WithInvalidContactId_ShouldReturnFalse()
        {
            // Arrange
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                    contactRepository.DeleteContactAsync(It.IsInRange(int.MinValue, 0, Range.Inclusive)))
                .ReturnsAsync(false);
            var contactManager = new ContactManager(mockContactRepository.Object);

            // Act
            // This assumes that there is no record with the id of -1
            var wasDeleted = await contactManager.DeleteContactAsync(-1);

            // Assert
            Assert.False(wasDeleted);
        }

        [Fact]
        public async Task DeleteContact_WithNullContact_ShouldReturnFalse()
        {
            // Arrange
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                contactRepository.DeleteContactAsync(It.IsAny<Contact>())).ReturnsAsync(false);
            var contactManager = new ContactManager(mockContactRepository.Object);

            // Act
            // This assumes that there is no record with the id of -1
            var wasDeleted = await contactManager.DeleteContactAsync(null);

            // Assert
            Assert.False(wasDeleted);
        }

        [Fact]
        public async Task DeleteContact_WithExistingContact_ShouldReturnTrue()
        {
            // Arrange
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                contactRepository.DeleteContactAsync(It.IsAny<Contact>())).ReturnsAsync(true);
            var contactManager = new ContactManager(mockContactRepository.Object);

            // Create a fake contact
            var contact = new Contact
            {
                FirstName = "TestUserFirstName",
                LastName = "TestUserLastName",
                EmailAddress = "TestUser@example.com",
                Birthday = DateTime.Now
            };

            // Act
            var wasDeleted = await contactManager.DeleteContactAsync(contact);

            // Assert
            Assert.True(wasDeleted);
        }

        // GetContactPhones

        [Fact]
        public async Task GetContactPhones_WithValidContactId_ShouldReturnAListOfPhones()
        {
            // Arrange
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                contactRepository.GetContactPhonesAsync(It.IsAny<int>())).ReturnsAsync(new List<Phone>());
            var contactManager = new ContactManager(mockContactRepository.Object);

            // Act
            var phones = await contactManager.GetContactPhonesAsync(1);

            // Assert
            Assert.NotNull(phones);

        }

        // GetContactPhone
        [Fact]
        public async Task GetContactPhone_WithValidContactIdAndPhoneId_ShouldReturnPhone()
        {
            // Arrange
            var mockContactRepository = new Mock<IContactRepository>();
            var setup = mockContactRepository.Setup(contactRepository =>
                    contactRepository.GetContactPhoneAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Phone());
            var contactManager = new ContactManager(mockContactRepository.Object);

            // Act
            var phone = await contactManager.GetContactPhoneAsync(1, 1);

            // Assert
            Assert.NotNull(phone);

        }

        // GetContactAddress
        [Fact]
        public async Task GetContactAddress_WithValidContactIdAndAddressId_ShouldReturnAddress()
        {
            // Arrange
            var mockContactRepository = new Mock<IContactRepository>();
            var setup = mockContactRepository.Setup(contactRepository =>
                    contactRepository.GetContactAddressAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Address());
            var contactManager = new ContactManager(mockContactRepository.Object);

            // Act
            var address = await contactManager.GetContactAddressAsync(1, 1);

            // Assert
            Assert.NotNull(address);

        }

        // GetContactAddresses
        [Fact]
        public async Task GetContactAddresses_WithValidContactId_ShouldReturnAListOfAddresses()
        {
            // Arrange
            var mockContactRepository = new Mock<IContactRepository>();
            mockContactRepository.Setup(contactRepository =>
                contactRepository.GetContactAddressesAsync(It.IsAny<int>())).ReturnsAsync(new List<Address>());
            var contactManager = new ContactManager(mockContactRepository.Object);

            // Act
            var addresses = await contactManager.GetContactAddressesAsync(1);

            // Assert
            Assert.NotNull(addresses);
        }
    }
}

