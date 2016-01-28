using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogProof
{
    class LogGenerator
    {
        private static string[] formatSamples = new string[]
                                {
                                    "Launch a missile: number:{0}, series:{1}, date:{2}, params: {3} {4} {5}",
                                    "Make a sandwich : number:{0}, series:{1}, date:{2}, params: {3} {4} {5}",
                                    "Save a whale: number:{0}, series:{1}, date:{2}, params: {3} {4} {5}"
                                };

        private static int[] formatSampleCodes = new int[] { 100, 101, 102 };

        private static string[] StrParamSamples = new string[] { "one", "two", "three", "four" };

        private int i = 1;

        public void Next()
        {
            int formatIndex = i % 3;
            FormatStr = formatSamples[formatIndex];
            FormatStrCode = formatSampleCodes[formatIndex];

            Param1 = i;
            Param2 = i / 2;
            Param3 = new DateTime(i);
            Param4 = i / 5;
            Param5 = i / 20;
            Param6 = StrParamSamples[i % 4];

            i++;
        }

        public ILogEntry NextObject()
        {
            Next();

            switch (FormatStrCode)
            {
                case 100: return new LaunchMissleLogEntry() { Param1 = this.Param1, Param2 = this.Param2, Param3 = this.Param3, Param4 = this.Param4, Param5 = this.Param5, Param6 = this.Param6 };
                case 101: return new MakeSandwitchLogEntry() { Param1 = this.Param1, Param2 = this.Param2, Param3 = this.Param3, Param4 = this.Param4, Param5 = this.Param5, Param6 = this.Param6 };
                case 102: return new SaveWhaleLogEntry() { Param1 = this.Param1, Param2 = this.Param2, Param3 = this.Param3, Param4 = this.Param4, Param5 = this.Param5, Param6 = this.Param6 };
            }

            throw new Exception();
        }

        public string FormatStr { get; private set; }
        public int FormatStrCode { get; private set; }

        public int Param1 { get; private set; }
        public int Param2 { get; private set; }
        public DateTime Param3 { get; private set; }
        public decimal Param4 { get; private set; }
        public decimal Param5 { get; private set; }
        public string Param6 { get; private set; }
    }
}
