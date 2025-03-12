
using ModelLayer.Model;

namespace BusinessLayer.Interface
{
    public interface IAddressBookBL
    {
        Task<IEnumerable<AddressBookEntry>> GetAllContacts();
        Task<AddressBookEntry> GetContactById(int id);
        Task AddContact(AddressBookEntry contact);
        Task UpdateContact(AddressBookEntry contact);
        Task DeleteContact(int id);
    }
}
