using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Model
{
    public class User
    {
       
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Store hashed password

        public string? Role { get; set; } = "User"; // Default role is "User"

        public ICollection<AddressBookEntry>? AddressBookEntries { get; set; }
    }
}
