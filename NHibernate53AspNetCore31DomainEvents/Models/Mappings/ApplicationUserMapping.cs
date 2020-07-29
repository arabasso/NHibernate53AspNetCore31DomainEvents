using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate53AspNetCore31DomainEvents.Models.Mappings
{
    public class ApplicationUserMapping :
        ClassMapping<ApplicationUser>
    {
        public ApplicationUserMapping()
        {
            Table("aspnetusers");
            Id(e => e.Id, id =>
            {
                id.Generator(Generators.Identity);
            });
            Property(e => e.NormalizedUserName, prop =>
            {
                prop.Length(64);
                prop.NotNullable(true);
                prop.Unique(true);
            });
            Property(e => e.NormalizedEmail, prop =>
            {
                prop.Length(256);
                prop.NotNullable(true);
            });
            Property(e => e.EmailConfirmed, prop =>
            {
                prop.NotNullable(true);
            });
            Property(e => e.PhoneNumber, prop =>
            {
                prop.Length(128);
                prop.NotNullable(false);
            });
            Property(e => e.PhoneNumberConfirmed, prop =>
            {
                prop.NotNullable(true);
            });
            Property(e => e.AccessFailedCount, prop =>
            {
                prop.NotNullable(true);
            });
            Property(e => e.ConcurrencyStamp, prop =>
            {
                prop.Length(36);
                prop.NotNullable(false);
            });
            Property(e => e.TwoFactorEnabled, prop =>
            {
                prop.NotNullable(true);
            });
            Property(e => e.SecurityStamp, prop =>
            {
                prop.Length(64);
                prop.NotNullable(false);
            });
            ManyToOne(m => m.User, p =>
            {
                p.Column("UserId");
                p.NotNullable(true);
                p.Cascade(Cascade.All);
                p.Lazy(LazyRelation.NoLazy);
            });
        }
    }
}
