using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NeoContact.Enums;

namespace NeoContact.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        public string? AppUserID { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at lest {2} and a max {1} charactors long.",MinimumLength = 2)]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at lest {2} and a max {1} charactors long.",MinimumLength = 2)]
        public string? LastName { get; set; }

        #region FullName
        [NotMapped]
        public string? FullName { get { return $"{FirstName} {LastName}"; } }
        #endregion

        [Display(Name = "Birthday")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Required]
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }

        [Required]
        public string? City { get; set; }

        [Required]
        public States State { get; set; }

        [Required]
        [Display(Name = "Zip")]
        [DataType(DataType.PostalCode)]
        public int? Zip { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        //Image properties
        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }

        [NotMapped]
        public IFormFile? formFile { get; set; }

        //Virtual Property
        public virtual AppUser? AppUser { get; set; }
        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();

    }
}
