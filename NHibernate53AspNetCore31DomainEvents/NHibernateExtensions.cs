using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Event;
using NHibernate53AspNetCore31DomainEvents.Domain.Events;

namespace NHibernate53AspNetCore31DomainEvents
{
    public class DataBaseProviderNotFoundException
        : Exception
    {
        public DataBaseProviderNotFoundException(
            string message)
            : base(message)
        {
        }
    }

    public static class NHibernateExtensions
    {
        public static Configuration Use(
            this Configuration configuration,
            string connectionString)
        {
            var match = Regex.Match(connectionString, @"provider=(?<provider>[^;]+);?", RegexOptions.IgnoreCase);

            if (!match.Success) throw new DataBaseProviderNotFoundException("Provider not found");

            connectionString = connectionString.Replace(match.Value, string.Empty);

            switch (match.Groups["provider"].Value.ToLower())
            {
                case "sqlserver":
                    return configuration.Use<NHibernate.Driver.SqlClientDriver, NHibernate.Dialect.MsSql2008Dialect>(connectionString);

                case "mysql":
                    return configuration.Use<NHibernate.Driver.MySqlDataDriver, NHibernate.Dialect.MySQL57Dialect>(connectionString);

                case "npgsql":
                    return configuration.Use<NHibernate.Driver.NpgsqlDriver, NHibernate.Dialect.PostgreSQL83Dialect>(connectionString);

                case "firebird":
                    return configuration.Use<NHibernate.Driver.FirebirdClientDriver, NHibernate.Dialect.FirebirdDialect>(connectionString);

                case "oracle":
                    return configuration.Use<NHibernate.Driver.OracleManagedDataClientDriver, NHibernate.Dialect.Oracle12cDialect>(connectionString);

                default:
                    throw new DataBaseProviderNotFoundException("Provider not found");
            }
        }

        public static Configuration Use<TDriver, TDialect>(
            this Configuration configuration,
            string connectionString)
            where TDriver : NHibernate.Driver.DriverBase
            where TDialect : NHibernate.Dialect.Dialect
        {
            configuration.DataBaseIntegration(db =>
            {
                db.Driver<TDriver>();
                db.Dialect<TDialect>();
                db.ConnectionString = connectionString;
                db.AutoCommentSql = true;
                db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
            });

            return configuration;
        }

        public static IServiceProvider GetServiceProvider(
            this AbstractEvent @event)
        {
            return ((ServiceProviderInterceptor)@event.Session.Interceptor).ServiceProvider;
        }

        public static T GetService<T>(
            this IServiceProvider serviceProvider)
        {
            return (T)serviceProvider.GetService(typeof(T));
        }

        public static IEnumerable<T> GetServices<T>(
            this IServiceProvider serviceProvider)
        {
            var genericEnumerable = typeof(IEnumerable<>).MakeGenericType(typeof(T));

            return (IEnumerable<T>)serviceProvider.GetService(genericEnumerable);
        }

        public static IEnumerable<object> GetServices(
            this IServiceProvider serviceProvider,
            Type serviceType)
        {
            var enumerable = typeof(IEnumerable<>).MakeGenericType(serviceType);

            return (IEnumerable<object>)serviceProvider.GetService(enumerable);
        }

        public static Configuration AddListener<T>(
            this Configuration configuration,
            ListenerType type,
            T listener)
            where T : class
        {
            // ReSharper disable once CoVariantArrayConversion
            configuration.AppendListeners(type, new [] { listener });

            return configuration;
        }

        public static Configuration AddDomainEventListener(
            this Configuration configuration)
        {
            var listener = new DomainEventHandleListener();

            configuration.AddListener<IPreInsertEventListener>(ListenerType.PreInsert, listener)
                .AddListener<IPostInsertEventListener>(ListenerType.PostInsert, listener)
                .AddListener<IPreUpdateEventListener>(ListenerType.PreUpdate, listener)
                .AddListener<IPostUpdateEventListener>(ListenerType.PostUpdate, listener)
                .AddListener<IPreDeleteEventListener>(ListenerType.PreDelete, listener)
                .AddListener<IPostDeleteEventListener>(ListenerType.PostDelete, listener);

            return configuration;
        }

        public static IEnumerable<PropertyEntry> ToPropertyEntry(
            this PostInsertEvent @event)
        {
            var entityType = NHibernateUtil.GetClass(@event.Entity);

            return @event.State
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((name, i) => new NHibernatePropertyEntry(entityType.GetProperty(@event.Persister.PropertyNames[i]), false, null, @event.State, i));
        }

        public static IEnumerable<PropertyEntry> ToPropertyEntry(
            this PostUpdateEvent @event)
        {
            var entityType = NHibernateUtil.GetClass(@event.Entity);

            return @event.State
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((name, i) =>
                {
                    var isModified = @event.Persister.PropertyTypes[i].IsModified(@event.OldState[i], @event.State[i], new[] { false }, @event.Session);

                    return new NHibernatePropertyEntry(entityType.GetProperty(@event.Persister.PropertyNames[i]), isModified, @event.OldState, @event.State, i);
                });
        }

        public static IEnumerable<PropertyEntry> ToPropertyEntry(
            this PostDeleteEvent @event)
        {
            var entityType = NHibernateUtil.GetClass(@event.Entity);

            return @event.DeletedState
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((propertyName, i) => new NHibernatePropertyEntry(entityType.GetProperty(@event.Persister.PropertyNames[i]), false, null, @event.DeletedState, i));
        }

        public static IEnumerable<PropertyEntry> ToPropertyEntry(
            this PreDeleteEvent @event)
        {
            var entityType = NHibernateUtil.GetClass(@event.Entity);

            return @event.DeletedState
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((name, i) => new NHibernatePropertyEntry(entityType.GetProperty(@event.Persister.PropertyNames[i]), false, null, @event.DeletedState, i));
        }

        public static IEnumerable<PropertyEntry> ToPropertyEntry(
            this PreInsertEvent @event)
        {
            var entityType = NHibernateUtil.GetClass(@event.Entity);

            return @event.State
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((name, i) => new NHibernatePropertyEntry(entityType.GetProperty(@event.Persister.PropertyNames[i]), false, null, @event.State, i));
        }

        public static IEnumerable<PropertyEntry> ToPropertyEntry(
            this PreUpdateEvent @event)
        {
            var entityType = NHibernateUtil.GetClass(@event.Entity);

            return @event.State
                .Select((t, i) => @event.Persister.PropertyNames[i])
                .Select((name, i) =>
                {
                    var isModified = @event.Persister.PropertyTypes[i].IsModified(@event.OldState[i], @event.State[i], new[] { false }, @event.Session);

                    return new NHibernatePropertyEntry(entityType.GetProperty(@event.Persister.PropertyNames[i]), isModified, @event.OldState, @event.State, i);
                });
        }

        public static Configuration WithSchemaCreate(
            this Configuration configuration)
        {
            configuration.DataBaseIntegration(db => db.SchemaAction = SchemaAutoAction.Create);

            return configuration;
        }
    }
}