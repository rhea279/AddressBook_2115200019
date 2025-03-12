﻿using RepositoryLayer.Context;
using RepositoryLayer.Interface;
using ModelLayer.Model;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLayer.Service
{
    public class AddressBookRL: IAddressBookRL
    {
        private readonly AppDbContext _context;
        public AddressBookRL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AddressBookEntry>> GetAllContacts()
        {
            return await _context.AddressBookEntries.ToListAsync();
        }

        public async Task<AddressBookEntry> GetContactById(int id)
        {
            return await _context.AddressBookEntries.FindAsync(id);
        }
        public async Task AddContact(AddressBookEntry contact)
        {
            await _context.AddressBookEntries.AddAsync(contact);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateContact(AddressBookEntry contact)
        {
            _context.AddressBookEntries.Update(contact);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteContact(int id)
        {
            var contact = await _context.AddressBookEntries.FindAsync(id);
            if (contact != null)
            {
                _context.AddressBookEntries.Remove(contact);
                await _context.SaveChangesAsync();
            }
        }
    }
}
