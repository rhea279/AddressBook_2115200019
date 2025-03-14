using System.ComponentModel.DataAnnotations;
namespace ModelLayer.Model
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required,EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required,MinLength(8)]
        public string PasswordHash { get; set; } = string.Empty;
        public string? ResetToken { get; set; }  
        public DateTime? ResetTokenExpiry { get; set; }
    }
}
