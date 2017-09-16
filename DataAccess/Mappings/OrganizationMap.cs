using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using OrganizationManagement.Domain;

namespace DataAccess.Mappings
{
    class OrganizationMap : ClassMapping<Organization>
    {
        public OrganizationMap()
        {
            Table("Organizations");
            Id(org => org.OrganizationId, mapper => mapper.Generator(Generators.Identity));
            Property(org => org.Leader, mapper => mapper.Column("Leader"));
            Property(org => org.OrganizationName, mapper => mapper.Column("Organization"));
            Property(org => org.OrganizationDescription, mapper => mapper.Column("Description"));
            Set(org => org.OrganizationTags, mapper =>
            {
                mapper.Key(k => k.Column("OrganizationId"));
                mapper.Cascade(Cascade.All);
                mapper.Table("Tags_organizations");
            },
            action => action.ManyToMany(map => map.Column("TagId")));
        }
    }
}
