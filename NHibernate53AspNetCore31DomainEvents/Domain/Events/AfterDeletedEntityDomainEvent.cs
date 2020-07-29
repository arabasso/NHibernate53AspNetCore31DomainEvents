using System;
using System.Collections.Generic;
using NHibernate53AspNetCore31DomainEvents.Models;

namespace NHibernate53AspNetCore31DomainEvents.Domain.Events
{
    public class AfterDeletedEntityDomainEvent<TEntity>
        : EntityDomainEvent<TEntity>
        where TEntity : IEntity
    {
        public AfterDeletedEntityDomainEvent(
            Type entityType,
            TEntity entity,
            IEnumerable<PropertyEntry> properties)
            : base(entityType, entity, properties)
        {
        }
    }
}