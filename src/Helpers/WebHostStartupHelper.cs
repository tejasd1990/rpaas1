// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ------------------------------------------------------------

namespace Provider.Helpers
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    /// <summary>
    /// The starup helper.
    /// </summary>
    public static class WebHostStartupHelper
    {
        /// <summary>
        /// The fully qualified path to secrets directory where init container stores the secrets.
        /// </summary>
        private const string SecretsDirectoryPath = "/secrets/secrets";

        /// <summary>
        /// The path to directory where secret appsettings.json file should be stored.
        /// </summary>
        private const string SecretAppSettingsDirectoryPath = "/secrets";

        /// <summary>
        /// The name of secret appsettings.json file.
        /// </summary>
        private const string SecretAppSettingsFileName = "appsettings.json";

        /// <summary>
        /// The path to directory where PEM files are stored.
        /// </summary>
        private const string CertificatesDirectoryPath = "/secrets/certs_keys";

        /// <summary>
        /// Creates the secret appsettings.json file.
        /// </summary>
        public static void CreateSecretAppSettingsFile()
        {
            if (!DeploymentEnvironmentHelper.IsDogfood() && !DeploymentEnvironmentHelper.IsPublicOrNationalCloud())
            {
                return;
            }

            var appSettingsDictionary = new Dictionary<string, string>();

            var fileEntries = Directory.GetFiles(WebHostStartupHelper.SecretsDirectoryPath);

            foreach (var fullyQualifiedFileName in fileEntries)
            {
                using (var streamReader = new StreamReader(fullyQualifiedFileName))
                {
                    #pragma warning disable CA1307 // Specify StringComparison
                    var secretKey = Path
                        .GetFileName(fullyQualifiedFileName)
                        .Replace("-", ".");

                    var secretValue = streamReader.ReadToEnd();
                    appSettingsDictionary.Add(secretKey, secretValue);
                }
            }

            var appsettingsJson = JsonConvert.SerializeObject(appSettingsDictionary);

            // Write the json string to app setings file.
            using (var streamWriter = new StreamWriter(Path.Combine(WebHostStartupHelper.SecretAppSettingsDirectoryPath, WebHostStartupHelper.SecretAppSettingsFileName)))
            {
                streamWriter.WriteLine(appsettingsJson);
            }
        }

        /// <summary>
        /// Creates the PFX certificates.
        /// </summary>
        /// <param name="pfxCertificates"> The dictionary of PFX certificates names as key and PEM files from which PFX would be creates as value. </param>
        public static void CreatePFXCertificates(Dictionary<string, string> pfxCertificates)
        {
            if (!DeploymentEnvironmentHelper.IsDogfood() && !DeploymentEnvironmentHelper.IsPublicOrNationalCloud())
            {
                return;
            }

            if (pfxCertificates != null)
            {
                foreach (KeyValuePair<string, string> keyValuePair in pfxCertificates)
                {
                    var pfxCertificateName = keyValuePair.Key;
                    var publicCertificateName = keyValuePair.Value.Split(":")[0];
                    var privateKeyName = keyValuePair.Value.Split(":")[1];

                    var pfxCertPath = Path.Combine(WebHostStartupHelper.CertificatesDirectoryPath, pfxCertificateName);
                    var publicCertPath = Path.Combine(WebHostStartupHelper.CertificatesDirectoryPath, publicCertificateName);
                    var privateCertkeyPath = Path.Combine(WebHostStartupHelper.CertificatesDirectoryPath, privateKeyName);

                    CreatePFXCertificate(pfxCertPath, publicCertPath, privateCertkeyPath);
                }
            }
        }

        /// <summary>
        /// Creates the PFX certificate out of PEM certificates
        /// </summary>
        /// <param name="pfxCertPath">The fully qualified path where PFX certificate is to be stored. </param>
        /// <param name="publicCertPath">The fully qualified path of pulic PEM certificate. </param>
        /// <param name="privateCertkey">The fully qualified path of private key file. </param>
        private static void CreatePFXCertificate(string pfxCertPath, string publicCertPath, string privateCertkey)
        {
            var command = $"openssl pkcs12 -export -out {pfxCertPath} -in {publicCertPath} -inkey {privateCertkey} -passout pass:";
            ShellHelper.Bash(command);
        }
    }
}
