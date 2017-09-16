using NHibernate.Mapping.ByCode.Conformist;
using ProjectManagement.Domain;

namespace DataAccess.Mappings
{
    public class VacancyTokenMap : ClassMapping<VacancyToken>
    {
        public VacancyTokenMap()
        {
            Id(t => t.Token, mapper => mapper.Column("Token"));
            Property(t => t.VacancyId, mapper => mapper.Column("VacancyId"));
        }
    }
}
