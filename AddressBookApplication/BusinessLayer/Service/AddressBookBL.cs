using AutoMapper;
using BusinessLayer.Interface;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BusinessLayer.Service
{
    public class AddressBookBL : IAddressBookBL
    {
        private readonly IAddressBookRL _addressBookRL;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public AddressBookBL(IAddressBookRL addressBookRL, IMapper mapper, IDistributedCache cache)
        {
            _addressBookRL = addressBookRL;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<AddressBookEntry>> GetAllContacts()
        {
            string cacheKey = "AllContacts";

            try
            {
                var cachedData = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    return JsonSerializer.Deserialize<List<AddressBookEntry>>(cachedData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis Error] Failed to retrieve cache: {ex.Message}");
            }

            var contacts = await _addressBookRL.GetAllContacts();

            try
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(contacts, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis Error] Failed to store cache: {ex.Message}");
            }

            return contacts;
        }

        public async Task<AddressBookEntry> GetContactById(int id)
        {
            string cacheKey = $"Contact_{id}";

            try
            {
                var cachedData = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    return JsonSerializer.Deserialize<AddressBookEntry>(cachedData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis Error] Failed to retrieve cache: {ex.Message}");
            }

            var contact = await _addressBookRL.GetContactById(id);
            if (contact == null) return null;

            try
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(contact), cacheOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis Error] Failed to store cache: {ex.Message}");
            }

            return contact;
        }

        public async Task<AddressBookDTO> AddContact(AddressBookDTO contactDTO)
        {
            var contact = _mapper.Map<AddressBookEntry>(contactDTO);
            var createdContact = await _addressBookRL.AddContact(contact);

            try
            {
                await _cache.RemoveAsync("AllContacts");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis Error] Failed to remove cache: {ex.Message}");
            }

            return _mapper.Map<AddressBookDTO>(createdContact);
        }

        public async Task<bool> UpdateContact(AddressBookDTO contactDTO)
        {
            var existingContact = await _addressBookRL.GetContactById(contactDTO.Id);
            if (existingContact == null) return false;

            _mapper.Map(contactDTO, existingContact);
            await _addressBookRL.UpdateContact(existingContact);

            try
            {
                string cacheKey = $"Contact_{contactDTO.Id}";
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(existingContact),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

                await _cache.RemoveAsync("AllContacts");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis Error] Failed to update cache: {ex.Message}");
            }

            return true;
        }

        public async Task<bool> DeleteContact(int id)
        {
            var result = await _addressBookRL.DeleteContact(id);
            if (!result) return false;

            try
            {
                await _cache.RemoveAsync($"Contact_{id}");
                await _cache.RemoveAsync("AllContacts");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis Error] Failed to remove cache: {ex.Message}");
            }

            return true;
        }
    }
}
