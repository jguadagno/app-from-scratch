using Contacts.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.WebApp.Controllers;

public class ContactController : Controller
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }

    public async Task<IActionResult> Index()
    {
        var contacts = await _contactService.GetContactsAsync();

        return View(contacts);
    }

    // Get One (Details)
    public async Task<IActionResult> Details(int id)
    {
        var contact = await _contactService.GetContactAsync(id);

        return View(contact);
    }
        
    public async Task<IActionResult> Edit(int id)
    {
        var contact = await _contactService.GetContactAsync(id);

        return View(contact);
    }

    [HttpPost]
    public async Task<RedirectToActionResult> Edit(Domain.Models.Contact contact)
    {
        var savedContact = await _contactService.SaveContactAsync(contact);
        return RedirectToAction("Details",
            savedContact is not null ? new { id = savedContact.ContactId } : new { id = contact.ContactId });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _contactService.DeleteContactAsync(id);

        if (result)
        {
            return RedirectToAction("Index");
        }

        return View();
    }
        
    public IActionResult Add()
    {
        return View(new Contacts.Domain.Models.Contact());
    }
        
    [HttpPost]
    public async Task<RedirectToActionResult> Add(Domain.Models.Contact contact)
    {
        var savedContact = await _contactService.SaveContactAsync(contact);
        return savedContact is not null
            ? RedirectToAction("Details", new { id = savedContact.ContactId })
            : RedirectToAction("Edit", new { id = contact.ContactId });
    }
}