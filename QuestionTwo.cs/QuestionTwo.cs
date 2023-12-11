using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class CrestronNVXInfo
{
    static async Task Main(string[] args)
    {
        // Collect necessary information from the user
        Console.WriteLine("Enter the IP Address of the Crestron NVX device:");
        string ipAddress = Console.ReadLine()!;

        Console.WriteLine("Enter the Username:");
        string username = Console.ReadLine()!;

        Console.WriteLine("Enter the Password:");
        string password = Console.ReadLine()!;

        try
        {
            // Construct the base URL for API requests
            string baseUrl = $"https://{ipAddress}/api/rest/";

            // Handle potential certificate issues for HTTPS requests
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            using (HttpClient client = new HttpClient(handler))
            {
                // Begin a session - Send GET request to login page to get TRACKID cookie
                HttpResponseMessage loginPageResponse = await client.GetAsync(baseUrl + "userlogin.html");

                if (loginPageResponse.IsSuccessStatusCode)
                {
                    string? trackIdCookie = loginPageResponse.Headers.GetValues("Set-Cookie")?.ToString();

                    if (trackIdCookie != null)
                    {
                        // Formulate and send POST request to perform login with provided credentials
                        var loginRequest = new HttpRequestMessage(HttpMethod.Post, baseUrl + "userlogin.html");
                        loginRequest.Headers.Add("Cookie", trackIdCookie);
                        loginRequest.Headers.Add("Origin", ipAddress);
                        loginRequest.Headers.Add("Referer", $"{ipAddress}/userlogin.html");

                        var loginData = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("login", username),
                            new KeyValuePair<string, string>("passwd", password)
                        });

                        loginRequest.Content = loginData;

                        HttpResponseMessage loginResponse = await client.SendAsync(loginRequest);

                        if (loginResponse.IsSuccessStatusCode)
                        {
                            // Authentication succeeded, continue with API call to fetch sync status
                            HttpResponseMessage syncStatusResponse = await client.GetAsync(baseUrl + "/Device/AudioVideoInputOutput/Inputs/x/Ports/x/");
                            // Update 'x' with the appropriate index or value for your device

                            if (syncStatusResponse.IsSuccessStatusCode)
                            {
                                string syncStatusContent = await syncStatusResponse.Content.ReadAsStringAsync();

                                // Parse syncStatusContent to get sync status information using System.Text.Json
                                var syncStatusObject = JsonSerializer.Deserialize<SyncStatusResponse>(syncStatusContent);

                                 if (syncStatusObject != null)
                                {
                                    Console.WriteLine($"Is Sync Detected: {syncStatusObject.IsSyncDetected}");
                                    Console.WriteLine($"Horizontal Resolution: {syncStatusObject.HorizontalResolution}");
                                    Console.WriteLine($"Vertical Resolution: {syncStatusObject.VerticalResolution}");
                                }
                                else
                                {
                                Console.WriteLine("Error: Deserialized object is null.");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Error fetching sync status: {syncStatusResponse.StatusCode} - {syncStatusResponse.ReasonPhrase}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Authentication failed: {loginResponse.StatusCode} - {loginResponse.ReasonPhrase}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Error accessing login page: {loginPageResponse.StatusCode} - {loginPageResponse.ReasonPhrase}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // Define a class to represent the structure of the sync status JSON response
    public class SyncStatusResponse
    {
        public bool IsSyncDetected { get; set; }
        public int HorizontalResolution { get; set; }
        public int VerticalResolution { get; set; }
        // Add other properties based on the actual response structure
    }
}
