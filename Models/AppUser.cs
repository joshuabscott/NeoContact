using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace NeoContact.Models
{
    public class AppUser : IdentityUser
    {
        #region First & Last Name
        [Required]
        [Display(Name = "First Name")]
        [StringLength(50,ErrorMessage = "The {0} must be at lest {2} and a max {1} charactors long.", MinimumLength = 2)]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at lest {2} and a max {1} charactors long.", MinimumLength = 2)]
        public string? LastName { get; set; }
        #endregion

        #region FullName
        [NotMapped]
        public string? FullName { get { return $"{FullName} {LastName}"; } }
        #endregion
    }
}
