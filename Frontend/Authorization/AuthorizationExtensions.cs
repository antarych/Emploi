using System.Security.Principal;
using UserManagement.Domain;

namespace Frontend.Authorization
{
    public static class AuthorizationExtensions
    {
        public static bool IsInRole(this IPrincipal principal, AccountRoles role)
        {
            return (principal as AmpluaPrincipal)?.IsInRole(role) ?? false;
        }
    }
}