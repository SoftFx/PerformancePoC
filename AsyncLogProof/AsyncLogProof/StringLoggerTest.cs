using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace AsyncLogProof
{
    class StringLoggerTest : ILogTest
    {
        public Task Completed { get { return Task.FromResult(0); } }

        public void ProduceLogs(int count, int buffSize)
        {
            LogGenerator g = new LogGenerator();

            using (var file = new StreamWriter("basic.log", false))
            {
                for (int i = 0; i < count; i++)
                {
                    g.Next();

                    var line = string.Format(g.FormatStr, g.Param1, g.Param2, g.Param3, g.Param4, g.Param5, g.Param6);
                    file.WriteLine(line);
                }
            }
        }
    }

    class AsyncWriteStringLoggerTest : ILogTest
    {
        public Task Completed { get; private set; }

        public void ProduceLogs(int count, int buffSize)
        {
            var options = new ExecutionDataflowBlockOptions() { BoundedCapacity = buffSize, MaxDegreeOfParallelism = 1, SingleProducerConstrained = true };

            LogGenerator g = new LogGenerator();

            var file = new StreamWriter("basic.async.log", false);

            ActionBlock<string> writer = new ActionBlock<string>(s => file.WriteLine(s), options);

            for (int i = 0; i < count; i++)
            {
                g.Next();

                var line = string.Format(g.FormatStr, g.Param1, g.Param2, g.Param3, g.Param4, g.Param5, g.Param6);
                writer.SendAsync(line).Wait();
            }

            writer.Complete();

            Completed = writer.Completion.ContinueWith(t => file.Close());
        }
    }

    class AsyncWriteStringLoggerTest_AddBuffer : ILogTest
    {
        public Task Completed { get; private set; }

        public void ProduceLogs(int count, int buffSize)
        {
            var bufferOptions = new DataflowBlockOptions() { BoundedCapacity = buffSize };
            var writerOptions = new ExecutionDataflowBlockOptions() { BoundedCapacity = 10, MaxDegreeOfParallelism = 1, MaxMessagesPerTask = 10, SingleProducerConstrained = true };

            LogGenerator g = new LogGenerator();

            var file = new StreamWriter("basic.async.buff.log", false);

            BufferBlock<string> buffer = new BufferBlock<string>(bufferOptions);
            ActionBlock<string> writer = new ActionBlock<string>(s => file.WriteLine(s), writerOptions);

            buffer.LinkTo(writer, new DataflowLinkOptions() { PropagateCompletion = true });

            for (int i = 0; i < count; i++)
            {
                g.Next();

                var line = string.Format(g.FormatStr, g.Param1, g.Param2, g.Param3, g.Param4, g.Param5, g.Param6);
                writer.SendAsync(line).Wait();
            }

            buffer.Complete();

            Completed = writer.Completion.ContinueWith(t => file.Close());
        }
    }

    class AsyncWriteStringLoggerTest_ConcurrentQueue : ILogTest
    {
        public Task Completed { get; private set; }

        public void ProduceLogs(int count, int buffSize)
        {
            LogGenerator g = new LogGenerator();
            BlockingCollection<string> queue = new BlockingCollection<string>(buffSize);
            Completed = Task.Factory.StartNew(() => Write(queue, count));

            for (int i = 0; i < count; i++)
            {
                g.Next();

                var line = string.Format(g.FormatStr, g.Param1, g.Param2, g.Param3, g.Param4, g.Param5, g.Param6);
                queue.Add(line);
            }
        }

        private void Write(BlockingCollection<string> queue, int count)
        {
            using (var file = new StreamWriter("basic.async.q.log", false))
            {
                for (int i = 0; i < count; i++)
                    file.WriteLine(queue.Take());
            }
        }
    }

    class AsyncSerializationStringLoggerTest : ILogTest
    {
        public Task Completed { get; private set; }

        public void ProduceLogs(int count, int buffSize)
        {
            var writerOptions = new ExecutionDataflowBlockOptions() { BoundedCapacity = 10, MaxDegreeOfParallelism = 1, MaxMessagesPerTask = 10 };
            var serializerOptions = new ExecutionDataflowBlockOptions() { BoundedCapacity = buffSize, MaxDegreeOfParallelism = 8, SingleProducerConstrained = true };

            LogGenerator g = new LogGenerator();

            var file = new StreamWriter("basic.async.srlz.log", false);

            TransformBlock<LogEntry, string> serializer = new TransformBlock<LogEntry, string>(
                e => string.Format(e.format, e.parameters),
                serializerOptions);

            ActionBlock<string> writer = new ActionBlock<string>(s => file.WriteLine(s), writerOptions);

            serializer.LinkTo(writer, new DataflowLinkOptions() { PropagateCompletion = true });

            for (int i = 0; i < count; i++)
            {
                g.Next();

                var entry = new LogEntry() { format = g.FormatStr, parameters = new object[] { g.Param1, g.Param2, g.Param3, g.Param4, g.Param5, g.Param6 } };
                serializer.SendAsync(entry).Wait();
            }

            serializer.Complete();

            Completed = writer.Completion.ContinueWith(t => file.Close());
        }
    }

    class AsyncSerializationStringLoggerTest_AddBuffer : ILogTest
    {
        public Task Completed { get; private set; }

        public void ProduceLogs(int count, int buffSize)
        {
            var bufferOptions = new DataflowBlockOptions() { BoundedCapacity = buffSize, MaxMessagesPerTask = 10 };
            var writerOptions = new ExecutionDataflowBlockOptions() { BoundedCapacity = 10, MaxDegreeOfParallelism = 1, MaxMessagesPerTask = 10 };
            var serializerOptions = new ExecutionDataflowBlockOptions() { BoundedCapacity = 80, MaxDegreeOfParallelism = 8, SingleProducerConstrained = true, MaxMessagesPerTask = 10 };

            LogGenerator g = new LogGenerator();

            var file = new StreamWriter("basic.async.srlz.buff.log", false);

            BufferBlock<LogEntry> buffer = new BufferBlock<LogEntry>(bufferOptions);

            TransformBlock<LogEntry, string> serializer = new TransformBlock<LogEntry, string>(
                e => string.Format(e.format, e.parameters),
                serializerOptions);

            ActionBlock<string> writer = new ActionBlock<string>(s => file.WriteLine(s), writerOptions);

            buffer.LinkTo(serializer, new DataflowLinkOptions() { PropagateCompletion = true });
            serializer.LinkTo(writer, new DataflowLinkOptions() { PropagateCompletion = true });

            for (int i = 0; i < count; i++)
            {
                g.Next();

                var entry = new LogEntry() { format = g.FormatStr, parameters = new object[] { g.Param1, g.Param2, g.Param3, g.Param4, g.Param5, g.Param6 } };
                buffer.SendAsync(entry).Wait();
            }

            buffer.Complete();

            Completed = writer.Completion.ContinueWith(t => file.Close());
        }
    }

    internal struct LogEntry
    {
        public string format;
        public object[] parameters;
    }


    class AsyncSerializationStringLoggerTest_ConcurrentQueue : ILogTest
    {
        public Task Completed { get; private set; }

        public void ProduceLogs(int count, int buffSize)
        {
            LogGenerator g = new LogGenerator();
            BlockingCollection<LogEntry> queue = new BlockingCollection<LogEntry>(buffSize);
            Completed = Task.Factory.StartNew(() => Write(queue, count));

            for (int i = 0; i < count; i++)
            {
                g.Next();

                var entry = new LogEntry() { format = g.FormatStr, parameters = new object[] { g.Param1, g.Param2, g.Param3, g.Param4, g.Param5, g.Param6 } };
                queue.Add(entry);
            }
        }

        private void Write(BlockingCollection<LogEntry> queue, int count)
        {
            using (var file = new StreamWriter("basic.async.srlz.q.log", false))
            {
                for (int i = 0; i < count; i++)
                {
                    var e = queue.Take();
                    file.WriteLine(string.Format(e.format, e.parameters));
                }
            }
        }
    }
}
