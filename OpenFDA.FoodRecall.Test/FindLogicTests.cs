using OpenFDA.FoodRecall.ConsoleHost;

namespace OpenFDA.FoodRecall.Test
{
    //exercise 2
    public class FindLogicTests
    {
        [Test]
        public void FewestRecallsFoundExists()
        {
            var timeSeries = Enumerable.Range(1, 1000).Select(i => (new DateOnly(i, 1, 1), i)).ToArray();
            var found = FindLogic.FewestRecalls(timeSeries);
            Assert.Contains(found, timeSeries);
        }

        [Test]
        public void FewestRecallsFoundEquals()
        {
            var timeSeries = Enumerable.Range(1, 1000).Select(i => (new DateOnly(i, 1, 1), i)).ToList();
            var expected = (new DateOnly(1, 2, 3), 0);
            timeSeries.Add(expected);
            
            var actual = FindLogic.FewestRecalls(timeSeries);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void FewestRecallsSameCount()
        {
            var expected = 7;
            var timeSeries = Enumerable.Range(1, 1000).Select(i => (new DateOnly(i, 1, 1), expected)).ToArray();
            var found = FindLogic.FewestRecalls(timeSeries);
            var actual = found.count;
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}