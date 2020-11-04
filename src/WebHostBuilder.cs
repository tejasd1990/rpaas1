// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ------------------------------------------------------------

namespace Provider
{
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Server.Kestrel.Https;
    using Microsoft.Extensions.Configuration;
    using Provider.Helpers;

    /// <summary>
    /// Web host builder.
    /// </summary>
    public class WebHostBuilder
    {
        /// <summary>
        /// Gets or sets configuration builder.
        /// </summary>
        private IConfigurationRoot Config { get; set; }

        /// <summary>
        /// Gets or sets args provided.
        /// </summary>
        private string[] Args { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebHostBuilder"/> class.
        /// </summary>
        public WebHostBuilder()
        {
            #pragma warning disable CA1304 // Specify CultureInfo
            var environment = DeploymentEnvironmentHelper.GetEnvironment().ToLower();

            this.Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"secrets/appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var pfxCertificates = this.Config
                .GetSection("Microsoft.Contoso.PFXCertificates")
                .Get<Dictionary<string, string>>();

            WebHostStartupHelper.CreatePFXCertificates(pfxCertificates);
            WebHostStartupHelper.CreateSecretAppSettingsFile();
        }

        /// <summary>
        /// Use the args provided.
        /// </summary>
        /// <param name="args">Main args.</param>
        public WebHostBuilder WithArgs(string[] args)
        {
            this.Args = args;
            return this;
        }

        /// <summary>
        /// Build the web host.
        /// </summary>
        public IWebHost Build()
        {
            return WebHost
                .CreateDefaultBuilder(this.Args)
                .UseConfiguration(this.Config)
                .UseKestrel(options =>
                {
                    options.ConfigureHttpsDefaults(httpsOptions =>
                    {
                        httpsOptions.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                    });
                })
                .UseStartup<Startup>()
                .Build();
        }
    }
}
