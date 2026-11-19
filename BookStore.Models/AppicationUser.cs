using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        // We don't want to create a column for this property in the database,
        // so we use the [NotMapped] attribute to indicate that it should be ignored by Entity Framework.
        [NotMapped]
        public string Role { get; set; } = string.Empty;
    }
}
