using System.IO;
using System.Reflection;
using Xunit;
namespace AzureRedisSample.Tests
{
    public class SimpleTests
    {
        private string configuration;

        [Fact]
        public void First()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AzureRedisSample.Tests.RedisKey"))
            using (var s = new StreamReader(stream))
            {
                configuration = s.ReadToEnd();
            }
            var connection = StackExchange.Redis.ConnectionMultiplexer.Connect(configuration);
        }
    }
}
