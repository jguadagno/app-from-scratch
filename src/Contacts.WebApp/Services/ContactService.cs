using System.Net;
using System.Text;
using System.Text.Json;
using Contacts.Domain.Models;
using Contacts.WebApp.Models;

namespace Contacts.WebApp.Services;

public class ContactService : IContactService
{
    private readonly HttpClient _httpClient;
    private readonly Settings _settings;
    public ContactService(HttpClient httpClient, Settings settings)
    {
        _httpClient = httpClient;
        _settings = settings;
    }

    public async Task<List<Contact>?> GetContactsAsync()
    {
        var url = $"{_settings.ContactsApiUrl}contacts";
        return await ExecuteGetAsync<List<Contact>>(url);
    }
    
    public async Task<Contact?> GetContactAsync(int id)
    {
        var url = $"{_settings.ContactsApiUrl}contacts/{id}";
        return await ExecuteGetAsync<Contact>(url);
    }

    public async Task<List<Contact>?> GetContactsByNameAsync(string firstName, string lastName)
    {
        var url = $"{_settings.ContactsApiUrl}contacts/search?firstname={firstName}&lastname={lastName}";
        return await ExecuteGetAsync<List<Contact>?>(url);
    }
    
    public async Task<Contact?> SaveContactAsync(Contact? contact)
    {
        var url = $"{_settings.ContactsApiUrl}contacts/";
        var jsonRequest = JsonSerializer.Serialize(contact);
        var jsonContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, jsonContent);

        if (response.StatusCode != HttpStatusCode.Created)
            throw new HttpRequestException(
                $"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
            
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        contact = JsonSerializer.Deserialize<Contact>(content, options);
        return contact;
    }
    
    public async Task<bool> DeleteContactAsync(Contact contact)
    {
        return await DeleteContactAsync(contact.ContactId);
    }

    public async Task<bool> DeleteContactAsync(int contactId)
    {
        var url = $"{_settings.ContactsApiUrl}contacts/{contactId}";
        var response = await _httpClient.DeleteAsync(url);
        return response.StatusCode == HttpStatusCode.NoContent;
    }

    public async Task<Phone?> GetContactPhoneAsync(int contactId, int phoneId)
    {
        var url = $"{_settings.ContactsApiUrl}contacts/{contactId}/phones/{phoneId}";
        return await ExecuteGetAsync<Phone>(url);
    }

    public async Task<List<Phone>?> GetContactPhonesAsync(int contactId)
    {
        var url = $"{_settings.ContactsApiUrl}contacts/{contactId}/phones";
        return await ExecuteGetAsync<List<Phone>>(url);
    }
        
    public async Task<Address?> GetContactAddressAsync(int contactId, int addressId)
    {
        var url = $"{_settings.ContactsApiUrl}contacts/{contactId}/addresses/{addressId}";
        return await ExecuteGetAsync<Address>(url);
    }

    public async Task<List<Address>?> GetContactAddressesAsync(int contactId)
    {
        var url = $"{_settings.ContactsApiUrl}contacts/{contactId}/addresses";
        return await ExecuteGetAsync<List<Address>>(url);
    }
    private async Task<T?> ExecuteGetAsync<T>(string url)
    {
        var response = await _httpClient.GetAsync(url);
        if (response.StatusCode != HttpStatusCode.OK)
            throw new HttpRequestException(
                $"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
            
        // Parse the Results
        var content = await response.Content.ReadAsStringAsync();
                
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        var results = JsonSerializer.Deserialize<T>(content, options);

        return results;
    }
}