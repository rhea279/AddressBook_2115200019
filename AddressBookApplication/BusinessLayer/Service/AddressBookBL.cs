using AutoMapper;
using BusinessLayer.Interface;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class AddressBookBL : IAddressBookBL
    {
        private readonly IAddressBookRL _addressBookRL;
        private readonly IMapper _mapper;
        public AddressBookBL(IAddressBookRL addressBookRL, IMapper mapper)
        {
            _addressBookRL = addressBookRL;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AddressBookEntry>> GetAllContacts()
        {
            return await _addressBookRL.GetAllContacts();
        }
        public async Task<AddressBookEntry> GetContactById(int id)
        {
            return await _addressBookRL.GetContactById(id);
        }
        public async Task<AddressBookDTO> AddContact(AddressBookDTO contactDTO)
        {
            var contact = _mapper.Map<AddressBookEntry>(contactDTO);
            var createdContact = await _addressBookRL.AddContact(contact); // Save to DB

            return _mapper.Map<AddressBookDTO>(createdContact); // ✅ Return DTO instead of entity
        }


        public async Task<bool> UpdateContact(AddressBookDTO contactDTO)
        {
            var existingContact = await _addressBookRL.GetContactById(contactDTO.Id);
            if (existingContact == null)
            {
                return false;
            }

            _mapper.Map(contactDTO, existingContact); 
            await _addressBookRL.UpdateContact(existingContact);
            return true;
        }


        public async Task<bool> DeleteContact(int id)
        {
            return await _addressBookRL.DeleteContact(id);
        }
    }
}
