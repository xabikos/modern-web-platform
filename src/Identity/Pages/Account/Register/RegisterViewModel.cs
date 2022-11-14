using System.ComponentModel.DataAnnotations;

namespace Identity.Pages.Account.Register
{
    public class RegisterViewModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Firstname { get; set; }
        [Required]
        public string? Lastname { get; set; }
        [Required]
        public string? Password { get; set; }

        public string? ReturnUrl { get; set; }
        
    }
}
