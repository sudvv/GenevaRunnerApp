// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICertificateLoader.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GenevaRunnerApp
{
    /// <summary>
    /// Interface to load local box certificates.
    /// </summary>
    public interface ICertificateLoader<T> where T : class
    {
        /// <summary>
        /// Function to get certificate based on thumbprint.
        /// </summary>
        /// <param name="thumbprint"></param>
        /// <returns>returns certificate object</returns>
        T GetCertificateByThumprint(string thumbprint);

        /// <summary>
        /// Function to get certificate based on thumbprint.
        /// </summary>
        /// <param name="subjectName"></param>
        /// <returns>returns certificate object</returns>
        T GetCertificateBySubjectName(string subjectName);
    }
}
