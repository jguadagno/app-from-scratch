using AutoMapper;
using Contacts.Domain;
using Contacts.Domain.Interfaces;
using Contacts.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Contacts.Data;

public class ContactRepository : IContactRepository
{
    private readonly ContactContext _contactContext;
    private readonly Mapper _mapper;

    public ContactRepository(IConfiguration configuration)
    {
        _contactContext = new ContactContext(configuration);
        var mapperConfiguration = new MapperConfiguration(cfg => { cfg.AddProfile<ContactProfile>(); });
        _mapper = new Mapper(mapperConfiguration);
    }

    public async Task<Contact> GetContactAsync(int contactId)
    {
        var dbContact = await _contactContext.Contacts
            .FirstOrDefaultAsync(c => c.ContactId == contactId);
        var contact = _mapper.Map<Contact>(dbContact);
        return contact;
    }

    public async Task<List<Contact>> GetContactsAsync()
    {
        var contacts = await _contactContext.Contacts.ToListAsync();
        return _mapper.Map<List<Contact>>(contacts);
    }

    public async Task<List<Contact>> GetContactsAsync(string firstName, string lastName)
    {
        ValidationForGetContacts(firstName, lastName);

        var dbContact = await _contactContext.Contacts
            .Where(contact => contact.LastName == lastName && contact.FirstName == firstName).ToListAsync();
        return _mapper.Map<List<Contact>>(dbContact);
    }

    private static void ValidationForGetContacts(string firstName, string lastName)
    {
        if (string.IsNullOrEmpty(lastName))
        {
            throw new ArgumentNullException(nameof(lastName), "LastName is a required field");
        }

        if (string.IsNullOrEmpty(firstName))
        {
            throw new ArgumentNullException(nameof(firstName), "FirstName is a required field");
        }
    }

    public async Task<Contact?> SaveContactAsync(Contact contact)
    {
        var dbContact = _mapper.Map<Models.Contact>(contact);

        _contactContext.Entry(dbContact).State =
            dbContact.ContactId == 0 ? EntityState.Added : EntityState.Modified;

        var wasSaved = await _contactContext.SaveChangesAsync() != 0;
        if (wasSaved)
        {
            contact.ContactId = dbContact.ContactId;
            return contact;
        }

        return null;
    }

    public async Task<bool> DeleteContactAsync(int contactId)
    {
        var contact = await _contactContext.Contacts
            .Include(c => c.Addresses)
            .Include(c => c.Phones)
            .FirstOrDefaultAsync(c => c.ContactId == contactId);

        if (contact == null)
        {
            return false;
        }

        _contactContext.Contacts.Remove(contact);
        foreach (var contactAddress in contact.Addresses)
        {
            _contactContext.Addresses.Remove(contactAddress);
        }

        foreach (var contactPhone in contact.Phones)
        {
            _contactContext.Phones.Remove(contactPhone);
        }

        return await _contactContext.SaveChangesAsync() != 0;
    }

    public async Task<bool> DeleteContactAsync(Contact contact)
    {
        return ValidateDeleteContact(contact) && await DeleteContactAsync(contact.ContactId);
    }

    private static bool ValidateDeleteContact(Contact contact)
    {
        // You may want to add some validation here
        return true;
    }

    public async Task<List<Phone>> GetContactPhonesAsync(int contactId)
    {
        var dbPhones = await _contactContext.Phones
            .Where(p => p.Contact.ContactId == contactId).ToListAsync();

        var phones = _mapper.Map<List<Phone>>(dbPhones);
        return phones;
    }

    public async Task<Phone> GetContactPhoneAsync(int contactId, int phoneId)
    {
        var dbPhone = await _contactContext.Phones
            .FirstOrDefaultAsync(p => p.Contact.ContactId == contactId && p.PhoneId == phoneId);
        var phone = _mapper.Map<Phone>(dbPhone);
        return phone;
    }

    public async Task<List<Address>> GetContactAddressesAsync(int contactId)
    {
        var dbAddresses = await _contactContext.Addresses
            .Where(a => a.Contact.ContactId == contactId).ToListAsync();

        var addresses = _mapper.Map<List<Address>>(dbAddresses);
        return addresses;
    }

    public async Task<Address> GetContactAddressAsync(int contactId, int addressId)
    {
        var dbAddress = await _contactContext.Addresses
            .FirstOrDefaultAsync(a => a.Contact.ContactId == contactId && a.AddressId == addressId);
        var address = _mapper.Map<Address>(dbAddress);
        return address;
    }
}
