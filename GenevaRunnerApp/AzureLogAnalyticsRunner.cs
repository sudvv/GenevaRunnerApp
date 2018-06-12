
namespace GenevaRunnerApp
{
    using Microsoft.Azure.OperationalInsights;
    using Microsoft.Azure.OperationalInsights.Models;
    using Microsoft.Cis.Monitoring.Runners;
    using Microsoft.Cloud.InstrumentationFramework;
    using Microsoft.Rest.Azure.Authentication;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;

    public class AzureLogAnalyticsRunner : RunnerAuthorBase
    {
        #region " Variables declaration "
        private int SleepTime { get; set; }

        private X509Certificate2 KvCertificate { get; set; }

        private string AADClientId { get; set; }

        private string AadCertSubjectName { get; set; }

        private string KeyvaultCertficateId { get; set; }

        private string KeyvaultSecretId { get; set; }

        private string MdmAccountName { get; set; }

        private string WorkspaceId { get; set; }

        private string WorkspaceName { get; set; }

        private string ClientSecret { get; set; }

        private string ClientId { get; set; }

        private string TokenAudience { get; set; }

        private string AuthEndpoint { get; set; }

        private string Domain { get; set; }

        public string MdmNamespace { get; set; }

        public string MetricName { get; set; }

        public string Query { get; set; }

        public string FilterCondition { get; set; }

        public Dictionary<string, Quries> QueryData { get; set; }
        public Dictionary<string, Quries> dirQueryResults { get; set; }

        private const string dimensionName1 = "Environment";
        private const string dimensionName2 = "Category";
        private const string dimensionName3 = "Test";
        private const int threadSleepTime = 30000;

        #endregion

        public AzureLogAnalyticsRunner(string[] args)
            : base(args)
        {
            ReadConfigurationValues();
            string clientSecret;
            FetchKeyvaultSecret(out clientSecret);
            this.ClientSecret = clientSecret;
        }

        #region " Private methods "
        /// <summary>
        /// Checking the test passed or not based on filter condition
        /// </summary>
        /// <param name="expectedCondition">expected condition value</param>
        /// <param name="expectedResult">expected result</param>
        /// <param name="currentValue">current query value</param>
        /// <returns>bool</returns>
        private bool TestPassed(string expectedCondition, int expectedResult, int currentValue)
        {
            bool testPassed = false;
            switch (expectedCondition)
            {
                case "eq":
                    {
                        if (currentValue == expectedResult)
                        {
                            testPassed = true;
                        }
                    }
                    break;
                case "lt":
                    {
                        if (currentValue < expectedResult)
                        {
                            testPassed = true;
                        }
                    }
                    break;
                case "gt":
                    {
                        if (currentValue > expectedResult)
                        {
                            testPassed = true;
                        }
                    }
                    break;
                case "lteq":
                    {
                        if (currentValue <= expectedResult)
                        {
                            testPassed = true;
                        }
                    }
                    break;
                case "gteq":
                    {
                        if (currentValue >= expectedResult)
                        {
                            testPassed = true;
                        }
                    }
                    break;
                default:
                    {
                        if (currentValue == expectedResult)
                        {
                            testPassed = true;
                        }
                    }
                    break;
            }
            return testPassed;
        }

        /// <summary>
        /// Log query results
        /// </summary>
        private void LogResults()
        {
            Console.WriteLine($"LogResults Start");
            genareteResultsToMDM();
            Console.WriteLine($"LogResults End");
        }

