using DataAccess.Mappings.Application;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using UserManagement.Domain;

namespace DataAccess.Mappings
{
    class UserMap : ClassMapping<Account>
    {
        public UserMap()
        {
            Table("Account");
            Id(user => user.UserId, mapper => mapper.Generator(Generators.Identity));          
            Property(user => user.Email, mapper =>
            {
                mapper.Column("Email");
                mapper.Unique(true);
                mapper.Type<MailAddressType>();

            });
            Property(user => user.Password, mapper =>
            {
                mapper.Column("Password");
                mapper.Type<PasswordType>();
            });
            Component(x => x.Profile, m =>
            {
                m.Property(profile => profile.Avatar, mapper => mapper.Column("Avatar"));
                m.Property(profile => profile.Name, mapper => mapper.Column("Name"));
                m.Property(profile => profile.Surname, mapper => mapper.Column("Surname"));
                m.Property(profile => profile.Middlename, mapper => mapper.Column("Middlename"));
                m.Property(profile => profile.AboutUser, mapper => mapper.Column("AboutUser"));
                m.Bag(profile => profile.Contacts, mapper =>
                {
                    mapper.Cascade(Cascade.All);               
                }, 
                action => action.OneToMany());
                m.Property(profile => profile.Institute, mapper => mapper.Column("Institute"));
                m.Property(profile => profile.Course, mapper => mapper.Column("Course"));
                m.Property(profile => profile.Direction, mapper => mapper.Column("Direction"));
                m.Set(profile => profile.Tags, mapper =>
                {
                    mapper.Key(k => k.Column("UserId"));
                    mapper.Cascade(Cascade.All);
                    mapper.Table("Tags_users");
                },
                action => action.ManyToMany(map => map.Column("TagId")));
                m.Bag(profile => profile.Portfolio, mapper =>
                {
                    mapper.Cascade(Cascade.All);
                },
                action => action.OneToMany());
                m.Set(profile => profile.Organizations, mapper =>
                {
                    mapper.Key(k => k.Column("UserId"));
                    mapper.Cascade(Cascade.All);
                    mapper.Table("Organizations_users");
                },
                action => action.ManyToMany(map => map.Column("OrganizationId")));
            });

            Property(user => user.RegistrationTime, mapper => mapper.Column("RegistrationDate"));
            Property(user => user.Role, mapper => mapper.Column("Role"));
            Property(user => user.ConfirmationStatus, mapper => mapper.Column("Confirmation_status"));
        }
    }
}
