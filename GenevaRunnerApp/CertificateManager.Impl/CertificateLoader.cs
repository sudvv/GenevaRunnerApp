// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CertificateLoader.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GenevaRunnerApp
{
    using System;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// Class to load local box certificates.
    /// </summary>
    public class CertificateLoader : ICertificateLoader<X509Certificate2>
    {
        /// <summary>
        /// Function to get certificate based on thumbprint.
        /// </summary>
        /// <param name="thumbprint"></param>
        /// <returns>returns certificate object</returns>
        public X509Certificate2 GetCertificateByThumprint(string thumbprint)
        {
            return this.GetCertificateByIdentifier(thumbprint, X509FindType.FindByThumbprint, StoreName.My, StoreLocation.CurrentUser);
        }

        /// <summary>
        /// Function to get certificate based on thumbprint.
        /// </summary>
        /// <param name="subjectName"></param>
        /// <returns>returns certificate object</returns>
        public X509Certificate2 GetCertificateBySubjectName(string subjectName)
        {
            return this.GetCertificateByIdentifier(subjectName, X509FindType.FindBySubjectName, StoreName.My, StoreLocation.CurrentUser);
        }

        /// <summary>
        /// Function to get certificate based on indetifier.
        /// </summary>
        /// <param name="identifier">identifier by which certificate will be accessed</param>
        /// <param name="identifierType">typoe of identifier</param>
        /// <param name="storeName">store name</param>
        /// <param name="storeLocation">store location</param>
        /// <returns>certificate object</returns>
        public X509Certificate2 GetCertificateByIdentifier(string identifier, X509FindType identifierType, StoreName storeName, StoreLocation storeLocation)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException(nameof(identifier));
            }

            X509Store store = new X509Store(storeName, storeLocation);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection col = null;
                col = store.Certificates.Find(identifierType, identifier, false);

                if (col == null || col.Count == 0)
                {
                    Console.WriteLine($"Certificate is not available on machine for identifier {identifier}");
                    return null;
                }

                return col[0];
            }
            finally
            {
                store.Close();
            }
        }
    }
}