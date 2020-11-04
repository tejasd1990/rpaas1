// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ------------------------------------------------------------

namespace Provider
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.AspNetCore.Routing.Constraints;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    /// <summary>
    /// The start up class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"> Collection of service descriptor.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => options.SuppressOutputFormatterBuffering = true).AddNewtonsoftJson();
            services.AddMvc();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application builder for configuring routes etc...</param>
        /// <param name="env">web hosting environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (env == null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(this.ConfigureRoutes);
        }

        /// <summary>
        /// Configures the routes.
        /// </summary>
        /// <param name="routeBuilder">Route builder.</param>
        private void ConfigureRoutes(IEndpointRouteBuilder routeBuilder)
        {
            this.ConfigureRoutesForProviderRegistrationController(routeBuilder);
        }

        /// <summary>
        /// Configures the routes for provider registration controller.
        /// </summary>
        /// <param name="routeBuilder">Route builder.</param>
        private void ConfigureRoutesForProviderRegistrationController(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapControllerRoute(
                name: "OnResourceCreationValidate",
                pattern: "subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/{providerNamespace}/{resourceType}/{resourceName}/resourceCreationValidate",
                defaults: new { controller = "Contoso", action = "OnResourceCreationValidate" },
                constraints: new { httpMethod = new HttpMethodRouteConstraint(new[] { "POST" }) });

            routeBuilder.MapControllerRoute(
                name: "OnResourceCreationCompleted",
                pattern: "subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/{providerNamespace}/{resourceType}/{resourceName}/resourceCreationCompleted",
                defaults: new { controller = "Contoso", action = "OnResourceCreationCompleted" },
                constraints: new { httpMethod = new HttpMethodRouteConstraint(new[] { "POST" }) });

            routeBuilder.MapControllerRoute(
                name: "OnResourceDeletionValidate",
                pattern: "subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/{providerNamespace}/{resourceType}/{resourceName}/resourceDeletionValidate",
                defaults: new { controller = "Contoso", action = "OnResourceDeletionValidate" },
                constraints: new { httpMethod = new HttpMethodRouteConstraint(new[] { "POST" }) });

            routeBuilder.MapControllerRoute(
                name: "OnResourceCreationBegin",
                pattern: "subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/{providerNamespace}/{resourceType}/{resourceName}",
                defaults: new { controller = "Contoso", action = "OnResourceCreationBegin" },
                constraints: new { httpMethod = new HttpMethodRouteConstraint(new[] { "PUT" }) });

            routeBuilder.MapControllerRoute(
                name: "OnSubscriptionLifeCycleNotification",
                pattern: "subscriptions/{subscriptionId}/providers/{providerNamespace}/{resourceType}/SubscriptionLifeCycleNotification",
                defaults: new { controller = "Contoso", action = "OnSubscriptionLifeCycleNotification" },
                constraints: new { httpMethod = new HttpMethodRouteConstraint(new[] { "PUT" }) });

            routeBuilder.MapControllerRoute(
                name: "OnHello",
                pattern: "hello",
                defaults: new { controller = "Contoso", action = "OnHello" },
                constraints: new { httpMethod = new HttpMethodRouteConstraint(new[] { "GET" }) });
        }
    }
}
