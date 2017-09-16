using NHibernate.Mapping;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using ProjectManagement.Domain;

namespace DataAccess.Mappings
{
    class VacancyMap : ClassMapping<Vacancy> 
    {
        public VacancyMap()
        {
            Table("Vacancies");
            Id(vacancy => vacancy.VacancyId, mapper => mapper.Generator(Generators.Increment));
            Property(vacancy => vacancy.Name, mapper => mapper.Column("Name"));
            Property(vacancy => vacancy.Description, mapper => mapper.Column("Description"));
            Set(vacancy => vacancy.VacancyTags, mapper =>
            {
                mapper.Key(k => k.Column("VacancyId"));
                mapper.Cascade(Cascade.All);
                mapper.Table("Tags_vacancies");
            },
            action => action.ManyToMany(map => map.Column("TagId")));
        }
    }
}
