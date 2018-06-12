using Microsoft.Azure.OperationalInsights;
using Microsoft.Rest.Azure.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenevaRunnerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
#if DEBUG
                if (args.Length == 0)
                {
                    //args = new string[3];
                    //args[0] = "runnername:InfraInsights_AzureLogAnalyticsRunnerProd";
                    //args[1] = "runnerinstance:aks-noload-eastus";
                    //args[2] = "area:Prod";
                    //System.Environment.SetEnvironmentVariable("RunnerName", "InfraInsights_AzureLogAnalyticsRunnerProd");
                    //System.Environment.SetEnvironmentVariable("RunnerInstance", "aks-noload-eastus");
                    //System.Environment.SetEnvironmentVariable("Area", "Prod");
                }
#endif
                // Starts the runner, it is expected that you handle the execution inside of the runner via an infinite loop
                // with proper wait time.
                AzureLogAnalyticsRunner runnerInstance = new AzureLogAnalyticsRunner(args);
                runnerInstance.StartRunner();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed with exception: {ex.Message}");
            }
        }
    }
}
