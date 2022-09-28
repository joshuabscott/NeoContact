using System.ComponentModel.DataAnnotations;

namespace NeoContact.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string? AppUserId { get; set; }
        
        [Required]
        [Display(Name="Category Name")]
        public string? Name { get; set; }

        //Virtual Property
        public virtual AppUser? AppUser { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; } = new HashSet<Contact>();
    }
}
