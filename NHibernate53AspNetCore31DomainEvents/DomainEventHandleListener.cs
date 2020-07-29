using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate53AspNetCore31DomainEvents.Domain.Events;
using NHibernate53AspNetCore31DomainEvents.Domain.Services;
using NHibernate53AspNetCore31DomainEvents.Models;

namespace NHibernate53AspNetCore31DomainEvents
{
    public class DomainEventHandleListener
        : IPostInsertEventListener, IPostUpdateEventListener, IPostDeleteEventListener,
        IPreInsertEventListener, IPreUpdateEventListener, IPreDeleteEventListener
    {
        public async Task OnPostDeleteAsync(
            PostDeleteEvent @event,
            CancellationToken cancellationToken)
        {
            if (!(@event.Entity is IEntity)) return;

            var properties = @event.ToPropertyEntry().ToList();

            await RaiseAsync(@event, typeof(AfterDeletedEntityDomainEvent<>), @event.Entity, properties);
        }

        public void OnPostDelete(
            PostDeleteEvent @event)
        {
            OnPostDeleteAsync(@event, CancellationToken.None).Wait();
        }

        public async Task OnPostInsertAsync(
            PostInsertEvent @event,
            CancellationToken cancellationToken)
        {
            if (!(@event.Entity is IEntity)) return;

            var properties = @event.ToPropertyEntry().ToList();

            await RaiseAsync(@event, typeof(AfterAddedEntityDomainEvent<>), @event.Entity, properties);
        }

        public void OnPostInsert(
            PostInsertEvent @event)
        {
            OnPostInsertAsync(@event, CancellationToken.None).Wait();
        }

        public async Task OnPostUpdateAsync(
            PostUpdateEvent @event,
            CancellationToken cancellationToken)
        {
            if (!(@event.Entity is IEntity)) return;

            var properties = @event.ToPropertyEntry().ToList();

            await RaiseAsync(@event, typeof(AfterModifiedEntityDomainEvent<>), @event.Entity, properties);
        }

        public void OnPostUpdate(
            PostUpdateEvent @event)
        {
            OnPostUpdateAsync(@event, CancellationToken.None).Wait();
        }

        public async Task<bool> OnPreDeleteAsync(
            PreDeleteEvent @event,
            CancellationToken cancellationToken)
        {
            if (!(@event.Entity is IEntity)) return false;

            var properties = @event.ToPropertyEntry().ToList();

            await RaiseAsync(@event, typeof(BeforeDeletedEntityDomainEvent<>), @event.Entity, properties);

            return false;
        }

        public bool OnPreDelete(
            PreDeleteEvent @event)
        {
            return OnPreDeleteAsync(@event, CancellationToken.None).Result;
        }

        public async Task<bool> OnPreInsertAsync(
            PreInsertEvent @event,
            CancellationToken cancellationToken)
        {
            if (!(@event.Entity is IEntity)) return false;

            var properties = @event.ToPropertyEntry().ToList();

            await RaiseAsync(@event, typeof(BeforeAddedEntityDomainEvent<>), @event.Entity, properties);

            return false;
        }

        public bool OnPreInsert(
            PreInsertEvent @event)
        {
            return OnPreInsertAsync(@event, CancellationToken.None).Result;
        }

        public async Task<bool> OnPreUpdateAsync(
            PreUpdateEvent @event,
            CancellationToken cancellationToken)
        {
            if (!(@event.Entity is IEntity)) return false;

            var properties = @event.ToPropertyEntry().ToList();

            await RaiseAsync(@event, typeof(BeforeModifiedEntityDomainEvent<>), @event.Entity, properties);

            return false;
        }

        public bool OnPreUpdate(
            PreUpdateEvent @event)
        {
            return OnPreUpdateAsync(@event, CancellationToken.None).Result;
        }

        private async Task RaiseAsync(
            AbstractEvent @event,
            Type eventType,
            object entity,
            IEnumerable<PropertyEntry> properties)
        {
            foreach (CollectionEntry collection in @event.Session.PersistenceContext.CollectionEntries.Values)
            {
                collection.IsProcessed = true;
            }

            var domainService = @event.GetServiceProvider().GetRequiredService<DomainEventHandleService>();

            foreach (var domainEvent in GetDomainEvents(eventType, entity, properties))
            {
                await domainService.RaiseAsync(domainEvent);
            }
        }

        private IEnumerable<object> GetDomainEvents(
            Type eventType,
            object entity,
            IEnumerable<PropertyEntry> properties)
        {
            var entityType = NHibernateUtil.GetClass(entity);

            return SelfAndInterfaces(entityType.GetInterfaces(), entityType)
                .Select(t => Activator.CreateInstance(eventType.MakeGenericType(t), entityType, entity, properties));
        }

        private IEnumerable<Type> SelfAndInterfaces(
            IEnumerable<Type> interfaces,
            params Type[] type)
        {
            return type.Concat(interfaces);
        }
    }
}