using Microsoft.EntityFrameworkCore;
using NeoContact.Data;
using NeoContact.Models;
using NeoContact.Services;

namespace NeoContact.Services.Interfaces
{
    public interface IAddressBookService
    {
        Task AddContactToCategoryAsync(int categoryId, int contactId);
        Task<bool> IsContactInCategory(int categoryId, int contactId);

        Task<IEnumerable<Category>> GetUserCategoriesAsync(string userId);
        Task<ICollection<int>> GetContactCategoryIdsAsync(int contactId);
        Task<ICollection<Category>> GetContactCategoriesAsync(int contactId);
        Task RemoveContactFromCategoryAsync(int categoryId, int contactId);
        
        IEnumerable<Contact> SerchForContacts(string searchString, string userId);
    }
}