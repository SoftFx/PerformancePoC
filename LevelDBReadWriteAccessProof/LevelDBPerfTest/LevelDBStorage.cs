using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.Threading.Tasks;
using LevelDB;

namespace LevelDBPerfTest
{
    class LevelDBStorage
    {
        private String name;
        private String path;
        private DB dbase;
        public int lastGettedCount;
        public LevelDBStorage(String name, String path)
        {
            this.name = name;
            this.path = path;
        }

        public bool OpenBase()
        {
            dbase = new DB(path+"/"+name,
                new Options() { CreateIfMissing = true, BloomFilter = new BloomFilterPolicy(10) });
            return true;
        }

        public bool CloseBase()
        {
            dbase.Dispose();
            return true;
        }

        public void SetAll(IEnumerable<KeyValuePair<byte[], byte[]>> dataForAppend)
        {
            foreach (var entry in dataForAppend)
            {
                dbase.Put(entry.Key, entry.Value);
            }
        }

        public List<KeyValuePair<byte[], byte[]>> GetAll()
        {

            var iterator = dbase.CreateIterator();
            iterator.SeekToFirst();
            var returnList = new List<KeyValuePair<byte[], byte[]>>();
            while (iterator.IsValid())
            {
                returnList.Add(new KeyValuePair<byte[], byte[]>(iterator.GetKey(), iterator.GetValue()));
                iterator.Next();
            }
            lastGettedCount = returnList.Count;
            return returnList;
        }

        public void NRetGetAll()
        {
            var wr = new StreamWriter(@"C:/LevelDbTest/" + Thread.CurrentThread.ManagedThreadId.ToString() + ".txt");
            var iterator = dbase.CreateIterator();
            iterator.SeekToFirst();
            var returnList = new List<KeyValuePair<byte[], byte[]>>();
            while (iterator.IsValid())
            {
                returnList.Add(new KeyValuePair<byte[], byte[]>(iterator.GetKey(), iterator.GetValue()));
                BitArray bits = new BitArray(iterator.GetKey());
                foreach (var bit in bits)
                {
                    wr.Write((bool)bit?"1":"0");
                }
                wr.WriteLine("");
                iterator.Next();
            }
            wr.Close();
            lastGettedCount = returnList.Count;

        }
    }
}
