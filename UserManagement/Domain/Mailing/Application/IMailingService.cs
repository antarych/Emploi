namespace UserManagement.Application
{
    public interface IMailingService
    {
        void SetupEmailConfirmation(int userId);
    }
}
