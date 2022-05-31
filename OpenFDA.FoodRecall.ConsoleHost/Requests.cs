using Newtonsoft.Json.Linq;
using RestSharp;

namespace OpenFDA.FoodRecall.ConsoleHost
{
    public static class FoodEnforcementRequests
    {
        public const string Resource = "food/enforcement.json";
        public const string DateFormat = "yyyyMMdd";

        /// <summary>
        /// Retrieves a timeseries of days and the recall count of that day, between <paramref name="start"/> and <paramref name="end"/>.
        /// <para/>
        /// </summary>
        public static async Task<(DateOnly date, int count)[]> ReportDateTimeSeries(RestClient client, DateOnly start, DateOnly end)
        {
            //prepare API request
            var request = new RestRequest(Resource, Method.Get)
                .AddParameter("search", $"report_date:[{start.ToString(DateFormat)}+TO+{end.ToString(DateFormat)}]", false)
                .AddParameter("count", "report_date");

            //send request and wait for response
            var response = await client.GetAsync(request);

            //parse response
            var json = JObject.Parse(response.Content!);
            var results = (JArray)json["results"]!;

            //convert to timeSeries
            var timeSeries = results.Select(t =>
                (date: DateOnly.ParseExact(t.Value<string>("time")!, DateFormat, null),
                count: t.Value<int>("count")))
                .ToArray();

            return timeSeries;
        }
        /// <summary>
        /// Retrieves the top 1000 recalls in a given <paramref name="day"/>
        /// </summary>
        public static async Task<JArray> GetRecallsInDay(RestClient client, DateOnly day)
        {
            //prepare API request
            var request = new RestRequest(Resource, Method.Get)
               .AddParameter("search", $"report_date:{day.ToString(DateFormat)}", false)
               .AddParameter("limit", 1000) //api defined max value for 'limit'
               .AddParameter("sort", "recall_initiation_date:asc");

            //send request and wait for response
            var response = await client.GetAsync(request);

            //parse resonse
            var json = JObject.Parse(response.Content!);
            var results = (JArray)json["results"]!;

            return results;
        }
    }
}
