using System.Threading.Tasks;

namespace NHibernate53AspNetCore31DomainEvents.Domain.Events.Handles
{
    public interface IDomainEventHandleAsync<in T>
        where T : IDomainEvent
    {
        Task HandleAsync(T args);
    }
}
