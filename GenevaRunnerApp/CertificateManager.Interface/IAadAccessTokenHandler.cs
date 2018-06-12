// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAadAccessTokenHandler.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GenevaRunnerApp
{
    using System.Threading.Tasks;

    /// <summary>
    /// An interface containing method how to handle access token from AAD to be used by KeyVault.
    /// </summary>
    public interface IAadAccessTokenHandler
    {
        /// <summary>
        /// Function to get AAD access token
        /// </summary>
        /// <param name="authority">AAD authority</param>
        /// <param name="resource">AAD resource</param>
        /// <param name="scope">AAD scope</param>
        /// <returns>Access token</returns>
        Task<string> GetAccessTokenAsync(string authority, string resource, string scope);
    }
}
