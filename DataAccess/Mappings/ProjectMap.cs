using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using ProjectManagement.Domain;

namespace DataAccess.Mappings
{
    class ProjectMap : ClassMapping<Project>
    {
        public ProjectMap()
        {
            Table("Projects");
            Id(project => project.ProjectId, mapper => mapper.Generator(Generators.Identity));
            Property(project => project.ProjectImage, mapper => mapper.Column("Image"));
            Property(project => project.Leader, mapper => mapper.Column("Leader"));
            Property(project => project.ProjectName, mapper => mapper.Column("Name"));
            Property(project => project.ProjectDescription, mapper => mapper.Column("Description"));
            Set(project => project.ProjectTags, mapper =>
            {
                mapper.Key(k => k.Column("ProjectId"));
                mapper.Cascade(Cascade.All);
                mapper.Table("Tags_projects");
            },
            action => action.ManyToMany(map => map.Column("TagId")));
            Bag(vacancies => vacancies.Vacancies, mapper =>
            {
                mapper.Cascade(Cascade.All);
            },
            action => action.OneToMany());
            Property(project => project.FromOrganization, mapper => mapper.Column("FromOrganization"));
            Property(project => project.OrganizationId, mapper => mapper.Column("Organization"));
        }
    }
}
