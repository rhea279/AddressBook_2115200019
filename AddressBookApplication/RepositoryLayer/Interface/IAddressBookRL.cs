using ModelLayer.Model;
namespace RepositoryLayer.Interface
{
    public interface IAddressBookRL
    {
        Task<IEnumerable<AddressBookEntry>> GetAllContacts();
        Task<AddressBookEntry> GetContactById( int id );
        Task AddContact(AddressBookEntry contact);
        Task UpdateContact(AddressBookEntry contact);
        Task DeleteContact(int id);
    }
}
