using Contacts.Domain.Interfaces;
using Contacts.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Api.Controllers
{
    /// <summary>
    /// The contacts endpoints provide the functionality to maintain our contacts.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactManager _contactManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contactManager">A <see cref="IContactManager"/></param>
        public ContactsController(IContactManager contactManager)
        {
            _contactManager = contactManager;
        }

        /// <summary>
        /// List all of the contacts currently available
        /// </summary>
        /// <returns>A List of <see cref="Contact"/></returns>
        /// <response code="200">Returns Ok</response>
        /// <response code="400">If requests is poorly formatted</response>            
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            return await _contactManager.GetContactsAsync();
        }

        /// <summary>
        /// Gets a specific contact from the contact manager
        /// </summary>
        /// <param name="id">The primary identifier of the contact</param>
        /// <returns>A <see cref="Contact"/></returns>
        /// <response code="200">Ok</response>
        /// <response code="400">If the request is poorly formatted</response>            
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}")]
        [ActionName(nameof(GetContactAsync))]
        public async Task<ActionResult<Contact>> GetContactAsync(int id)
        {
            var contact = await _contactManager.GetContactAsync(id);
            if (contact is not null)
            {
                return contact;
            }

            return NotFound();
        }

        /// <summary>
        /// Adds a contact to the contact manager
        /// </summary>
        /// <param name="contact">A contact</param>
        /// <returns>The contact with the Url to view its details</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null or there are data violations</response>            
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Contact>> SaveContact(Contact contact)
        {
            var savedContact = await _contactManager.SaveContactAsync(contact);

            if (savedContact != null)
            {
                return CreatedAtAction(nameof(GetContactAsync),
                    new { id = contact.ContactId },
                    contact);
            }

            return Problem("Failed to insert the contact");
        }

        /// <summary>
        /// Deletes the specified contact
        /// </summary>
        /// <param name="id">The primary identifier for the contact</param>
        /// <returns></returns>
        /// <response code="200">If the item was deleted</response>
        /// <response code="400">If the request is poorly formatted</response>            
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> DeleteContact(int id)
        {
            var wasDeleted = await _contactManager.DeleteContactAsync(id);
            if (wasDeleted)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Searches for a contact
        /// </summary>
        /// <param name="firstname">The first name of the contact to search for</param>
        /// <param name="lastname">The last name of the contact to search for</param>
        /// <returns>A list of 0 or more contacts that meet the criteria</returns>
        /// <response code="200">Ok</response>
        /// <response code="400">If the request is poorly formatted</response>            
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<List<Contact>> GetContacts([FromQuery] string firstname, [FromQuery] string lastname)
        {
            return await _contactManager.GetContactsAsync(firstname, lastname);
        }

        /// <summary>
        /// Gets phone numbers for the contact
        /// </summary>
        /// <param name="id">The primary identifier of the contact</param>
        /// <returns>A list of <see cref="Phone"/></returns>
        /// <response code="200">Ok</response>
        /// <response code="400">If the request is poorly formatted</response>    
        [HttpGet("{id}/phones")]
        public async Task<List<Phone>> GetContactPhones(int id)
        {
            return await _contactManager.GetContactPhonesAsync(id);
        }

        /// <summary>
        /// Gets a specific phone for the specified contact
        /// </summary>
        /// <param name="id">The primary identifier of the contact</param>
        /// <param name="phoneId">The primary identifier of the phone number</param>
        /// <returns>A <see cref="Contact"/></returns>
        /// <response code="200">Ok</response>
        /// <response code="400">If the request is poorly formatted</response>    
        [HttpGet("{id}/phones/{phoneId}")]
        public async Task<ActionResult<Phone>> GetContactPhone(int id, int phoneId)
        {
            var phone = await _contactManager.GetContactPhoneAsync(id, phoneId);
            if (phone is not null)
            {
                return phone;
            }

            return NotFound();
        }

        /// <summary>
        /// Gets addresses for the contact
        /// </summary>
        /// <param name="id">The primary identifier of the contact</param>
        /// <returns>A list of <see cref="Address"/>es</returns>
        /// <response code="200">Ok</response>
        /// <response code="400">If the request is poorly formatted</response>    
        [HttpGet("{id}/addresses")]
        public async Task<List<Address>> GetContactAddresses(int id)
        {
            return await _contactManager.GetContactAddressesAsync(id);
        }

        /// <summary>
        /// Gets a specific phone for a specific contact\
        /// </summary>
        /// <param name="id">The primary identifier of the contact</param>
        /// <param name="addressId">The primary identifier of the address</param>
        /// <returns>A <see cref="Contact"/></returns>
        /// <response code="200">Ok</response>
        /// <response code="400">If the request is poorly formatted</response>    
        [HttpGet("{id}/addresses/{addressId}")]
        public async Task<ActionResult<Address>> GetContactAddressAsync(int id, int addressId)
        {
            var address = await _contactManager.GetContactAddressAsync(id, addressId);
            if (address is not null)
            {
                return address;
            }

            return NotFound();
        }
    }
}