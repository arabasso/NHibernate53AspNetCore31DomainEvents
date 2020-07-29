using System.Threading.Tasks;
using NHibernate53AspNetCore31DomainEvents.Domain.Events;

namespace NHibernate53AspNetCore31DomainEvents.Domain.Services
{
    public interface IDomainEventHandleServiceAsync
    {
        Task RaiseAsync(object @event);
        Task RaiseAsync<T>(T @event) where T : IDomainEvent;
    }
}