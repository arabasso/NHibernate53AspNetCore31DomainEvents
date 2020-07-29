using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate53AspNetCore31DomainEvents.Models;

namespace NHibernate53AspNetCore31DomainEvents.Domain.Events.Handles
{
    public class IsNotAudit : Attribute { }

    public enum AuditEventType
    {
        Added,
        Modified,
        Deleted
    }

    public class AuditDomainEventHandle
        : IDomainEventHandleAsync<AfterAddedEntityDomainEvent<IAudit>>,
        IDomainEventHandleAsync<AfterModifiedEntityDomainEvent<IAudit>>,
        IDomainEventHandleAsync<AfterDeletedEntityDomainEvent<IAudit>>
    {
        private readonly ISession _session;

        public AuditDomainEventHandle(
            ISession session)
        {
            _session = session;
        }

        public async Task HandleAsync(
            AfterAddedEntityDomainEvent<IAudit> args)
        {
            await HandleAsync(AuditEventType.Added, args, entry => true);
        }

        public async Task HandleAsync(
            AfterModifiedEntityDomainEvent<IAudit> args)
        {
            await HandleAsync(AuditEventType.Modified, args, entry => entry.IsModified);
        }

        public void Handle(
            AfterDeletedEntityDomainEvent<IAudit> args)
        {
            HandleAsync(AuditEventType.Deleted, args, entry => true).Wait();
        }

        public async Task HandleAsync(
            AfterDeletedEntityDomainEvent<IAudit> args)
        {
            await HandleAsync(AuditEventType.Deleted, args, entry => true);
        }

        public async Task HandleAsync(
            AuditEventType action,
            EntityDomainEvent<IAudit> args,
            Func<PropertyEntry, bool> predicate)
        {
            var now = DateTime.Now;

            var audit = new Audit
            {
                Module = args.EntityType.Name,
                Action = action,
                Date = now.Date,
                Time = now.TimeOfDay
            };

            foreach (var property in args.Properties.Where(predicate).Where(w => !Attribute.IsDefined(w.Info, typeof(IsNotAudit))))
            {
                if (property.CurrentValue is IList) continue;

                var originalValue = GetValue(property.OriginalValue);
                var currentValue = GetValue(property.CurrentValue);

                audit.Data.Add(new AuditData(audit, property.Info.Name, originalValue, currentValue));
            }

            await _session.SaveAsync(audit);
        }

        private static string GetValue(
            object value)
        {
            if (value is bool b)
            {
                return b ? "Yes" : "No";
            }

            var v = value + string.Empty;

            return string.IsNullOrEmpty(v)
                ? null
                : v;
        }
    }
}
