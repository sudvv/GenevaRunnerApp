// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyVaultSecretValidator.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GenevaRunnerApp
{
    using Microsoft.Azure.KeyVault.Models;
    using System;

    /// <summary>
    /// Class to validate if the provided value is valid for secret 
    /// </summary>
    public class KeyVaultSecretValidator : ISecretValidator<SecretBundle>
    {
        /// <summary>
        /// Function to validate if the provided input is certificate
        /// </summary>
        /// <param name="input">input value</param>
        /// <returns>true if input is valid for certificate</returns>
        public bool ValidateCertificate(SecretBundle input)
        {
            if (input == null || string.IsNullOrEmpty(input.Value))
            {
                Console.WriteLine($"Input string is empty or null.");
                return false;
            }

            if ((input.ContentType != "application/x-pkcs12") && (input.ContentType != "application/x-pem-file"))
            {
                string errorMessage =
                    string.Format($"Certificate retrieved from Keyvault is not type application/x-pkcs12 for secret id {input.Id}");
                Console.WriteLine(errorMessage);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Function to validate if the url provided is valid.
        /// </summary>
        /// <param name="uri">input url for secret</param>
        /// <returns>true if input url is valid</returns>
        public bool IsValidSecretURI(string uri)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                return false;
            Uri tmp;
            if (!Uri.TryCreate(uri, UriKind.Absolute, out tmp))
                return false;
            return tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps;
        }
    }
}
