using System;
using System.IO;
using System.Linq;
using System.Reflection;
using StackExchange.Redis;
using Xunit;
namespace AzureRedisSample.Tests
{
    public class SimpleTests
    {
        Lazy<ConnectionMultiplexer> connection = new Lazy<ConnectionMultiplexer>(Connect);
        [Fact]
        public void First()
        {
            Assert.NotNull(connection.Value);
        }

        [Fact]
        public void SortedList()
        {
            RedisKey key = "SortedList.Key";
            var db = connection.Value.GetDatabase();
            db.KeyDelete(key);
            db.SortedSetAdd(key, "First", 1D);
            db.SortedSetAdd(key, "Fourth", 2D);
            db.SortedSetAdd(key, "Third", 1.2D);
            db.SortedSetAdd(key, "Second", 1.1D);
            var result = db.SortedSetScan(key);
            Assert.NotNull(result);
            var resultArray = result.ToArray();
            Assert.Equal(4, resultArray.Length);
            Assert.Equal((RedisValue) "First", resultArray[0].Element);
            Assert.Equal((RedisValue)"Second", resultArray[1].Element);
            Assert.Equal((RedisValue)"Third", resultArray[2].Element);
            Assert.Equal((RedisValue)"Fourth", resultArray[3].Element);
        }


        private static ConnectionMultiplexer Connect()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AzureRedisSample.Tests.RedisKey"))
            using (var s = new StreamReader(stream))
            {
                return ConnectionMultiplexer.Connect(s.ReadToEnd());
            }
        }
    }
}
;