using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogProof
{
    interface ILogEntry
    {
        int Code { get; }
        object[] Parameters { get; }
    }

    [ProtoContract]
    class LaunchMissleLogEntry : ILogEntry
    {
        public int Code { get { return 100; } }

        [ProtoMember(1)]
        public int Param1 { get; set; }
        [ProtoMember(2)]
        public int Param2 { get; set; }
        [ProtoMember(3)]
        public DateTime Param3 { get; set; }
        [ProtoMember(4)]
        public decimal Param4 { get; set; }
        [ProtoMember(5)]
        public decimal Param5 { get; set; }
        [ProtoMember(6)]
        public string Param6 { get; set; }

        public object[] Parameters { get { return new object[] { Param1, Param2, Param3, Param4, Param5, Param6 }; } }
    }

    [ProtoContract]
    class MakeSandwitchLogEntry : ILogEntry
    {
        public int Code { get { return 101; } }

        [ProtoMember(1)]
        public int Param1 { get; set; }
        [ProtoMember(2)]
        public int Param2 { get; set; }
        [ProtoMember(3)]
        public DateTime Param3 { get; set; }
        [ProtoMember(4)]
        public decimal Param4 { get; set; }
        [ProtoMember(5)]
        public decimal Param5 { get; set; }
        [ProtoMember(6)]
        public string Param6 { get; set; }

        public object[] Parameters { get { return new object[] { Param1, Param2, Param3, Param4, Param5, Param6 }; } }
    }

    [ProtoContract]
    class SaveWhaleLogEntry : ILogEntry
    {
        public int Code { get { return 102; } }

        [ProtoMember(1)]
        public int Param1 { get; set; }
        [ProtoMember(2)]
        public int Param2 { get; set; }
        [ProtoMember(3)]
        public DateTime Param3 { get; set; }
        [ProtoMember(4)]
        public decimal Param4 { get; set; }
        [ProtoMember(5)]
        public decimal Param5 { get; set; }
        [ProtoMember(6)]
        public string Param6 { get; set; }

        public object[] Parameters { get { return new object[] { Param1, Param2, Param3, Param4, Param5, Param6 }; } }
    }
}
