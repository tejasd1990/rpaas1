// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ------------------------------------------------------------
namespace Provider.Helpers
{
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    /// <summary>
    /// Helper class for generating tokens.
    /// </summary>
    public static class TokensHelper
    {
        /// <summary>
        /// The configuration.
        /// </summary>
        private static IConfiguration Configuration;

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="tenantId">The tenant Id.</param>
        public static async Task<AuthenticationResult> GetAccessToken(IConfiguration config, string tenantId)
        {
            TokensHelper.Configuration = config;

            var authority = $"{TokensHelper.AadLoginTemplate}/{tenantId ?? TokensHelper.TenantId}";

            var clientCert = string.IsNullOrEmpty(TokensHelper.CertLocation)
                ? GetCertificateByThumbprint(TokensHelper.CertThumbprint)
                : new X509Certificate2(TokensHelper.CertLocation, TokensHelper.CertPassword);

            var clientCredential = new ClientAssertionCertificate(TokensHelper.ClientAppId, clientCert);
            var context = new AuthenticationContext(authority, false);

            return await context.AcquireTokenAsync(Audience, clientCredential).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the certificate by thumbprint.
        /// </summary>
        /// <param name="thumbprint">The thumbprint.</param>
        private static X509Certificate2 GetCertificateByThumbprint(string thumbprint)
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            var cert = store.Certificates.OfType<X509Certificate2>()
                .FirstOrDefault(x => x.Thumbprint == thumbprint);

            store.Close();
            return cert;
        }

        #region Config Values

        /// <summary>
        /// Gets the certificate location.
        /// </summary>
        private static string CertLocation { get => TokensHelper.Configuration["Microsoft.Contoso.ContosoApp.ServicePrincipalCertificateLocation"]; }

        /// <summary>
        /// Gets the certificate password.
        /// </summary>
        private static string CertPassword { get => TokensHelper.Configuration["Microsoft.Contoso.ContosoApp.ServicePrincipalCertificatePassword"]; }

        /// <summary>
        /// Gets the certificate thumbprint.
        /// </summary>
        private static string CertThumbprint { get => TokensHelper.Configuration["Microsoft.Contoso.ContosoApp.ServicePrincipalCertificateThumbprint"]; }

        /// <summary>
        /// Gets the client application ID.
        /// </summary>
        private static string ClientAppId { get => TokensHelper.Configuration["Microsoft.Contoso.ContosoApp.ApplicationId"]; }

        /// <summary>
        /// Gets the AAD login template.
        /// </summary>
        private static string AadLoginTemplate { get => TokensHelper.Configuration["Microsoft.Contoso.ContosoApp.AadLoginTemplate"]; }

        /// <summary>
        /// Gets the default tenant ID.
        /// </summary>
        private static string TenantId { get => TokensHelper.Configuration["Microsoft.Contoso.ContosoApp.TenantId"]; }

        /// <summary>
        /// Gets the audience.
        /// </summary>
        private static string Audience { get => TokensHelper.Configuration["Microsoft.Contoso.ContosoApp.AadAllowedAudiences"]; }

        #endregion
    }
}