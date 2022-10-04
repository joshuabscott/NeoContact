using NeoContact.Data;
using NeoContact.Models;
using NeoContact.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace NeoContact.Services
{
    public class AddressBookService : IAddressBookService
    {
        private readonly ApplicationDbContext _context;

        public AddressBookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddContactToCategoryAsync(int categoryId, int contactId)
        {
            //MODIFY    AddContactToCategoryAsync
            try
            //throw new NotImplementedException();
            {
                //check to see if the categor is in the contact
                if (!await IsContactInCategory(categoryId, contactId))
                {
                    Contact? contact = await _context.Contacts.FindAsync(categoryId);
                    Category? category = await _context.Categories.FindAsync(contactId);

                    if (category != null && contact != null)
                    {
                        category.Contacts.Add(contact);
                        await _context.SaveChangesAsync();
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<ICollection<Category>> GetContactCategoriesAsync(int contactId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<int>> GetContactCategoryIdsAsync(int contactId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Category>> GetUserCategoriesAsync(string userId)
        {
            //MODIFY   GetUserCategoriesAsync
            List<Category> categories = new List<Category>();
            //throw new NotImplementedException();
            try
            {
                categories = await _context.Categories.Where(c => c.AppUserId == userId)
                                                      .OrderBy(c => c.Name)
                                                    .ToListAsync();
            }
            catch
            {
                throw;
            }

            return categories;

        }

        public async Task<bool> IsContactInCategory(int categoryId, int contactId)
        {
            //MODIFY     IsContactInCategory
            Contact? contact = await _context.Contacts.FindAsync(contactId);

            return await _context.Categories
                .Include(c => c.Contacts)
                .Where(c => c.Id == categoryId && c.Contacts.Contains(contact!))
                .AnyAsync();
            //throw new NotImplementedException();


        }

        public Task RemoveContactFromCategoryAsync(int categoryId, int contactId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Contact> SerchForContacts(string searchString, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
