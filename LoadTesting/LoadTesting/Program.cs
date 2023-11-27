using System;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json;



class Program
{
    static readonly HttpClient httpClient = new HttpClient();
    static string apiEndpoint = "https://localhost:7170/GameData";

    static async Task Main(string[] args)
    {
        Console.WriteLine("Select the type of test to run:");
        Console.WriteLine("1: Smoke Tests");
        Console.WriteLine("2: Average Load Tests");
        Console.WriteLine("3: Spike Tests");


        string input = Console.ReadLine();

        switch (input)
        {

            case "1":
                await RunSmokeTests();
                break;
            case "2":
                
                await RunAverageLoadTests();
                break;
            case "3":
                // Uncomment the following line if RunSpikeTests method exists
                // await RunSpikeTests();
                break;
            default:
                Console.WriteLine("Invalid input. Please enter 1, 2, or 3.");
                break;
        }

    }
    private static async Task RunSmokeTests()
    {
        var smokeTester = new SmokeTest(apiEndpoint);
        await smokeTester.TestApiGetRequest(apiEndpoint);
        await smokeTester.TestApiGetRequestWithId(apiEndpoint, "0d6d2e64-b077-4dc6-ac9e-8eef432a6ed7");
        await smokeTester.TestApiPutRequest(apiEndpoint, "0d6d2e64-b077-4dc6-ac9e-8eef432a6ed7", new GameData
        {
            GameName = "Sic Bo",
            Category = "Dice Game",
            TotalBets = 6000,
            TotalWins = 3000,
            AverageBetAmount = 5,
            PopularityScore = 5.8,
            LastUpdated = "2023-11-26T17:30:57.502414Z"
        });
        await smokeTester.TestApiDeleteRequest(apiEndpoint, "0d6d2e64-b077-4dc6-ac9e-8eef432a6ed7");
        await smokeTester.TestApiPostRequest(apiEndpoint, new GameData
        {
            GameName = "Sic Bo",
            Category = "Dice Game",
            TotalBets = 6000,
            TotalWins = 3000,
            AverageBetAmount = 5,
            PopularityScore = 5.8,
            LastUpdated = "2023-11-26T17:30:57.502414Z"
        });
    }

    private static async Task RunAverageLoadTests()
    {
        var stages = new List<(TimeSpan duration, int target)>
        {
            (TimeSpan.FromMinutes(5), 100),  // Ramp-up to 100 users over 5 minutes
            (TimeSpan.FromMinutes(30), 100), // Stay at 100 users for 30 minutes
            (TimeSpan.FromMinutes(5), 0)     // Ramp-down to 0 users
        };
        var avgLoadTester = new AvgnSpikeLoadTester(apiEndpoint, stages);
        await avgLoadTester.StartGeneralGetTest();
        await avgLoadTester.StartGetTestWithId("02ee6536-551c-4753-b197-7e866f183030");
        // PUT test with a specific ID and data
        string putId = "0d6d2e64-b077-4dc6-ac9e-8eef432a6ed7";
        var gameData = new GameData
        {
            GameName = "Sic Bo",
            Category = "Dice Game",
            TotalBets = 6000,
            TotalWins = 3000,
            AverageBetAmount = 5,
            PopularityScore = 5.8,
            LastUpdated = "2023-11-26T17:30:57.502414Z"
        };
        await avgLoadTester.StartPutTestWithId(putId, gameData);
        await avgLoadTester.StartDeleteTestWithId("0d6d2e64-b077-4dc6-ac9e-8eef432a6ed7");

        // POST test with data
        var gameDataForPost = new GameData
        {
            GameName = "Sic Bo",
            Category = "Dice Game",
            TotalBets = 6000,
            TotalWins = 3000,
            AverageBetAmount = 5,
            PopularityScore = 5.8,
            LastUpdated = "2023-11-26T17:30:57.502414Z"
        };
        await avgLoadTester.StartPostTest(gameDataForPost);
    }

    private static async Task RunSpikeTests()
    {
        var stages = new List<(TimeSpan duration, int target)>
        {
            (TimeSpan.FromMinutes(1), 100),  // Ramp-up to 100 users over 5 minutes
            (TimeSpan.FromMinutes(2), 1400), // Stay at 100 users for 30 minutes
            (TimeSpan.FromMinutes(1), 0)     // Ramp-down to 0 users
        };
        var spikeLoadTester = new AvgnSpikeLoadTester(apiEndpoint, stages);
        await spikeLoadTester.StartGeneralGetTest();
        await spikeLoadTester.StartGetTestWithId("02ee6536-551c-4753-b197-7e866f183030");
        // PUT test with a specific ID and data
        string putId = "0d6d2e64-b077-4dc6-ac9e-8eef432a6ed7";
        var gameData = new GameData
        {
            GameName = "Sic Bo",
            Category = "Dice Game",
            TotalBets = 6000,
            TotalWins = 3000,
            AverageBetAmount = 5,
            PopularityScore = 5.8,
            LastUpdated = "2023-11-26T17:30:57.502414Z"
        };
        await spikeLoadTester.StartPutTestWithId(putId, gameData);
        await spikeLoadTester.StartDeleteTestWithId("0d6d2e64-b077-4dc6-ac9e-8eef432a6ed7");

        // POST test with data
        var gameDataForPost = new GameData
        {
            GameName = "Sic Bo",
            Category = "Dice Game",
            TotalBets = 6000,
            TotalWins = 3000,
            AverageBetAmount = 5,
            PopularityScore = 5.8,
            LastUpdated = "2023-11-26T17:30:57.502414Z"
        };
        await spikeLoadTester.StartPostTest(gameDataForPost);
    }


}
