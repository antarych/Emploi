using ProjectManagement.Domain;

namespace Frontend.Models
{
    public class VacancyTokenPresentation
    {
        public VacancyTokenPresentation(VacancyToken token)
        {
            vacancyToken = token.Token;
        }

        public string vacancyToken { get; set; }
    }
}