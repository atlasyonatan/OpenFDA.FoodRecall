using System.Text.RegularExpressions;

namespace OpenFDA.FoodRecall.ConsoleHost
{
    public static class FindLogic
    {
        public static (DateOnly date, int count) FewestRecalls(IEnumerable<(DateOnly date, int count)> timeseries) => timeseries
            .OrderBy(item => item.count)
            .First();

        private const string WordPattern = @"\b[a-zA-Z]{4,}\b";

        public static IDictionary<string, int> WordOccurence(IEnumerable<string> paragraphs) => paragraphs
            .SelectMany(p => Regex.Matches(p, WordPattern))
            .Select(m => m.Value)
            .GroupBy(word => word.ToLower())
            .ToDictionary(group => group.Key, group => group.Count());

        public static (string[] words, int count) MostCommonWords(IDictionary<string, int> occurances)
        {
            var ordered = occurances.OrderByDescending(word => word.Value);
            var count = ordered.First().Value;
            var words = ordered.TakeWhile(word => word.Value == count)
                .Select(word => word.Key)
                .ToArray();
            return (words, count);
        }
    }
}
