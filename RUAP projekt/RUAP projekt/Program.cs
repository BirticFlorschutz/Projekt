// This code requires the Nuget package Microsoft.AspNet.WebApi.Client to be installed.
// Instructions for doing this in Visual Studio:
// Tools -> Nuget Package Manager -> Package Manager Console
// Install-Package Microsoft.AspNet.WebApi.Client

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CallRequestResponseService
{

    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            InvokeRequestResponseService().Wait();
        }

        static async Task InvokeRequestResponseService()
        {
            Console.WriteLine("Date:");
            string date = Console.ReadLine();
            Console.WriteLine("\nTemperature:");
            string temp = Console.ReadLine();
            Console.WriteLine("\nHumidity:");
            string hum = Console.ReadLine();
            Console.WriteLine("\nLight:");
            string light = Console.ReadLine();
            Console.WriteLine("\nCO2:");
            string co2 = Console.ReadLine();
            Console.WriteLine("\nHumidityRatio:");
            string humrat = Console.ReadLine();
            Console.WriteLine("\nOccupancy:");
            string occupancy = Console.ReadLine();
            
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {

                    Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"date", "Temperature", "Humidity", "Light", "CO2", "HumidityRatio", "Occupancy"},
                                Values = new string[,] {  { date, temp, hum, light, co2, humrat, occupancy },  }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };
                const string apiKey = "srXDUcjoBazYE3Mceq95fc+Oh720mtsyyhGlg7tzvpPkWNvrEEca+leyaVmOSNFHMW+tUPLudBywNHoOC3a84Q=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/333de07892bb4258aab23cb3dfe6a0ba/services/7f7b9515b258462096e772278f5cdd53/execute?api-version=2.0&details=true");

                // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)


                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    if (result[result.Length - 10].ToString() == ".")
                    {
                        Console.WriteLine("\nScored label: {0}", result[result.Length - 15]);
                        Console.WriteLine("Scored probability: {0}.{1}{2}", result[result.Length - 11], result[result.Length - 9], result[result.Length - 8]);
                    }
                    else
                    {
                        Console.WriteLine("\nScored label: {0}", result[result.Length - 12]);
                        Console.WriteLine("Scored probability: {0}", result[result.Length - 8]);
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                    Console.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                }
            }
        }
    }
}
