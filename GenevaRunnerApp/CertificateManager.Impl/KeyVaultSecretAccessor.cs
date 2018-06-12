// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyVaultSecretAccessor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GenevaRunnerApp
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Rest.Azure;
    using Microsoft.Azure.KeyVault.Models;

    /// <summary>
    /// Keyvault implementation to access secret value based on provided input.
    /// </summary>
    public class KeyVaultSecretAccessor : ISecretAccessor<string, Task<string>>
    {
        /// <summary>
        /// Key vault client
        /// </summary>
        private KeyVaultClient _keyVaultClient;

        /// <summary>
        /// Object for AAD access token handler
        /// </summary>
        private IAadAccessTokenHandler _aadAccessTokenHandler;

        /// <summary>
        /// object for secret validator
        /// </summary>
        private ISecretValidator<SecretBundle> _secretValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyVaultSecretAccessor"/> class.
        /// This constructor is introduced to increase testability
        /// </summary>
        public KeyVaultSecretAccessor(IAadAccessTokenHandler aadAccessTokenHandler)
        {
            _aadAccessTokenHandler = aadAccessTokenHandler;
            _keyVaultClient = new KeyVaultClient(_aadAccessTokenHandler.GetAccessTokenAsync);
            _secretValidator = new KeyVaultSecretValidator();
        }

        /// <summary>
        /// Function to access secret value
        /// </summary>
        /// <param name="secretPath">Input parameters for accessing secret information/param>
        /// <returns>Secret information for output</returns>
        public async Task<string> GetSecret(string secretPath)
        {
            SecretBundle secretBundle = await this.GetSecretBundle(secretPath);
            if (secretBundle != null)
            {
                return secretBundle.Value;
            }

            return null;
        }

        /// <summary>
        /// Function to access certificate value
        /// </summary>
        /// <param name="secretParameters">Input parameters for accessing secret information/param>
        /// <returns>Secret information for output</returns>
        public async Task<string> GetCertificate(string secretPath)
        {
            SecretBundle secretBundle = await this.GetSecretBundle(secretPath);
            if (_secretValidator.ValidateCertificate(secretBundle))
            {
                return secretBundle.Value;
            }

            return null;
        }

        /// <summary>
        /// Function to access secret value
        /// </summary>
        /// <param name="keyPath">Input parameters for accessing secret information/param>
        /// <returns>Secret information for output</returns>
        public async Task<KeyBundle> GetKey(string keyPath)
        {
            try
            {
                if (_secretValidator.IsValidSecretURI(keyPath))
                {
                    KeyBundle keyBundle = await _keyVaultClient.GetKeyAsync(keyPath);
                    return keyBundle;
                }
                else
                {
                    throw new ArgumentException($"{keyPath} is invalid");
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Error while getting secret for path {keyPath}");
                throw;
            }
        }

        /// <summary>
        /// Function to access certificate value
        /// </summary>
        /// <param name="secretParameters">Input prameters for accessing secret information/param>
        /// <returns>Secret information for output</returns>
        private async Task<SecretBundle> GetSecretBundle(string secretPath)
        {
            try
            {
                if (_secretValidator.IsValidSecretURI(secretPath))
                {
                    SecretBundle secretBundle = await _keyVaultClient.GetSecretAsync(secretPath);
                    return secretBundle;

                    // Based on the scenario, it can be thought out to introdcue the cache to keep older values and auto rotation.
                }
                else
                {
                    throw new ArgumentException($"{secretPath} is invalid");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while getting secret for path {secretPath}. " + ex.Message);
                throw;
            }
        }
    }
}
