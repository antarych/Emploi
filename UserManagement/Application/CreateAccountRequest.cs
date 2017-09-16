using System.Net.Mail;
using Journalist;


namespace UserManagement.Application
{
    public class CreateAccountRequest
    {
        public CreateAccountRequest(            
            string password,
            MailAddress email)
        {
            Require.NotNull(email, nameof(email));
            Require.NotEmpty(password, nameof(password));

            Email = email;
            Password = password;
        }

        public MailAddress Email { get; private set; }

        public string Password { get; private set; }
    }
}
