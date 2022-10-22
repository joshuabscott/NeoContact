using NeoContact.Data;
using NeoContact.Models;
using NeoContact.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace NeoContact.Services
{
        public class AddressBookService : IAddressBookService
    {
        private readonly ApplicationDbContext _context;

        public AddressBookService(ApplicationDbContext context)
        {
            _context = context;
        }

        //MODIFY #21 Saving Contact Categories
        public async Task AddContactToCategoryAsync(int categoryId, int contactId)
        {
            //MODIFY #21 Saving Contact Categories
            try
            //throw new NotImplementedException();
            {
                //check to see if the category is in the contact
                if (!await IsContactInCategory(categoryId, contactId))
                {
                    // Using Virtual Properties???
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

        public async Task<ICollection<Category>> GetContactCategoriesAsync(int contactId)
        {
            //ADD Lesson #33 Edit Contact View - Saving Categories
            try
            {
                Contact? contact = await _context.Contacts.Include(c => c.Categories).FirstOrDefaultAsync();
                return contact.Categories;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<ICollection<int>> GetContactCategoryIdsAsync(int contactId)
        {
            //ADD Lesson #30 Edit Contact View - Saving Dates
            try
            {
                var contact = await _context.Contacts.Include(c => c.Categories)
                                                    .FirstOrDefaultAsync(c => c.Id == contactId);

                List<int> categoryIds = contact.Categories.Select(c => c.Id).ToList();
                return categoryIds;
            }
            catch(Exception){
                throw;
            }
        }

        //MODIFY #20 Binding Categories
        public async Task<IEnumerable<Category>> GetUserCategoriesAsync(string userId)
        {
            //MODIFY #20 Binding Categories / ContactsController GET: Contacts/Create
            List<Category> categories = new List<Category>();
            try
            { // Link statment that is filtered    // This is an => Arrow Function || Categories = c
                categories = await _context.Categories.Where(c => c.AppUserId == userId)
                                                      .OrderBy(c => c.Name) //.OrderBy is ascending order
                                                      .ToListAsync();
            }
            catch
            {
                throw;
            }
            return categories;
        }

        //MODIFY #21 Saving Contact Categories
        public async Task<bool> IsContactInCategory(int categoryId, int contactId)
        {
            //MODIFY #21 Saving Contact Categories
            Contact? contact = await _context.Contacts.FindAsync(contactId);
            return await _context.Categories
                                .Include(c => c.Contacts)
                                .Where(c => c.Id == categoryId && c.Contacts.Contains(contact!))
                                .AnyAsync();
        }

        public async Task RemoveContactFromCategoryAsync(int categoryId, int contactId)
        {
            //MODIFY  Lesson# 33 Edit Contact View - Saving Categories
            try
            {
                if (await IsContactInCategory(categoryId, contactId))
                {
                    Contact contact = await _context.Contacts.FindAsync(contactId);
                    Category category = await _context.Categories.FindAsync(contactId);

                    if (category != null && contact != null)
                    {
                        category.Contacts.Remove(contact);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<Contact> SerchForContacts(string searchString, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
