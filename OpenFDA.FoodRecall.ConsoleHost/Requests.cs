using Newtonsoft.Json.Linq;
using RestSharp;

namespace OpenFDA.FoodRecall.ConsoleHost
{
    public static class Requests
    {
        public const string DateFormat = "yyyyMMdd";
        public static async Task<(DateOnly date, int count)[]> ReportDateTimeSeries(RestClient client, DateOnly start, DateOnly end)
        {
            var request = new RestRequest("https://api.fda.gov/food/enforcement.json", Method.Get)
                .AddParameter("search", $"report_date:[{start.ToString(DateFormat)}+TO+{end.ToString(DateFormat)}]", false)
                .AddParameter("count", "report_date");
            var response = await client.GetAsync(request);
            var json = JObject.Parse(response.Content!);
            var results = (JArray)json["results"]!;
            return results.Select(t =>
                (
                    DateOnly.ParseExact(t.Value<string>("time")!,DateFormat, null),
                    t.Value<int>("count"))
                )
                .ToArray();
        }

        public static async Task<JArray> GetRecallsInDay(RestClient client, DateOnly day)
        {
            var request = new RestRequest("https://api.fda.gov/food/enforcement.json", Method.Get)
               .AddParameter("search", $"report_date:{day.ToString(DateFormat)}", false)
               .AddParameter("limit", 1000)
               .AddParameter("sort", "recall_initiation_date:asc");
            var response = await client.GetAsync(request);
            var json = JObject.Parse(response.Content!);
            var results = (JArray)json["results"]!;
            return results;
        }
    }
}
