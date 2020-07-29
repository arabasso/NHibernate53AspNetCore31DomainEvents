using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate53AspNetCore31DomainEvents.Models;

namespace NHibernate53AspNetCore31DomainEvents.Domain.Events
{
    public class EntityDomainEvent<TEntity>
        : IDomainEvent
        where TEntity : IEntity
    {
        public Type EntityType { get; set; }
        public TEntity Entity { get; }
        public IEnumerable<PropertyEntry> Properties { get; }

        public EntityDomainEvent(
            Type entityType,
            TEntity entity,
            IEnumerable<PropertyEntry> properties)
        {
            EntityType = entityType;
            Entity = entity;
            Properties = properties;
        }

        public PropertyEntry Property(
            string name)
        {
            return Properties.Single(s => s.Info.Name == name);
        }

        public bool PropertyIsModified(
            string name)
        {
            return Property(name).IsModified;
        }
    }
}
