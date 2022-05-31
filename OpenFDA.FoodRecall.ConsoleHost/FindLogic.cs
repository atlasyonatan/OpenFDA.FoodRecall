using System.Text.RegularExpressions;

namespace OpenFDA.FoodRecall.ConsoleHost
{
    public static class FindLogic
    {
        public const string WordPattern = @"\b[a-zA-Z]{4,}\b";

        /// <summary>
        /// Finds a day with the fewest recalls in a given timeSeries
        /// </summary>
        public static (DateOnly date, int count) FewestRecalls(IEnumerable<(DateOnly date, int count)> timeseries) => timeseries
            .OrderBy(item => item.count)
            .First();

        /// <summary>
        /// Prepare a dictionary whose keys are words (defined by <see cref="WordPattern"/> and case insensitive) contained in the given paragraphs, 
        /// and whose values are the amount of times each word occured in all the paragraphs.
        /// </summary>
        public static IDictionary<string, int> WordOccurence(IEnumerable<string> paragraphs)
        {
            var merge = new Dictionary<string, int>();
            foreach (var occurance in paragraphs.Select(WordOccurance))
                foreach (var (key, value) in occurance)
                {
                    if (!merge.ContainsKey(key))
                        merge[key] = 0;
                    merge[key] += value;
                }
            return merge;
        }

        /// <summary>
        /// Prepare a dictionary whose keys are words (defined by <see cref="WordPattern"/> and case insensitive) contained in the given paragraph, 
        /// and whose values are the amount of times each word occured in the paragraph.
        /// </summary>
        public static IDictionary<string, int> WordOccurance(string paragraph) => Regex.Matches(paragraph, WordPattern)
            .Select(m => m.Value)
            .GroupBy(word => word.ToLower())
            .ToDictionary(group => group.Key, group => group.Count());

        /// <summary>
        /// Finds the most common words in a given dictionary of words and their occurances.
        /// </summary>
        /// <returns>A <see cref="Tuple"/> containing a <see cref="string"/>[] of the most common words, and an <see cref="int"/> representing the occurance amount of those words</returns>
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
