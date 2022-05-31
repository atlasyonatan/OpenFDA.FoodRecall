// See https://aka.ms/new-console-template for more information
using OpenFDA.FoodRecall.ConsoleHost;
using RestSharp;
using RestSharp.Authenticators;

var apiKey = "7GvCRKO92fHHgjrcBXP2owjKCNXHWxfqZyfdDvdy";
var authenticator = new HttpBasicAuthenticator(apiKey, "");
using (var client = new RestClient().UseAuthenticator(authenticator))
{
    var year = new DateOnly(2012, 1, 1);
    var timeseries = await Requests.ReportDateTimeSeries(client, year, year.AddYears(1).AddDays(-1));
    var fewestDay = FindLogic.FewestRecalls(timeseries);
    Console.WriteLine($"A day with fewest recalls in {year.Year}: {fewestDay.date} ({fewestDay.count} recalls)");

    var recalls = await Requests.GetRecallsInDay(client, fewestDay.date);
    Console.WriteLine($"Recalls from {fewestDay.date} ({recalls.Count} recalls) ordered by 'recall_initiation_date' ascending:");
    Console.WriteLine(recalls);

    var fieldName = "reason_for_recall";
    var paragraphs = recalls.Select(recall => recall.Value<string>(fieldName)).ToArray();
    var occurances = FindLogic.WordOccurence(paragraphs!);
    var (mostCommonWords, occuranceCount) = FindLogic.MostCommonWords(occurances);
    var occuranceText = occuranceCount + " occurance" + (occuranceCount == 1 ? "" : "s");
    Console.WriteLine($"Most common words (with {occuranceText}) under '{fieldName}' in the above recalls are:");
    Console.WriteLine(string.Join(", ", mostCommonWords.Select(word => $"'{word}'")));
}

Console.ReadLine();
