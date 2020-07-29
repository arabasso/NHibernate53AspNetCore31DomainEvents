using System;
using System.Threading.Tasks;
using NHibernate53AspNetCore31DomainEvents.Domain.Events;
using NHibernate53AspNetCore31DomainEvents.Domain.Events.Handles;

namespace NHibernate53AspNetCore31DomainEvents.Domain.Services
{
    public class DomainEventHandleService
        : IDomainEventHandleServiceAsync
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventHandleService(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task RaiseAsync(
            object @event)
        {
            var type = typeof(IDomainEventHandleAsync<>).MakeGenericType(@event.GetType());
            var method = type.GetMethod("HandleAsync");

            if (method == null) return;

            foreach (var handle in _serviceProvider.GetServices(type))
            {
                await (Task) method.Invoke(handle, new[] {@event});
            }
        }

        public async Task RaiseAsync<T>(T @event)
            where T : IDomainEvent
        {
            foreach (var handle in _serviceProvider.GetServices<IDomainEventHandleAsync<T>>())
            {
                await handle.HandleAsync(@event);
            }
        }
    }
}
