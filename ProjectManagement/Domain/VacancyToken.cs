namespace ProjectManagement.Domain
{
    public class VacancyToken
    {
        public VacancyToken()
        {
            
        }

        public virtual int VacancyId { get; set; }

        public virtual string Token { get; set; }
    }
}
