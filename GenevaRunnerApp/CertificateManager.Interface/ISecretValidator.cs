// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISecretValidator.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GenevaRunnerApp
{
    /// <summary>
    /// Interface if the provided value is valid for secret 
    /// </summary>
    /// <typeparam name="T">type of input</typeparam>
    public interface ISecretValidator<T> where T : class
    {
        /// <summary>
        /// Function to validate if the provided input is certificate
        /// </summary>
        /// <param name="input">input value</param>
        /// <returns>true if input is valid for certificate</returns>
        bool ValidateCertificate(T input);

        /// <summary>
        /// Function to validate if the url provided is valid.
        /// </summary>
        /// <param name="url">input url for secret</param>
        /// <returns>true if input url is valid</returns>
        bool IsValidSecretURI(string url);
    }
}
