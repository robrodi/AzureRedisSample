using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NLog;
using StackExchange.Redis;
using Xunit;
namespace AzureRedisSample.Tests
{
    public class SimpleTests
    {
        Logger log = NLog.LogManager.GetCurrentClassLogger();

        readonly Lazy<IDatabase> connection = new Lazy<IDatabase>(Connect);

        private IDatabase Connection
        {
            get
            {
                return connection.Value;
            }
        }

        [Fact]
        public void First()
        {
            log.Info("==============Starting Test First==============");
            Assert.NotNull(connection.Value);
        }

        [Fact]
        public void SmallSortedList()
        {
            RedisKey key = "SortedList.Key";

            log.Info("==============Starting Test SmallSortedList==============");
            var s = Stopwatch.StartNew();
            log.Trace("Purging DB {0}", s.ElapsedMilliseconds);
            Connection.KeyDelete(key);
            log.Trace("Purging DB -> Done {0}", s.ElapsedMilliseconds);

            s.Restart();

            Tuple<string, double>[] values =
            {
                new Tuple<string, double>("First", 1D),
                new Tuple<string, double>("Fourth", 2D),
                new Tuple<string, double>("Third", 1.2D),
                new Tuple<string, double>("Second", 1.1D)
            };

            log.Trace("Inserting Keys {0}", s.ElapsedMilliseconds);
            var promises = values.Select(value => Connection.SortedSetAddAsync(key, value.Item1, value.Item2)).Select(t => (Task) t);

            Task.WaitAll(promises.ToArray());
            log.Trace("Inserting Keys -> Done {0}", s.ElapsedMilliseconds);

            log.Trace("Getting Keys {0}", s.ElapsedMilliseconds);
            var result = Connection.SortedSetScan(key);
            Assert.NotNull(result);
            log.Trace("Getting Keys -> Have Enumerator {0}", s.ElapsedMilliseconds);

            var resultArray = result.ToArray();
            log.Trace("Getting Keys -> Done {0}", s.ElapsedMilliseconds);

            Assert.Equal(4, resultArray.Length);
            Assert.Equal((RedisValue) "First", resultArray[0].Element);
            Assert.Equal((RedisValue)"Second", resultArray[1].Element);
            Assert.Equal((RedisValue)"Third", resultArray[2].Element);
            Assert.Equal((RedisValue)"Fourth", resultArray[3].Element);
        }

        private static IDatabase Connect()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AzureRedisSample.Tests.RedisKey"))
            using (var s = new StreamReader(stream))
            {
                return ConnectionMultiplexer.Connect(s.ReadToEnd()).GetDatabase();
            }
        }
    }
}
;