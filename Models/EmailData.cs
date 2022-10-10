using System.ComponentModel.DataAnnotations;

namespace NeoContact.Models
{
    public class EmailData
    {
        //ADD Lesson #36 Email Contact - Creating The View Models
        [Required]
        public string EmailAddress { get; set; } = "";

        [Required]
        public string Subject { get; set; } = "";

        [Required]
        public string Body { get; set; } = "";

        public int? Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string GroupName { get; set; }
    }
}