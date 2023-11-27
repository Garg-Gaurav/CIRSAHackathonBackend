using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameData
{
    public string GameName { get; set; } = String.Empty;
    public string Category { get; set; } = String.Empty;
    public int TotalBets { get; set; }
    public int TotalWins { get; set; }
    public int AverageBetAmount { get; set; }
    public double PopularityScore { get; set; }
    public string LastUpdated { get; set; } = String.Empty;
}