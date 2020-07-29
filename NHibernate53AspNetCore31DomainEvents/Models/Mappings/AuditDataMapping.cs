using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate53AspNetCore31DomainEvents.Models.Mappings
{
    public class AuditDataMapping :
        ClassMapping<AuditData>
    {
        public AuditDataMapping()
        {
            Table("auditsdata");
            Id(m => m.Id, p => p.Generator(Generators.Identity));
            Property(m => m.Field, p =>
            {
                p.NotNullable(true);
                p.Length(150);
            });
            Property(m => m.OriginalValue, p => p.Length(2048));
            Property(m => m.CurrentValue, p => p.Length(2048));
            ManyToOne(m => m.Audit, p => p.NotNullable(true));
        }
    }
}