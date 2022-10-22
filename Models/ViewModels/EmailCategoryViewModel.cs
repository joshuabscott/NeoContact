namespace NeoContact.Models.ViewModels
{
    public class EmailCategoryViewModel
    {
        //ADD #47 Category Edit - Email Category GET Action
        public List<Contact>? Contacts { get; set; }
        public EmailData? EmailData { get; set; }
    }
}