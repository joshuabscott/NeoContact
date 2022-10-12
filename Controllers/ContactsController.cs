using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using NeoContact.Data;
using NeoContact.Models;
using NeoContact.Enums;
using NeoContact.Models.ViewModels;
using NeoContact.Services;
using NeoContact.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace NeoContact.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;
        //ADD
        private readonly UserManager<AppUser> _userManager;
        private readonly IImageService _imageService;
        private readonly IAddressBookService _addressBookService;
        private readonly IEmailSender _emailService;

        //MODIFY
        public ContactsController(ApplicationDbContext context,
                                    UserManager<AppUser> userManager,
                                    IImageService imageService,
                                    IAddressBookService addressBookService,
                                    IEmailSender emailService)
        {
            _context = context;
            //ADD
            _userManager = userManager;
            _imageService = imageService;
            _addressBookService = addressBookService;
            _emailService = emailService;
        }

        // GET: Contacts
        [Authorize]
        public async Task<IActionResult> Index(int categoryId, string swalMessage = null)
        {
            ViewData["SwalMessage"] = swalMessage;
            //MODIFY
            List<Contact> contacts = new List<Contact>();
            string appUserId = _userManager.GetUserId(User);

            //return the userId and its associated contacts & categories
            AppUser appUser = _context.Users
                                       .Include(c => c.Contacts)
                                       .ThenInclude(c => c.Categories)
                                       .FirstOrDefault(u => u.Id == appUserId);

            var categories = appUser.Categories;
            //MODIFY Lesson #25 Filter Contacts By Category
            if (categoryId == 0)
            {
                contacts = appUser.Contacts
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName)
                    .ToList();
            }
            else
            {
                contacts = appUser.Categories.FirstOrDefault(c => c.Id == categoryId)
                                  .Contacts
                                  .OrderBy(c => c.LastName)
                                  .ThenBy(c => c.FirstName)
                                  .ToList();
            }
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", categoryId);

            //return View(await applicationDbContext.ToListAsync());
            return View(contacts);
        }

        //ADD Lesson #26 Searching Contacts
        [Authorize]
        public IActionResult SearchContacts(string searchString)
        {
            string appUserId = _userManager.GetUserId(User);
            var contacts = new List<Contact>();

            AppUser appUser = _context.Users
                                      .Include(c => c.Contacts)
                                      .ThenInclude(c => c.Categories)
                                      .FirstOrDefault(u => u.Id == appUserId);

            if (String.IsNullOrEmpty(searchString))
            {
                contacts = appUser.Contacts
                                  .OrderBy(c => c.LastName)
                                  .ThenBy(c => c.FirstName)
                                  .ToList();

            }
            else
            {
                contacts = appUser.Contacts.Where(c => c.FullName!.ToLower().Contains(searchString.ToLower()))
                                  .OrderBy(c => c.LastName)
                                  .ThenBy(c => c.FirstName)
                                  .ToList();
            }
            ViewData["CategoryId"] = new SelectList(appUser.Categories, "Id", "Name", 0);
            return View(nameof(Index), contacts);
        }

        //ADD Lesson #35 Emailing Contact - Creating The View
        [Authorize]
        public async Task<IActionResult> EmailContact(int id)
        {
            //ADD Lesson #36 Email Contact - Creating The View Models
            string appUserId = _userManager.GetUserId(User);
            Contact contact = await _context.Contacts.Where(c => c.Id == id && c.AppUserID == appUserId)
                                                     .FirstOrDefaultAsync();
            if (contact == null)
            {
                return NotFound();
            }

            EmailData emailData = new EmailData()
            {
                EmailAddress = contact.Email,
                FirstName = contact.FirstName,
                LastName = contact.LastName,

            };

            EmailContactViewModel model = new EmailContactViewModel()
            {
                Contacts = contact,
                EmailData = emailData,
            };

            return View(model);
        }

        //ADD Lesson #41 Email Contact - Sending Emails
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EmailContact(EmailContactViewModel ecvm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _emailService.SendEmailAsync(ecvm.EmailData.EmailAddress, ecvm.EmailData.Subject, ecvm.EmailData.Body);
                    return RedirectToAction("Index", "Contacts", new {swalMessage = "Success: Email Sent!"});
                }
                catch
                {
                    return RedirectToAction("Index", "Contacts", new { swalMessage = "Error: Email Send Failed!"});
                    throw;
                }
            }
            return View(ecvm);
        }

        // GET: Contacts/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contacts/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            //CHANGE
            string appUserId = _userManager.GetUserId(User);
            //ADD
            ViewData["StatesList"] = new SelectList(Enum.GetValues(typeof(States)).Cast<States>().ToList());
            ViewData["CategoryList"] = new MultiSelectList(await _addressBookService.GetUserCategoriesAsync(appUserId), "Id", "Name");
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Date,Address1,Address2,City,State,Zip,Email,PhoneNumber,formFile")] Contact contact, List<int> CategoryList)
        {
            //REMOVE
            ModelState.Remove("AppUserId");

            if (ModelState.IsValid)
            {
                //ADD
                contact.AppUserID = _userManager.GetUserId(User);
                contact.Created = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                if (contact.Date != null)
                {
                    contact.Date = DateTime.SpecifyKind(contact.Date.Value, DateTimeKind.Utc);
                }

                if (contact.formFile != null)
                {
                    //ADD
                    contact.ImageData = await _imageService.ConvertFileToByteArrayAsync(contact.formFile);
                    contact.ImageType = contact.formFile.ContentType;
                }
                _context.Add(contact);
                await _context.SaveChangesAsync();

                //loop over all the selected categories
                foreach (int categoryId in CategoryList)
                {
                    await _addressBookService.AddContactToCategoryAsync(categoryId, contact.Id);
                }
                //save each category selected to the contractcategories table

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Contacts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            //ADD Lesson #29 Edit Contact View for Security Issues - Adding States
            string appUserId = _userManager.GetUserId(User);
            //MODIFY
            //var contact = await _context.Contacts.FindAsync(id);
            var contact = await _context.Contacts.Where(c => c.Id == id && c.AppUserID == appUserId)
                                        .FirstOrDefaultAsync();
            if (contact == null)
            {
                return NotFound();
            }
            //REMOVE
            //ViewData["AppUserID"] = new SelectList(_context.Users, "Id", "Id", contact.AppUserID);
            ViewData["StatesList"] = new SelectList(Enum.GetValues(typeof(States)).Cast<States>().ToList());
            ViewData["CategoryList"] = new MultiSelectList(await _addressBookService.GetUserCategoriesAsync(appUserId), "Id", "Name", await _addressBookService.GetContactCategoryIdsAsync(contact.Id));
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUserID,FirstName,LastName,Date,Address1,Address2,City,State,Zip,Email,PhoneNumber,Created,formFile,ImageData,ImageType")] Contact contact, List<int> CategoryList) //ADD Lesson#33 Edit Contact View - Saving Categories
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //ADD Lesson #31 Edit Contact View - Saving Dates
                    contact.Created = DateTime.SpecifyKind(contact.Created, DateTimeKind.Utc);

                    if (contact.Date != null)
                    {
                        contact.Date = DateTime.SpecifyKind(contact.Date.Value, DateTimeKind.Utc);
                    }

                    //ADD Lesson #32 Edit Contact View - Saving Images
                    if (contact.formFile != null)
                    {
                        contact.ImageData = await _imageService.ConvertFileToByteArrayAsync(contact.formFile);
                        contact.ImageType = contact.formFile.ContentType;
                    }

                    _context.Update(contact);
                    await _context.SaveChangesAsync();

                    //ADD Lesson #33 Edit Contact View - Saving Categories
                    //remove the current categories
                    List<Category> oldCategories = (await _addressBookService.GetContactCategoriesAsync(contact.Id)).ToList();
                    foreach (var category in oldCategories)
                    {
                        await _addressBookService.RemoveContactFromCategoryAsync(category.Id, contact.Id);
                    }
                    //add the selected categories
                    foreach (int categoryid in CategoryList)
                    {
                        await _addressBookService.AddContactToCategoryAsync(categoryid, contact.Id);
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppUserID"] = new SelectList(_context.Users, "Id", "Id", contact.AppUserID);
            return View(contact);
        }

        // GET: Contacts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }
            //MODIFY Lesson #42 Email Contact - Fix Delete View
            string appUserId = _userManager.GetUserId(User);
            var contact = await _context.Contacts
                                .FirstOrDefaultAsync(c => c.Id == id && c.AppUserID == appUserId);
            if (contact == null)
            {
                return NotFound();
            }
            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //MODIFY Lesson #42 Email Contact - Fix Delete View
            string appUserId = _userManager.GetUserId(User);
            var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id && c.AppUserID == appUserId);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }
    }
}