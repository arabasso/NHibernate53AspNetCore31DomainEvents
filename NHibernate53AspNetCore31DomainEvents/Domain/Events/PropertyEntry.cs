using System.Reflection;

namespace NHibernate53AspNetCore31DomainEvents.Domain.Events
{
    public abstract class PropertyEntry
    {
        public PropertyInfo Info { get; protected set; }
        public abstract object CurrentValue { get; set; }
        public abstract object OriginalValue { get; set; }
        public virtual bool IsModified => !Equals(CurrentValue, OriginalValue);
    }
}
