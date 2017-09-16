using System.ComponentModel.DataAnnotations;


namespace Frontend.Models
{
    public class UserRegistrationModel
    {
        [EmailAddress]
        public string Mail { get; set; }

        [MaxLength(50)]
        public string Password { get; set; }
    }
}