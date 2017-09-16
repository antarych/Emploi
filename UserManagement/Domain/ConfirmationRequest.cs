namespace UserManagement.Domain
{
    public class ConfirmationRequest
    {
        public ConfirmationRequest(int userId, string confirmationToken, ConfirmationType confirmationType)
        {
            UserId = userId;
            ConfirmationToken = confirmationToken;
            ConfirmationType = confirmationType;
        }

        protected ConfirmationRequest()
        {
            
        }

        public virtual int UserId { get; set; }

        public virtual string ConfirmationToken { get; set; }

        public virtual ConfirmationType ConfirmationType { get; set; }
    }
}
