using System.Collections.Generic;
using Frontend.Models;

namespace UserManagement.Domain
{
    public class CurrentUserPresentation
    {
        public CurrentUserPresentation(Account account, AuthorizationTokenInfo userToken, List<ProjectPresentation> portfolio)
        {            
            token = userToken.Token;
            user = new UserPresentation(account, portfolio);
        }

        protected CurrentUserPresentation()
        {
            
        }

        public virtual string token { get; set; }

        public UserPresentation user { get; set; }
    }
}