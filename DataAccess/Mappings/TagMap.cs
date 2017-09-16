using Common;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace DataAccess.Mappings
{
    class TagMap : ClassMapping<Tag>
    {
        public TagMap()
        {
            Table("Tags");
            Id(tag => tag.TagId, mapper => mapper.Generator(Generators.Identity));
            Property(tag => tag.TagName, mapper => mapper.Column("Tag"));
        }
    }
}
