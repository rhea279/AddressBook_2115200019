using AutoMapper;
using ModelLayer.DTO;
using ModelLayer.Model;

namespace BusinessLayer.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<AddressBookEntry, AddressBookDTO>().ReverseMap();
        }
    }
}
