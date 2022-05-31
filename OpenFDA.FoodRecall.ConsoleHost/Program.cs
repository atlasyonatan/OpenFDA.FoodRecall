// See https://aka.ms/new-console-template for more information
using OpenFDA.FoodRecall.ConsoleHost;
using RestSharp;
using RestSharp.Authenticators;

//should be stored in secrets for security, but since this is a time limited exercise I won't bother with it.
var apiKey = "7GvCRKO92fHHgjrcBXP2owjKCNXHWxfqZyfdDvdy";

try
{
    //set up the http client which will be used to connect to OpenFDA api
    var authenticator = new HttpBasicAuthenticator(apiKey, "");
    var baseUrl = new Uri("https://api.fda.gov/");
    using (var client = new RestClient(baseUrl).UseAuthenticator(authenticator))
    {
        //exercise 1
        var year = new DateOnly(2012, 1, 1);
        var timeseries = await FoodEnforcementRequests.ReportDateTimeSeries(client, year, year.AddYears(1).AddDays(-1));
        var fewestDay = FindLogic.FewestRecalls(timeseries);
        Console.WriteLine($"A day with fewest recalls in {year.Year}: {fewestDay.date} ({fewestDay.count} recalls)");
        Console.WriteLine();

        //exercise 3
        var recalls = await FoodEnforcementRequests.GetRecallsInDay(client, fewestDay.date);
        Console.WriteLine($"Recalls from {fewestDay.date} ({recalls.Count} recalls) ordered by 'recall_initiation_date' ascending:");
        Console.WriteLine(recalls);
        Console.WriteLine();

        //exercise 4
        var fieldName = "reason_for_recall";
        var paragraphs = recalls.Select(recall => recall.Value<string>(fieldName)).ToArray();
        var occurances = FindLogic.WordOccurence(paragraphs!);
        var (mostCommonWords, occuranceCount) = FindLogic.MostCommonWords(occurances);
        var occuranceText = occuranceCount + " occurance" + (occuranceCount == 1 ? "" : "s");
        Console.WriteLine($"Most common words (with {occuranceText}) under '{fieldName}' in the above recalls are:");
        Console.WriteLine(string.Join(", ", mostCommonWords.Select(word => $"'{word}'")));
    }
}
catch(Exception ex)
{
    Console.WriteLine("An error occured during program runtime:");
    Console.WriteLine(ex);
    throw;
}

Console.ReadLine();
