using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenevaRunnerApp;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Threading;

namespace GenevaRunnerAppTest
{
    [TestClass]
    public class AzureLogAnalyticsRunnerTest
    {
        public Dictionary<string, Quries> QueryResults { get; set; }

        [TestMethod]
        public void ExecuteLogAnalyticsQueriesTest()
        {
            string[] args = new string[3];

            #region "Mock Data" 
            args[0] = "runnername:InfraInsights_AzureLogAnalyticsRunnerProdTest";
            args[1] = "runnerinstance:aks-noload-eastus";
            args[2] = "area:ProdTest";
            System.Environment.SetEnvironmentVariable("RunnerName", "InfraInsights_AzureLogAnalyticsRunnerProdTest");
            System.Environment.SetEnvironmentVariable("RunnerInstance", "aks-noload-eastus");
            System.Environment.SetEnvironmentVariable("Area", "ProdTest");
            #endregion

            AzureLogAnalyticsRunner runnerInstance = new AzureLogAnalyticsRunner(args);
            runnerInstance.QueryData = runnerInstance.ReadQueryJsonfile("Query.json", runnerInstance.FilterCondition);
            int loop = 0;
            bool testPassed = true;

            while (true)
            {
                runnerInstance.ExecuteLogAnalyticsQueries();
                QueryResults = runnerInstance.dirQueryResults;
                //runnerInstance.genareteResultsToMDM();
                testPassed = true;
                string category = string.Empty;
                List<string> sbResults = new List<string>();
                List<string> sbResultsPerComputer = new List<string>();
                sbResults.Add("Enveronment " + args[2]);
                foreach (KeyValuePair<string, Quries> item in QueryResults)
                {
                    testPassed = true;
                    if (item.Value.Result == "No Data" || item.Value.Result == "Error")
                    {
                        testPassed = false;
                    }
                    if (item.Value.Category == "Perf")
                    {
                        if (item.Value.PerfResult.Count > 0)
                        {
                            testPassed = true;
                            foreach (KeyValuePair<string, double> resultItem in item.Value.PerfResult)
                            {
                                sbResults.Add($"        {item.Value.Category} Test Name: {resultItem.Key},  Result: {testPassed}, RecordsCount: {resultItem.Value}");
                            }
                        }
                    }
                    else
                    {
                        testPassed = false;
                        switch (item.Value.ExpectedCondition)
                        {
                            case "eq":
                                {
                                    if (Convert.ToInt32(item.Value.Result) == Convert.ToInt32(item.Value.ExpectedResult))
                                    {
                                        testPassed = true;
                                    }
                                }
                                break;
                            case "lt":
                                {
                                    if (Convert.ToInt32(item.Value.Result) < Convert.ToInt32(item.Value.ExpectedResult))
                                    {
                                        testPassed = true;
                                    }
                                }
                                break;
                            case "gt":
                                {
                                    if (Convert.ToInt32(item.Value.Result) > Convert.ToInt32(item.Value.ExpectedResult))
                                    {
                                        testPassed = true;
                                    }
                                }
                                break;
                            case "lteq":
                                {
                                    if (Convert.ToInt32(item.Value.Result) <= Convert.ToInt32(item.Value.ExpectedResult))
                                    {
                                        testPassed = true;
                                    }
                                }
                                break;
                            case "gteq":
                                {
                                    if (Convert.ToInt32(item.Value.Result) >= Convert.ToInt32(item.Value.ExpectedResult))
                                    {
                                        testPassed = true;
                                    }
                                }
                                break;
                            default:
                                {
                                    if (item.Value.Result == item.Value.ExpectedResult)
                                    {
                                        testPassed = true;
                                    }
                                }
                                break;
                        }
                    }
                    if (category != item.Value.Category)
                    {
                        sbResults.Add($"    Category: {item.Value.Category}");
                        category = item.Value.Category;
                    }
                    if (item.Value.Category == "Perf")
                    {
                        foreach (KeyValuePair<string, double> resultItem in item.Value.PerfResult)
                        {
                            if ("aks-agentpool-39959840-0" == resultItem.Key)
                            {
                                sbResultsPerComputer.Add($"{resultItem.Key} :{DateTime.Now}: {resultItem.Value}");
                            }
                            sbResults.Add($"        Pref Test Name: {resultItem.Key},  Result: {testPassed}, RecordsCount: {resultItem.Value}");
                        }
                    }
                    else
                    {
                        sbResults.Add($"        Test Name: {item.Value.Name},  Result: {testPassed}, RecordsCount: {item.Value.Result}, Date: {DateTime.Now}");
                    }
                }

                File.AppendAllLines("../../TestResults/TestResultsAll.txt", sbResults.ToArray());
                File.AppendAllLines("../../TestResults/TestResults.txt", sbResultsPerComputer.ToArray());
                loop++;
                Thread.Sleep(50000);
            }
            Assert.AreEqual(true, testPassed);
        }



    }
}
