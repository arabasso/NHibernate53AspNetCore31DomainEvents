using System;
using NHibernate;

namespace NHibernate53AspNetCore31DomainEvents
{
    public class ServiceProviderInterceptor :
        EmptyInterceptor
    {
        public IServiceProvider ServiceProvider { get; }

        public ServiceProviderInterceptor(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}