        /// <summary>
        /// Generating the metrics.
        /// </summary>
        /// <param name="metricName">metric name</param>
        /// <param name="value">metric value</param>
        private void EmitMDMMetric(string testName, string testCategory, long value, string metricName, string dimension3Name = null, string dimension3Value = null)
        {
            try
            {
                ErrorContext mdmError = new ErrorContext();

                // MeasureMetric usage sample
                MeasureMetric3D testMeasure = MeasureMetric3D.Create(
                  MdmAccountName, // Your MDM Account Name
                  MdmNamespace,   // MDM Namespace, can be as specific as needed by separating values with '/'.
                  metricName,     // The Metric name.
                                  // The following dimensions are necessary to identify the runner and instance generating the metrics.
                                  // The values for these dimensions come from the base class RunnerAuthorBase.
                  dimensionName1, // The Worksapce name as Environment name.
                  dimensionName2, // The current test category name.
                  dimension3Name == null ? dimensionName3 : dimension3Name, // The current test/computer name.                  
                  ref mdmError);

                if (testMeasure == null)
                {
                    Console.WriteLine("Fail to create MeasureMetric, error code is {0:X}", mdmError.ErrorCode);
                    Console.WriteLine("    error message: {0}", mdmError.ErrorMessage);
                }
                else
                {
                    // Logs the metric as defined above. 
                    var result = testMeasure.LogValue(
                        value,
                        WorkspaceName,                                         // The Worksapce name as Environment name.
                        testCategory,                                          // The current test category name.
                        dimension3Value == null ? testName : dimension3Value,  // The current test category name.
                        ref mdmError);

                    if (!result)
                    {
                        Console.WriteLine("Fail to set MeasureMetric value, error code is {0:X}", mdmError.ErrorCode);
                        Console.WriteLine("    error message: {0}", mdmError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail to set MeasureMetric value, error Source is {0}", ex.Source);
                Console.WriteLine("    error message: {0}", ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Generating the 4D metrics.
        /// </summary>
        /// <param name="metricName">metric name</param>
        /// <param name="value">metric value</param>
        private void EmitMDMMetric(string testName, string testCategory, long value, string metricName, string dimension3Name, string dimension4Name, string dimension3Value)
        {
            try
            {
                ErrorContext mdmError = new ErrorContext();

                // MeasureMetric usage sample
                MeasureMetric4D testMeasure = MeasureMetric4D.Create(
                  MdmAccountName, // Your MDM Account Name
                  MdmNamespace,   // MDM Namespace, can be as specific as needed by separating values with '/'.
                  metricName,     // The Metric name.
                                  // The following dimensions are necessary to identify the runner and instance generating the metrics.
                                  // The values for these dimensions come from the base class RunnerAuthorBase.
                  dimensionName1, // The Worksapce name as Environment name.
                  dimensionName2, // The current category name.
                  dimension3Name, // The current computer name. 
                  dimension4Name,  // Type like ( KubeEvents_CL or KubeNodeInventory …)
                  ref mdmError);

                if (testMeasure == null)
                {
                    Console.WriteLine("Fail to create MeasureMetric, error code is {0:X}", mdmError.ErrorCode);
                    Console.WriteLine("    error message: {0}", mdmError.ErrorMessage);
                }
                else
                {
                    // Logs the metric as defined above. 
                    var result = testMeasure.LogValue(
                        value,
                        WorkspaceName,       // The Worksapce name as Environment name.
                        testCategory,        // The current category name.
                        dimension3Value,     // The current computer name.
                        testName,            // Type like ( KubeEvents_CL or KubeNodeInventory …)
                        ref mdmError);

                    if (!result)
                    {
                        Console.WriteLine("Fail to set MeasureMetric value, error code is {0:X}", mdmError.ErrorCode);
                        Console.WriteLine("    error message: {0}", mdmError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail to set MeasureMetric value, error Source is {0}", ex.Source);
                Console.WriteLine("    error message: {0}", ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Reading some sample configuration values defined in RunnerDefinition.xml
        /// </summary>
        private void ReadConfigurationValues()
        {
            Console.WriteLine($"ReadConfigurationValues Start");
            KvCertificate = GetCertificateByThumbprint(this.GetConfigurationValue("AadCertThumbprint"));
            AADClientId = this.GetConfigurationValue("AADClientId");
            AadCertSubjectName = this.GetConfigurationValue("AadCertSubjectName");
            KeyvaultCertficateId = this.GetConfigurationValue("KeyvaultCertficateId");
            KeyvaultSecretId = this.GetConfigurationValue("KeyvaultSecretId");
            MdmAccountName = this.GetConfigurationValue("MdmAccountName");
            WorkspaceId = this.GetConfigurationValue("WorkspaceId");
            WorkspaceName = this.GetConfigurationValue("WorkspaceName");
            ClientSecret = this.GetConfigurationValue("ClientSecret");
            ClientId = this.GetConfigurationValue("ClientId");
            TokenAudience = this.GetConfigurationValue("TokenAudience");
            AuthEndpoint = this.GetConfigurationValue("AuthEndpoint");
            Domain = this.GetConfigurationValue("Domain");
            MdmNamespace = this.GetConfigurationValue("MdmNamespace");
            MetricName = this.GetConfigurationValue("MetricName");
            Query = this.GetConfigurationValue("Query");
            FilterCondition = this.GetConfigurationValue("FilterCondition");
            SleepTime = Convert.ToInt32(this.GetConfigurationValue("SleepTime"));
            // Print configuration values
            Console.WriteLine(@"Configuration values provided :- AADClientId = {0} :: AadCertSubjectName = {1} :: KeyvaultCertficateId = {2} :: KeyvaultSecretId = {3} :: MdmAccountName = {4} :: WorkspaceId = {5} :: ClientSecret = {6} :: ClientId = {7} :: TokenAudience = {8} ::  AuthEndpoint = {9} :: Domain = {10} :: Query = {11} :: FilterCondition = {12} :: SleepTime = {13}",
            AADClientId, AadCertSubjectName, KeyvaultCertficateId, KeyvaultSecretId, MdmAccountName, WorkspaceId, ClientSecret, ClientId, TokenAudience, AuthEndpoint, Domain, Query, FilterCondition, SleepTime);
            Console.WriteLine($"ReadConfigurationValues End");

        }

        /// <summary>
        /// Function to fetch secret
        /// </summary>
        /// <returns>true is fetch was successful</returns>
        private void FetchKeyvaultSecret(out string value)
        {
            value = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(KeyvaultSecretId))
                {
                    Console.WriteLine("Key Vault secret id is empty or null.");
                    return;
                }
                // Fetch secret from the keyvault
                AadSettings aadSettings = new AadSettings(AADClientId, AadCertSubjectName);
                AadAccessTokenHandler aadAccessTokenHandler = new AadAccessTokenHandler(aadSettings);
                aadAccessTokenHandler._aadCert = KvCertificate;
                KeyVaultSecretAccessor keyValutSecretAccessor = new KeyVaultSecretAccessor(aadAccessTokenHandler);
                value = keyValutSecretAccessor.GetSecret(KeyvaultSecretId).Result;
                // NEVER Log secret along with the KV path.
                Console.WriteLine($"Fetched kevault secret value for secret id {KeyvaultSecretId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while feaching Keyvault Secret id {KeyvaultSecretId}. Exception {ex.Message}");
            }
        }

        /// <summary>
        /// Fetch keyvault certificate
        /// </summary>
        /// <returns>bool</returns>
        private bool FetchKeyvaultCertificate()
        {
            if (string.IsNullOrWhiteSpace(KeyvaultCertficateId))
            {
                Console.WriteLine("Key Vault certificate id is empty or null.");
                return true;
            }

            Console.WriteLine($"{nameof(FetchKeyvaultCertificate)} called with AAD Client ID '{AADClientId}', AAD Cert Subject Name '{AadCertSubjectName}'");

            // Fetch certificate from the keyvault
            AadSettings aadSettings = new AadSettings(AADClientId, AadCertSubjectName);
            AadAccessTokenHandler aadAccessTokenHandler = new AadAccessTokenHandler(aadSettings);
            aadAccessTokenHandler._aadCert = KvCertificate;
            KeyVaultSecretAccessor keyValutSecretAccessor = new KeyVaultSecretAccessor(aadAccessTokenHandler);
            string kvCertificateString = keyValutSecretAccessor.GetCertificate(KeyvaultCertficateId).Result;

            // NEVER Log secret along with the KV path.
            Console.WriteLine($"Fetched keyvault certificate for key {KeyvaultCertficateId}");

            // Always call validate and metric emission.
            this.ValidateCertificate(kvCertificateString);
            return true;
        }

        /// <summary>
        /// Function to validate the certificate
        /// </summary>
        /// <param name="kvCertificateString">Key Vault Certificate string</param>
        private void ValidateCertificate(string kvCertificateString)
        {
            // install certificate on localbox. There can be multiple utlization of this certificate
            X509Certificate2 kvCertificate = new X509Certificate2(Convert.FromBase64String(kvCertificateString));
            Console.WriteLine($"Created X509Certificate2 certificate");

            string thumbprint = kvCertificate.Thumbprint;
            string subjectName = kvCertificate.SubjectName.Name.Replace("CN=", string.Empty);

            // Here we can persist last installed thumbprint and validate 
            // if the new cert has different thumbprint, then only we need to install.
            Console.WriteLine($"Fetched certificate has Thumbprint: {thumbprint}, subjectName: {subjectName}");

            // DON'T install the certificate.

            // validate 
            if (!string.IsNullOrWhiteSpace(subjectName) &&
                (subjectName.ToLower().Contains("runnersvcaad") ||
                (subjectName.Equals(this.AadCertSubjectName, StringComparison.OrdinalIgnoreCase))))
            {
                throw new ArgumentException($"Runner should not have subject name {subjectName}");
            }
        }

        #endregion

        #region " Public methods "

        /// <summary>
        /// Start runner
        /// </summary>
        public override void StartRunner()
        {
            try
            {
                QueryData = ReadQueryJsonfile("Query.json", FilterCondition);
                if (!string.IsNullOrEmpty(this.ClientSecret))
                {
                    ExecuteLogAnalyticsQueries();
                    LogResults();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while processing... Exception: {ex}");
                Thread.Sleep(SleepTime);
            }

        }

        /// <summary>
        /// Executing the log analytics queries
        /// </summary>
        public void ExecuteLogAnalyticsQueries()
        {
            Console.WriteLine($"ExecuteLogAnalyticsQueries Start");
            var adSettings = new ActiveDirectoryServiceSettings
            {
                AuthenticationEndpoint = new Uri(AuthEndpoint),
                TokenAudience = new Uri(TokenAudience),
                ValidateAuthority = true
            };

            var creds = ApplicationTokenProvider.LoginSilentAsync(Domain, AADClientId, ClientSecret, adSettings).GetAwaiter().GetResult();
            var client = new OperationalInsightsDataClient(creds);
            client.WorkspaceId = WorkspaceId;
            dirQueryResults = new Dictionary<string, Quries>();
            bool retry = true;
            int retryCount = 0;
            QueryResults results = null;
            foreach (KeyValuePair<string, Quries> item in QueryData)
            {
                try
                {
                    try
                    {
                        while (retry && retryCount <= 2)
                        {
                            results = client.Query(item.Value.Query);
                            retry = false;
                        }
                    }
                    catch (Exception e)
                    {
                        retryCount++;
                        Console.WriteLine($"Error while processing the query name {item.Key} retry count {retryCount}, Exception: {e}");
                        Thread.Sleep(threadSleepTime);
                        client = new OperationalInsightsDataClient(creds);
                        client.WorkspaceId = WorkspaceId;
                    }
                    retry = true;
                    retryCount = 0;
                    if (results != null && results.Tables.Count > 0)
                    {
#if DEBUG
                        //string rowformat = "{0},{1}";
                        //string rowData = string.Empty;
                        //foreach (var dr in results.Tables[0].Rows)
                        //{
                        //    foreach (string dc in dr)
                        //    {
                        //        if (dc != null)
                        //            rowData = rowData == "" ? dc : string.Format(rowformat, rowData, dc);
                        //    }
                        //    rowData = string.Empty;
                        //}
#endif
                        if (!string.IsNullOrEmpty(item.Value.Summarize))
                        {
                            item.Value.PerfResult = null;
                            Dictionary<string, double> PerfResult = new Dictionary<string, double>();
                            for (int i = 0; i < results.Tables[0].Rows.Count; i++)
                            {
                                if (results.Tables[0].Rows[i].Count > 1 && !string.IsNullOrEmpty(results.Tables[0].Rows[i][0]))
                                {
                                    //Console.WriteLine($"{item.Value.Category} Result Name: {results.Tables[0].Rows[i][0]} Result Value: {results.Tables[0].Rows[i][1]} == time :{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")} UTC Time: {DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")}");
                                    PerfResult.Add(results.Tables[0].Rows[i][0], Convert.ToDouble(results.Tables[0].Rows[i][1]));
                                }
                            }
                            item.Value.PerfResult = PerfResult;
                        }

                        item.Value.Result = results.Tables[0].Rows.Count.ToString();

                    }
                    else
                    {
                        item.Value.Result = "No Data";
                        Console.WriteLine("No Data");
                    }
                }
                catch (Exception ex)
                {
                    item.Value.Result = "Error";
                    Console.WriteLine($"Error while processing the query name {item.Key} Exception: {ex}");
                }
                results = null;
                dirQueryResults.Add(item.Key, item.Value);
            }
            Console.WriteLine($"ExecuteLogAnalyticsQueries End");
        }

        /// <summary>
        /// Genareting the query results for MDM
        /// </summary>
        public void genareteResultsToMDM()
        {

            bool testPassed = true;
            string category = string.Empty;
            Console.WriteLine("Enveronment " + this.Environment);
            foreach (KeyValuePair<string, Quries> item in dirQueryResults)
            {
                try
                {
                    Console.WriteLine($"{item.Value.Category} Test Name: {item.Value.Name} == time :{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")} UTC Time: {DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")}");

                    testPassed = true;
                    if (item.Value.Result == "No Data" || item.Value.Result == "Error")
                    {
                        testPassed = false;
                        EmitMDMMetric(item.Key, item.Value.Category, testPassed ? 1 : 0, "TestResult");
                    }
                    else if (!string.IsNullOrEmpty(item.Value.Summarize))
                    {
                        if (item.Value.PerfResult.Count > 0)
                        {
                            testPassed = true;
                            foreach (KeyValuePair<string, double> resultItem in item.Value.PerfResult)
                            {
                                if (!string.IsNullOrEmpty(resultItem.Key))
                                {
                                    Console.WriteLine($"{item.Value.Category} Computer Name: {resultItem.Key},  Result: {testPassed}, RecordsCount: {resultItem.Value}, ExpectedResult:{item.Value.ExpectedResult} {item.Value.ExpectedCondition}");
                                    if (item.Value.Dimensions == 4)
                                    {
                                        EmitMDMMetric(item.Key, item.Value.Category, (long)resultItem.Value, "CountOfType", "Computer", "Type", resultItem.Key);
                                    }
                                    else
                                    {
                                        EmitMDMMetric(item.Key, item.Value.Category, (long)resultItem.Value, item.Key, "Computer", resultItem.Key);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        testPassed = TestPassed(item.Value.ExpectedCondition, Convert.ToInt32(item.Value.Result), Convert.ToInt32(item.Value.ExpectedResult));
                        EmitMDMMetric(item.Key, item.Value.Category, testPassed ? 1 : 0, "TestResult");
                    }
                    if (category != item.Value.Category)
                    {
                        Console.WriteLine($"Category: {item.Value.Category}");
                        category = item.Value.Category;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Processing : {item.Value.Name} Exception: {ex}");
                    testPassed = false;
                }


            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Query json file path</param>
        /// <param name="filterCondition">default filter condition</param>
        /// <returns></returns>
        public Dictionary<string, Quries> ReadQueryJsonfile(string path, string filterCondition)
        {
            Dictionary<string, Quries> quries = new Dictionary<string, Quries>();
            string jsonData = File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<JsonRoot>(jsonData);

            foreach (var aq in data.AllQuries)
            {
                foreach (var q in aq.Quries)
                {
                    q.Category = aq.Ctegory;
                    q.Query = q.FilterCondition == "" ? q.Query + filterCondition : q.Query + q.FilterCondition;
                    q.Query = q.Query + q.Summarize;
                    Console.WriteLine("Query Name:{0} :: Query:{1}", q.Name, q.Query);
                    quries.Add(q.Name, q);
                }
            }

            return quries;
        }
        #endregion
    }
}
