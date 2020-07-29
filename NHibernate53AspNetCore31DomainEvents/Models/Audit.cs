using System;
using System.Collections.Generic;
using NHibernate53AspNetCore31DomainEvents.Domain.Events.Handles;

namespace NHibernate53AspNetCore31DomainEvents.Models
{
    public class Audit
        : IEntity
    {
        public virtual long Id { get; set; }
        public virtual AuditEventType Action { get; set; }
        public virtual string Module { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual TimeSpan Time { get; set; }

        private IList<AuditData> _data;
        public virtual IList<AuditData> Data
        {
            get => _data ??= new List<AuditData>();
            set => _data = value;
        }
    }
}
