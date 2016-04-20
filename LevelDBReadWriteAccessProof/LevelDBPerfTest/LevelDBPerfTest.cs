using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using LevelDB;

namespace LevelDBPerfTest
{
    class LevelDBPerfTest
    {
        

        static public byte[] GetNextDBEntry(byte[] key)
        {

            long FirstPart;
            if (key!=null &&  key.Length != 0)
            {
                using (var ms = new MemoryStream(key))
                {
                    using (var bw = new BinaryReader(ms))
                    {
                        FirstPart = bw.ReadInt64();
                        bw.Close();
                    }
                    ms.Close();
                }
            }
            else
            {
                FirstPart = 0;
            }
            int count = sizeof(long) + sizeof(byte) + sizeof(byte);
            var data = new byte[count];

            using (var ms = new MemoryStream(data))
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write((FirstPart + 99971) % 9999907);
                    bw.Write((byte)0);
                    bw.Write((byte)0);
                    bw.Close();
                }
                ms.Close();
            }
            return data;

        }

        static public void DoParallelTest(int baseNum, int recNum, String path, bool isOnlyReadTest = false)
        {
            Console.WriteLine("Parallel test: "+ baseNum.ToString()+" databases, \t"+ recNum.ToString() +" records in each database\n");

            var listForAppend = new List<KeyValuePair<byte[], byte[]>>();
            var dbList = new List<LevelDBStorage>();

            var key = GetNextDBEntry(null);

            Stopwatch perfWatch = new Stopwatch();
            perfWatch.Start();
            for (int base_ind = 0; base_ind < baseNum; base_ind++)
            {
                dbList.Add(new LevelDBStorage("database_"+base_ind.ToString(),path));
                dbList[base_ind].OpenBase();
            }
            perfWatch.Stop();
            
            if(!isOnlyReadTest)
                Console.WriteLine("\nEmpty databases initialization and opening time: "+ perfWatch.ElapsedMilliseconds.ToString()+" ms\n");
            else Console.WriteLine("\nNot-Empty databases initialization and opening time: " + perfWatch.ElapsedMilliseconds.ToString() + " ms\n");
            perfWatch.Reset();
            
            if (!isOnlyReadTest)
            {
                for (int rec_ind = 0; rec_ind < recNum; rec_ind++)
                {
                    listForAppend.Add(new KeyValuePair<byte[], byte[]>(key, key));
                    key = GetNextDBEntry(key);
                }



                perfWatch.Start();

                Parallel.ForEach(dbList, t => t.SetAll(listForAppend));
                perfWatch.Stop();
                Console.WriteLine("WriteTime: " + (baseNum*recNum).ToString() + " records in " +
                                  (perfWatch.ElapsedMilliseconds).ToString() + " ms");
                Console.WriteLine("WriteSpeed: " + ((1000.0*baseNum*recNum)/perfWatch.ElapsedMilliseconds).ToString() +
                                  " rec/s");
                Console.Write("\n");
                perfWatch.Reset();

                perfWatch.Start();
                for (int base_ind = 0; base_ind < baseNum; base_ind++)
                {
                    dbList[base_ind].CloseBase();
                }
                perfWatch.Stop();
                Console.WriteLine("\nNot-Empty databases closing time: " + perfWatch.ElapsedMilliseconds.ToString() + " ms\n");
                perfWatch.Reset();
                perfWatch.Start();
                for (int base_ind = 0; base_ind < baseNum; base_ind++)
                {
                    dbList[base_ind].OpenBase();
                }
                perfWatch.Stop();
                Console.WriteLine("\nNot-Empty databases opening time: " + perfWatch.ElapsedMilliseconds.ToString() + " ms\n");
                perfWatch.Reset();

                Console.WriteLine("\nMemory: " + Process.GetCurrentProcess().PeakVirtualMemorySize64.ToString() + " bytes\n");

            }

            perfWatch.Start();
            Parallel.ForEach(dbList, t => t.GetAll());
            int gettedCount = 0;
            dbList.ForEach(t=> gettedCount+=t.lastGettedCount);
            perfWatch.Stop();
            Console.WriteLine("ReadTime: " + gettedCount.ToString() + " records in " + (perfWatch.ElapsedMilliseconds).ToString() + " ms");
            Console.WriteLine("ReadSpeed: " + ((1000.0 * gettedCount) / perfWatch.ElapsedMilliseconds).ToString() + " rec/s");

        }

        static public void DoParallelReadAccessTest()
        {
            var listForAppend = new List<KeyValuePair<byte[], byte[]>>();
            var db = new LevelDBStorage("parallelAccessTestDB", @"C:/LevelDbTest");
            var key = GetNextDBEntry(null);
            for (int rec_ind = 0; rec_ind < 10000; rec_ind++)
            {
                listForAppend.Add(new KeyValuePair<byte[], byte[]>(key, key));
                key = GetNextDBEntry(key);
            }
            db.OpenBase();
            db.SetAll(listForAppend);
            var thr_1 = new Thread(db.NRetGetAll);
            thr_1.Start();
            var thr_2 = new Thread(db.NRetGetAll);
            thr_2.Start();
            thr_1.Join();
            thr_2.Join();
        }

        static void Main(string[] args)
        {
            DoParallelReadAccessTest();
            bool readOnly = false;
            if (args.Length > 0 && args[0] == "ro")
                readOnly = true;
            Stopwatch testTimer = new Stopwatch();
            testTimer.Start();
            DoParallelTest(1024,10000,@"C:/LevelDbTest", readOnly);
            testTimer.Stop();
            Console.WriteLine("\nTest time: "+testTimer.ElapsedMilliseconds.ToString()+" ms");
            Console.ReadKey();
        }
    }
}
