using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate53AspNetCore31DomainEvents.Models.Mappings
{
    public class UserMapping :
        ClassMapping<User>
    {
        public UserMapping()
        {
            Table("users");
            Id(m => m.Id, p => p.Generator(Generators.Identity));
            Property(m => m.Login, p =>
            {
                p.NotNullable(true);
                p.Length(150);
            });
            Property(m => m.Email, p =>
            {
                p.NotNullable(true);
                p.Length(50);
            });
            Property(m => m.Password, p =>
            {
                p.NotNullable(true);
                p.Length(150);
            });
            Property(m => m.Active);
        }
    }
}