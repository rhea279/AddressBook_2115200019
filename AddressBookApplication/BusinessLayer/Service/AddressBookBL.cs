
using BusinessLayer.Interface;
using ModelLayer.Model;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class AddressBookBL:IAddressBookBL
    {
        private readonly IAddressBookRL _addressBookRL;
        public AddressBookBL(IAddressBookRL addressBookRL)
        {
            _addressBookRL = addressBookRL;
        }

        public async Task<IEnumerable<AddressBookEntry>> GetAllContacts()
        {
            return await _addressBookRL.GetAllContacts();
        }
        public async Task<AddressBookEntry> GetContactById(int id)
        {
            return await _addressBookRL.GetContactById(id);
        }
        public async Task AddContact(AddressBookEntry contact)
        {
            await _addressBookRL.AddContact(contact);
        }
        public async Task UpdateContact(AddressBookEntry contact)
        {
            await _addressBookRL.UpdateContact(contact);
        }
        public async Task DeleteContact(int id)
        {
            await _addressBookRL.DeleteContact(id);
        }
    }
}
