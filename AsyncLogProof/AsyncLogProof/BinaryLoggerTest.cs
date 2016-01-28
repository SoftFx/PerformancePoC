using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogProof
{
    class BasicBinaryLoggerTest_Bson : ILogTest
    {
        public Task Completed { get { return Task.FromResult(0); } }

        public void ProduceLogs(int count, int buffSize)
        {
            LogGenerator g = new LogGenerator();

            using (var file = new FileStream("binary.bson.log", FileMode.Create))
            {
                using (BsonWriter writer = new BsonWriter(file))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    for (int i = 0; i < count; i++)
                    {
                        g.Next();

                        var entry = new BinaryLogEntry();
                        entry.FormatStringCode = g.FormatStrCode;
                        entry.Parameters = new object[] { g.Param1, g.Param2, g.Param3, g.Param4, g.Param5, g.Param6 };

                        serializer.Serialize(writer, entry);
                    }
                }
            }
        }
    }

    class BasicBinaryLoggerTest_Json : ILogTest
    {
        public Task Completed { get { return Task.FromResult(0); } }

        public void ProduceLogs(int count, int buffSize)
        {
            LogGenerator g = new LogGenerator();

            using (var file = new StreamWriter("binary.json.log", false))
            {
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    for (int i = 0; i < count; i++)
                    {
                        g.Next();

                        var entry = new BinaryLogEntry();
                        entry.FormatStringCode = g.FormatStrCode;
                        entry.Parameters = new object[] { g.Param1, g.Param2, g.Param3, g.Param4, g.Param5, g.Param6 };

                        serializer.Serialize(writer, entry);
                    }
                }
            }
        }
    }

    class BasicBinaryLoggerTest_ProtoBuf : ILogTest
    {
        public Task Completed { get { return Task.FromResult(0); } }

        public void ProduceLogs(int count, int buffSize)
        {
            LogGenerator g = new LogGenerator();

            using (var file = new FileStream("binary.proto.log", FileMode.Create))
            {

                for (int i = 0; i < count; i++)
                {
                    var entry = g.NextObject();
                    ProtoBuf.Serializer.Serialize(file, entry);
                }
            }
        }
    }

    class AsyncWriteBinaryLoggerTest_ProtoBuf : ILogTest
    {
        public Task Completed { get; private set; }

        public void ProduceLogs(int count, int buffSize)
        {
            LogGenerator g = new LogGenerator();
            var queue = new BlockingCollection<ILogEntry>(buffSize);
            Completed = Task.Factory.StartNew(() => Write(queue, count));

            for (int i = 0; i < count; i++)
                queue.Add(g.NextObject());
        }

        private void Write(BlockingCollection<ILogEntry> queue, int count)
        {
            using (var file = new FileStream("binary.proto.async.log", FileMode.Create))
            {
                for (int i = 0; i < count; i++)
                    ProtoBuf.Serializer.Serialize(file, queue.Take());
            }
        }
    }

    [Serializable]
    public struct BinaryLogEntry
    {
        public int FormatStringCode;
        public object[] Parameters;
    }
}
