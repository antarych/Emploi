
namespace UserManagement.Domain
{
    public enum AccountRoles
    {
        Admin,
        User
    }

    public enum ConfirmationStatus
    {
        NotConfirmed,
        MailConfirmed
    }

    public enum ConfirmationType
    {
        MailConfirmation,
        ProfileFilling
    }

    public enum Institutes
    {
        itasu, inmin, ekotekh, ibo, eupp, gorniy
    }
}
