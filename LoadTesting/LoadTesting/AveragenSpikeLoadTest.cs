using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

public class AvgnSpikeLoadTester
{
    private readonly HttpClient httpClient = new HttpClient();
    private readonly string baseUrl;
    private List<(TimeSpan duration, int target)> stages;

    public AvgnSpikeLoadTester(string baseUrl, List<(TimeSpan duration, int target)> stages)
    {
        this.baseUrl = baseUrl;
        this.stages = stages;
    }
    public async Task StartGeneralGetTest()
    {
        foreach (var stage in stages)
        {
            Console.WriteLine($"Starting General GET stage: Duration {stage.duration.TotalMinutes} minutes, Target Users = {stage.target}");
            double averageResponseTime = await RunStage(stage.duration, stage.target, SimulateGeneralGetRequest);
            Console.WriteLine($"Average response time for General GET stage: {averageResponseTime} ms");
        }
    }

    public async Task StartGetTestWithId(string id)
    {

        foreach (var stage in stages)
        {
            Console.WriteLine($"Starting GET with ID stage: Duration {stage.duration.TotalMinutes} minutes, Target Users = {stage.target}");
            double averageResponseTime = await RunStage(stage.duration, stage.target, () => SimulateUserWithId(id));
            Console.WriteLine($"Average response time for GET with ID stage: {averageResponseTime} ms");
        }
    }

    public async Task StartPutTestWithId(string id, GameData gameData)
    {

        foreach (var stage in stages)
        {
            Console.WriteLine($"Starting PUT with ID stage: Duration {stage.duration.TotalMinutes} minutes, Target Users = {stage.target}");
            double averageResponseTime = await RunStage(stage.duration, stage.target, () => SimulatePutRequestWithId(id, gameData));
            Console.WriteLine($"Average response time for PUT with ID stage: {averageResponseTime} ms");
        }
    }

    public async Task StartPostTest(GameData gameData)
    {

        foreach (var stage in stages)
        {
            Console.WriteLine($"Starting POST stage: Duration {stage.duration.TotalMinutes} minutes, Target Users = {stage.target}");
            double averageResponseTime = await RunStage(stage.duration, stage.target, () => SimulatePostRequest(gameData));
            Console.WriteLine($"Average response time for POST stage: {averageResponseTime} ms");
        }
    }

    public async Task StartDeleteTestWithId(string id)
    {

        foreach (var stage in stages)
        {
            Console.WriteLine($"Starting DELETE with ID stage: Duration {stage.duration.TotalMinutes} minutes, Target Users = {stage.target}");
            double averageResponseTime = await RunStage(stage.duration, stage.target, () => SimulateDeleteRequestWithId(id));
            Console.WriteLine($"Average response time for DELETE with ID stage: {averageResponseTime} ms");
        }
    }

    private async Task<double> RunStage(TimeSpan duration, int targetUserCount, Func<Task<long>> simulateRequest)
    {
        var tasks = new List<Task<long>>();
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        while (stopwatch.Elapsed < duration)
        {
            while (tasks.Count < targetUserCount)
            {
                tasks.Add(simulateRequest());
            }

            // Remove completed tasks
            tasks.RemoveAll(task => task.IsCompleted);

            await Task.Delay(1000); // 1 second delay between checks
        }

        var completedTasks = await Task.WhenAll(tasks);
        long totalResponseTime = 0;
        foreach (var time in completedTasks)
        {
            totalResponseTime += time;
        }

        return completedTasks.Length > 0 ? (double)totalResponseTime / completedTasks.Length : 0.0;
    }

    private async Task<long> SimulateGeneralGetRequest()
    {
        var requestUri = baseUrl;
        return await SendRequestAndGetResponseTime(requestUri, HttpMethod.Get);
    }

    private async Task<long> SimulateUserWithId(string id)
    {
        var requestUri = $"{baseUrl}/{id}";
        return await SendRequestAndGetResponseTime(requestUri, HttpMethod.Get);
    }
    

    private async Task<long> SimulatePutRequestWithId(string id, GameData gameData)
    {
        var requestUri = $"{baseUrl}/{id}";
        var jsonContent = JsonSerializer.Serialize(gameData);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        return await SendRequestAndGetResponseTime(requestUri, HttpMethod.Put, content);
    }

    private async Task<long> SimulatePostRequest(GameData gameData)
    {
        var jsonContent = JsonSerializer.Serialize(gameData);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        return await SendRequestAndGetResponseTime(baseUrl, HttpMethod.Post, content);
    }

    private async Task<long> SimulateDeleteRequestWithId(string id)
    {
        var requestUri = $"{baseUrl}/{id}";
        return await SendRequestAndGetResponseTime(requestUri, HttpMethod.Delete);
    }



    private async Task<long> SendRequestAndGetResponseTime(string requestUri, HttpMethod httpMethod, HttpContent content = null)
    {
        var request = new HttpRequestMessage(httpMethod, requestUri) { Content = content };
        var stopwatch = new Stopwatch();

        try
        {
            stopwatch.Start();
            var response = await httpClient.SendAsync(request);
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"Error during {httpMethod} request to {requestUri}: {ex.Message}");
            return stopwatch.ElapsedMilliseconds;
        }
    }


}
