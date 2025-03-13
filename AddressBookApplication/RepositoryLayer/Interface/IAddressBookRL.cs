using ModelLayer.Model;
namespace RepositoryLayer.Interface
{
    public interface IAddressBookRL
    {
        Task<IEnumerable<AddressBookEntry>> GetAllContacts();
        Task<AddressBookEntry> GetContactById( int id );
        Task<AddressBookEntry> AddContact(AddressBookEntry contact);
        Task UpdateContact(AddressBookEntry contact);
        Task<bool> DeleteContact(int id);
    }
}
