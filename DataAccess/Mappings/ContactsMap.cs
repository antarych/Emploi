using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using UserManagement.Domain;


namespace DataAccess.Mappings
{
    public class ContactsMap:ClassMapping<Contacts>
    {
        public ContactsMap()
        {
            Table("Contacts");
            Id(contact => contact.Id, mapper => mapper.Generator(Generators.Increment));
            Property(contact => contact.name, mapper => mapper.Column("Name"));
            Property(contact => contact.value, mapper => mapper.Column("Value"));   
        }
    }
}
