// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISecretAccessor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GenevaRunnerApp
{
    /// <summary>
    /// Interface to access secret value based on provided input.
    /// There will be implementations specific to Keyvault and DSMS.
    /// TODO: To check if this method can become async.
    /// </summary>
    public interface ISecretAccessor<TInput, TOutput>
        where TInput : class
        where TOutput : class
    {
        /// <summary>
        /// Function to access secret value
        /// </summary>
        /// <param name="secretParameters">Input parameters for accessing secret information/param>
        /// <returns>Secret information for output</returns>
        TOutput GetSecret(TInput secretParameters);

        /// <summary>
        /// Function to access certificate value
        /// </summary>
        /// <param name="secretParameters">Input parameters for accessing secret information/param>
        /// <returns>Secret information for output</returns>
        TOutput GetCertificate(TInput secretParameters);
    }
}