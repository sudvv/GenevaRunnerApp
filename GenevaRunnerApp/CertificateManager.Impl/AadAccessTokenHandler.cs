// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AadAccessTokenHandler.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GenevaRunnerApp
{
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using System;

    /// <summary>
    /// Implementation of how access token from AAD is handled with a cert authentication.
    /// </summary>
    public class AadAccessTokenHandler : IAadAccessTokenHandler
    {
        /// <summary>
        /// object to access certificate from local box.
        /// </summary>
        ICertificateLoader<X509Certificate2> certificateLoader;

        /// <summary>
        /// objet for holding AAD settings
        /// </summary>
        public AadSettings _aadSettings { get; set; }

        public X509Certificate2 _aadCert { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="AadAccessTokenHandler"/> class.
        /// This constructor will be used in testing
        /// </summary>
        public AadAccessTokenHandler(AadSettings settings)
        {
            _aadSettings = settings;
            certificateLoader = new CertificateLoader();
        }

        /// <summary>
        /// Function to get AAD access token
        /// </summary>
        /// <param name="authority">AAD authority</param>
        /// <param name="resource">AAD resource</param>
        /// <param name="scope">AAD scope</param>
        /// <returns>Access token</returns>
        public async Task<string> GetAccessTokenAsync(string authority, string resource, string scope)
        {
            Console.WriteLine($"{nameof(GetAccessTokenAsync)} called with authority '{authority}', resource '{resource}', scope '{scope}'");

            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);

            X509Certificate2 aadCert = _aadCert;
            if (aadCert == null)
                aadCert = !string.IsNullOrEmpty(_aadSettings.CertificateThumbprint) ? this.certificateLoader.GetCertificateByThumprint(_aadSettings.CertificateThumbprint) : this.certificateLoader.GetCertificateBySubjectName(this._aadSettings.CertificateSubjectName);
            Console.WriteLine($"Loaded certificate with subject name '{aadCert.SubjectName.Name}', thumbprint '{aadCert.Thumbprint}'");

            var assertionCert = new ClientAssertionCertificate(this._aadSettings.ClientId, aadCert);
            var result = await context.AcquireTokenAsync(resource, assertionCert);
            return result.AccessToken;
        }
    }
}
