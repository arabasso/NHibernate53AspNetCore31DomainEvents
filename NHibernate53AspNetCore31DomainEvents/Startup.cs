using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.NetCore;
using NHibernate53AspNetCore31DomainEvents.Domain.Events;
using NHibernate53AspNetCore31DomainEvents.Domain.Events.Handles;
using NHibernate53AspNetCore31DomainEvents.Domain.Services;
using NHibernate53AspNetCore31DomainEvents.Models;
using NHibernate53AspNetCore31DomainEvents.Models.Mappings;

namespace NHibernate53AspNetCore31DomainEvents
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var cfg = new Configuration()
                .Use(Configuration.GetConnectionString("DefaultConnection"))
                //.WithSchemaCreate()
                .AddDomainEventListener();

            var mapper = new ModelMapper();

            mapper.AddMapping(new IdentityRoleMapping<ApplicationRole, long>(Generators.Identity));
            mapper.AddMapping(new IdentityRoleClaimMapping<long>(Generators.Identity));
            mapper.AddMapping<ApplicationUserMapping>();
            mapper.AddMapping(new IdentityUserClaimMapping<long>(Generators.Identity));
            mapper.AddMapping<IdentityUserLoginMapping<long>>();
            mapper.AddMapping<IdentityUserRoleMapping<long>>();
            mapper.AddMapping<IdentityUserTokenMapping<long>>();
            mapper.AddMapping<UserMapping>();
            mapper.AddMapping<AuditMapping>();
            mapper.AddMapping<AuditDataMapping>();

            cfg.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

            services.AddHibernate(cfg);

            services.AddTransient<IDomainEventHandleAsync<AfterAddedEntityDomainEvent<IAudit>>, AuditDomainEventHandle>();
            services.AddTransient<IDomainEventHandleAsync<AfterModifiedEntityDomainEvent<IAudit>>, AuditDomainEventHandle>();
            services.AddTransient<IDomainEventHandleAsync<AfterDeletedEntityDomainEvent<IAudit>>, AuditDomainEventHandle>();

            services.Replace(ServiceDescriptor.Scoped(provider =>
                provider.GetRequiredService<ISessionFactory>()
                    .WithOptions()
                    .Interceptor(new ServiceProviderInterceptor(provider))
                    .OpenSession()));
            services.AddScoped<DomainEventHandleService>();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddHibernateStores();
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
