using Contacts.Domain.Models;

namespace Contacts.Domain.Interfaces;

public interface IContactRepository
{
    Task<Contact> GetContactAsync(int contactId);
    Task<List<Contact>> GetContactsAsync();
    Task<List<Contact>> GetContactsAsync(string firstName, string lastName);
    Task<Contact?> SaveContactAsync(Contact contact);
    Task<bool> DeleteContactAsync(int contactId);
    Task<bool> DeleteContactAsync(Contact contact);
    Task<List<Phone>> GetContactPhonesAsync(int contactId);
    Task<Phone> GetContactPhoneAsync(int contactId, int phoneId);
    Task<List<Address>> GetContactAddressesAsync(int contactId);
    Task<Address> GetContactAddressAsync(int contactId, int addressId);
}