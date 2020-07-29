using System;
using System.Collections.Generic;
using NHibernate53AspNetCore31DomainEvents.Models;

namespace NHibernate53AspNetCore31DomainEvents.Domain.Events
{
    public class AfterModifiedEntityDomainEvent<TEntity>
        : EntityDomainEvent<TEntity>
        where TEntity : IEntity
    {
        public AfterModifiedEntityDomainEvent(
            Type entityType,
            TEntity entity,
            IEnumerable<PropertyEntry> properties)
            : base(entityType, entity, properties)
        {
        }
    }
}
