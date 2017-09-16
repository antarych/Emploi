using NHibernate.Mapping.ByCode.Conformist;
using UserManagement.Domain;

namespace DataAccess.Mappings
{
    public class ConfirmationRequestMap : ClassMapping<ConfirmationRequest>
    {
        public ConfirmationRequestMap()
        {
            Id(request => request.ConfirmationToken, mapper => mapper.Column("Token"));
            Property(request => request.UserId, mapper => mapper.Column("UserId"));
            Property(request => request.ConfirmationType, mapper => mapper.Column("ConfirmationType"));
        }
    }
}
