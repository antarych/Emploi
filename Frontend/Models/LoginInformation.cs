using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
    public class LoginInformation
    {
        [EmailAddress]
        public string Mail { get; set; }

        public string Password { get; set; }
    }
}