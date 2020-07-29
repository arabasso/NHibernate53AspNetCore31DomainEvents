namespace NHibernate53AspNetCore31DomainEvents.Models
{
    public class AuditData
        : IEntity
    {
        public virtual long Id { get; set; }
        public virtual Audit Audit { get; set; }
        public virtual string Field { get; set; }
        public virtual string OriginalValue { get; set; }
        public virtual string CurrentValue { get; set; }

        protected AuditData()
        {
        }

        public AuditData(
            Audit audit,
            string field,
            string originalValue,
            string currentValue)
        {
            Audit = audit;
            Field = field;
            OriginalValue = originalValue;
            CurrentValue = currentValue;
        }
    }
}