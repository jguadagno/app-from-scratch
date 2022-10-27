using AutoMapper;

namespace Contacts.Data;

public class ContactProfile: Profile
{
    public ContactProfile()
    {
        CreateMap<Domain.Models.Contact, Models.Contact>();
        CreateMap<Domain.Models.Address, Models.Address>();
        CreateMap<Domain.Models.AddressType, Models.AddressType>();
        CreateMap<Domain.Models.Phone, Models.Phone>();
        CreateMap<Domain.Models.PhoneType, Models.PhoneType>();
            
        CreateMap<Models.Contact, Domain.Models.Contact>();
        CreateMap<Models.Address, Domain.Models.Address>();
        CreateMap<Models.AddressType, Domain.Models.AddressType>();
        CreateMap<Models.Phone, Domain.Models.Phone>();
        CreateMap<Models.PhoneType, Domain.Models.PhoneType>();
    }
}