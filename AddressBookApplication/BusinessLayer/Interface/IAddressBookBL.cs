
using ModelLayer.DTO;
using ModelLayer.Model;

namespace BusinessLayer.Interface
{
    public interface IAddressBookBL
    {
        Task<IEnumerable<AddressBookEntry>> GetAllContacts();
        Task<AddressBookEntry> GetContactById(int id);
        Task<AddressBookDTO> AddContact(AddressBookDTO contactDto);
        Task<bool> UpdateContact(AddressBookDTO contactDTO);
        Task<bool> DeleteContact(int id);
    }
}
