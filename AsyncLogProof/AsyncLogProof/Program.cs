using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogProof
{
    class Program
    {
        private const int TestSize = 1000000;
        private const int BufferLimit = 1000000;

        static void Main(string[] args)
        {
            RunTest("string log", new StringLoggerTest());
            RunTest("string log (backgorund writer, TPL)", new AsyncWriteStringLoggerTest());
            RunTest("string log (backgorund writer, TPL, additional buffer)", new AsyncWriteStringLoggerTest_AddBuffer());
            RunTest("string log (backgorund writer, lockfree queue)", new AsyncWriteStringLoggerTest_ConcurrentQueue());
            RunTest("string log (backgorund serializer, backgorund writer, TPL)", new AsyncSerializationStringLoggerTest());
            RunTest("string log (backgorund serializer, backgorund writer, TPL, additional buffer)", new AsyncSerializationStringLoggerTest_AddBuffer());
            RunTest("string log (backgorund serializer, backgorund writer, lockfree queue)", new AsyncSerializationStringLoggerTest_ConcurrentQueue());

            RunTest("binary log (Bson)", new BasicBinaryLoggerTest_Bson());
            RunTest("binary log (Json)", new BasicBinaryLoggerTest_Json());
            RunTest("binary log (ProtoBuf)", new BasicBinaryLoggerTest_ProtoBuf());
            RunTest("binary log (ProtoBuf, backgorund writer)", new AsyncWriteBinaryLoggerTest_ProtoBuf());

            Console.Read();
        }

        static void RunTest(string name, ILogTest testDef)
        {
            Console.WriteLine(name + ":");
            Stopwatch watch = Stopwatch.StartNew();
            testDef.ProduceLogs(TestSize, BufferLimit);
            watch.Stop();
            Console.WriteLine("\t produce - " + watch.ElapsedMilliseconds + "ms");
            watch.Start();
            testDef.Completed.Wait();
            watch.Stop();
            Console.WriteLine("\t total - " + watch.ElapsedMilliseconds + "ms");
            Console.WriteLine();

            GC.Collect();
        }
    }

    interface ILogTest
    {
        void ProduceLogs(int count, int buffSize);
        Task Completed { get; }
    }
}
