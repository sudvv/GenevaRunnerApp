// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AadSettings.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GenevaRunnerApp
{
    /// <summary>
    /// Class to hold AAD settings
    /// </summary>
    public class AadSettings
    {
        /// <summary>
        /// AAD client ID
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Certificate Subject name
        /// </summary>
        public string CertificateSubjectName { get; set; }

        /// <summary>
        /// Certificate Subject name
        /// </summary>
        public string CertificateThumbprint { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AadSettings"/> class.
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <param name="subjectName">subjectName</param>
        public AadSettings(string clientId, string subjectName)
        {
            this.ClientId = clientId;
            this.CertificateSubjectName = subjectName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AadSettings"/> class.
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <param name="subjectName">subjectName</param>
        /// <param name="thumbprint">thumbprint</param>
        public AadSettings(string clientId, string subjectName, string thumbprint)
        {
            this.ClientId = clientId;
            this.CertificateSubjectName = subjectName;
            this.CertificateThumbprint = thumbprint;
        }
    }
}
