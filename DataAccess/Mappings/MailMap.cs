using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using UserManagement.Domain.Mailing;

namespace DataAccess.Mappings
{
    public class MailMap : ClassMapping<NotificationModel>
    {
        public MailMap()
        {
            Id(model => model.Id, mapper => mapper.Generator(Generators.Native));
            Property(model => model.UsersId, mapper => mapper.Column("Users"));
            Property(model => model.Message, mapper => mapper.Column("Message"));
        }
    }
}
