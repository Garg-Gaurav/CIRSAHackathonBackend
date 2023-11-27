using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class SmokeTest
{
    private readonly HttpClient httpClient = new HttpClient();
    private readonly string baseUrl;
    public SmokeTest(string baseUrl)
    {
        this.baseUrl = baseUrl;

    }
    public async Task TestApiGetRequest(string requestUri)
    {

        var stopwatch = new Stopwatch();

        try
        {
            stopwatch.Start();
            var response = await httpClient.GetAsync(requestUri);
            stopwatch.Stop();
            //Console.WriteLine($"Status Code: {response.Content}");
            Console.WriteLine($"Status Code: {response.StatusCode}");
            Console.WriteLine($"Response Time: {stopwatch.ElapsedMilliseconds} ms");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("Response Content:");
                //Console.WriteLine(content);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
        }
    }

    public async Task TestApiGetRequestWithId(string baseUrl, string id)
    {
        var requestUri = $"{baseUrl}/{id}"; // Append the ID to the URL
        var stopwatch = new Stopwatch();

        try
        {
            stopwatch.Start();
            var response = await httpClient.GetAsync(requestUri);
            stopwatch.Stop();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Data Found (200 OK)");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine("Data Not Found (404 Not Found)");
            }
            else
            {
                Console.WriteLine($"API GET Request with ID failed with status code: {response.StatusCode}");
            }

            Console.WriteLine($"Response Time for ID {id}: {stopwatch.ElapsedMilliseconds} ms");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
        }
    }

    public async Task TestApiPutRequest(string baseUrl, string id, GameData data)
    {
        var requestUri = $"{baseUrl}/{id}"; // Append the ID to the URL
        var stopwatch = new Stopwatch();

        try
        {
            var jsonContent = System.Text.Json.JsonSerializer.Serialize(data);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            stopwatch.Start();
            var response = await httpClient.PutAsync(requestUri, content);
            stopwatch.Stop();
            Console.WriteLine(response.StatusCode);
            Console.WriteLine($"Status Code for PUT request: {response.StatusCode}");
            Console.WriteLine($"Response Time for PUT request: {stopwatch.ElapsedMilliseconds} ms");

            // Optionally, read and display the response content
            // var responseContent = await response.Content.ReadAsStringAsync();
            // Console.WriteLine($"Response Content for PUT request: {responseContent}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
        }
    }

    public async Task TestApiDeleteRequest(string baseUrl, string id)
    {
        var requestUri = $"{baseUrl}/{id}"; // Append the ID to the URL
        var stopwatch = new Stopwatch();

        try
        {
            stopwatch.Start();
            var response = await httpClient.DeleteAsync(requestUri);
            stopwatch.Stop();

            Console.WriteLine($"Status Code for DELETE request: {response.StatusCode}");
            Console.WriteLine($"Response Time for DELETE request: {stopwatch.ElapsedMilliseconds} ms");

            // Optionally, read and display the response content
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Content for DELETE request: {responseContent}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
        }
    }

    public async Task TestApiPostRequest(string requestUri, GameData data)
    {
        var stopwatch = new Stopwatch();

        try
        {
            var jsonContent = System.Text.Json.JsonSerializer.Serialize(data);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            stopwatch.Start();
            var response = await httpClient.PostAsync(requestUri, content);
            stopwatch.Stop();

            Console.WriteLine($"Status Code for POST request: {response.StatusCode}");
            Console.WriteLine($"Response Time for POST request: {stopwatch.ElapsedMilliseconds} ms");

            // Optionally, read and display the response content
            // var responseContent = await response.Content.ReadAsStringAsync();
            // Console.WriteLine($"Response Content for POST request: {responseContent}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
        }
    }
}

