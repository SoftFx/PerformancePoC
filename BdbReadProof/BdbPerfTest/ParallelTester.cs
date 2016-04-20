using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using BerkeleyDB;

namespace BdbPerformTest
{
    class ParallelTester
    {
        public BdbStorage storageToRead;
        public BdbStorage storageToWrite;
        public IEnumerable<KeyValuePair<DatabaseEntry, DatabaseEntry>> initialData;
        public  int writeTime=0;
        public  int readTime=0;
        public  bool stopFlag = false;
        public  int writeCount = 0;
        public  int readCount = 0;
        public  int syncTime = 0;


        public  void SetAll()
        {
            writeCount = 0;
            writeTime = 0;
            Stopwatch watch =  new Stopwatch();
            watch.Reset();
            watch.Start();

            

            foreach (var kv in initialData)
                if (stopFlag == false)
                {
                    storageToWrite.Set(kv.Key, kv.Value, false);
                    writeCount += 1;
                }
                else break;
            stopFlag = true;
            watch.Stop();
            writeTime += (int)watch.ElapsedMilliseconds;
            watch.Reset();
            watch.Start();

            storageToWrite._btreeDb.Sync();
            watch.Stop();
            syncTime+= (int)watch.ElapsedMilliseconds;
            writeTime += (int)watch.ElapsedMilliseconds;
        }

        public  void GetAll()
        {

            readTime = 0;
            readCount = 0;
            Stopwatch watch = new Stopwatch();
            watch.Reset();
            watch.Start();
            Cursor dbc;

            using (dbc = storageToRead._btreeDb.Cursor())
            {
                dbc.MoveFirst();

                var dbEntry = dbc.Current;
                while (dbEntry.Key != null && dbEntry.Value != null && !stopFlag)
                {
                    var readed = new KeyValuePair<DatabaseEntry, DatabaseEntry>((dbEntry.Key),
                                                              (dbEntry.Value));
                    dbc.MoveNext();
                    dbEntry = dbc.Current;
                    readCount += 1;
                }
                stopFlag = true;
            }

            watch.Stop();
            readTime += (int)watch.ElapsedMilliseconds;
        }
    }
}
