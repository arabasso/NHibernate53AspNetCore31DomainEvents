using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate53AspNetCore31DomainEvents.Models.Mappings
{
    public class AuditMapping :
        ClassMapping<Audit>
    {
        public AuditMapping()
        {
            Table("audits");
            Id(m => m.Id, p => p.Generator(Generators.Identity));
            Property(m => m.Action);
            Property(m => m.Module, p => p.Length(255));
            Property(m => m.Date, p => p.Type(NHibernateUtil.Date));
            Property(m => m.Time, p => p.Type(NHibernateUtil.TimeAsTimeSpan));
            Bag(m => m.Data, p =>
            {
                p.Inverse(true);
                p.Key(k => k.Column("Audit"));
                p.Cascade(Cascade.All);
            }, mm => mm.OneToMany());
        }
    }
}