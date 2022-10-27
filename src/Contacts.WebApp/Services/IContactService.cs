using Contacts.Domain.Models;

namespace Contacts.WebApp.Services;

public interface IContactService
{
    Task<List<Contact>?> GetContactsAsync();
    Task<Contact?> GetContactAsync(int id);
    Task<List<Contact>?> GetContactsByNameAsync(string firstName, string lastName);
    Task<Contact?> SaveContactAsync(Contact? contact);
    Task<bool> DeleteContactAsync(Contact contact);
    Task<bool> DeleteContactAsync(int contactId);
    Task<Phone?> GetContactPhoneAsync(int contactId, int phoneId);
    Task<List<Phone>?> GetContactPhonesAsync(int contactId);
    Task<Address?> GetContactAddressAsync(int contactId, int addressId);
    Task<List<Address>?> GetContactAddressesAsync(int contactId);
}