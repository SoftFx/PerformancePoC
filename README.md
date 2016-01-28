# PerformancePoC
AsyncLogProof
string log:
         produce - 2417ms
         total - 2418ms

string log (backgorund writer, TPL):
         produce - 3500ms
         total - 3501ms

string log (backgorund writer, TPL, additional buffer):
         produce - 3634ms
         total - 3637ms

string log (backgorund writer, lockfree queue):
         produce - 2609ms
         total - 2609ms

string log (backgorund serializer, backgorund writer, TPL):
         produce - 1869ms
         total - 3507ms

string log (backgorund serializer, backgorund writer, TPL, additional buffer):
         produce - 934ms
         total - 3770ms

string log (backgorund serializer, backgorund writer, lockfree queue):
         produce - 829ms
         total - 2902ms

binary log (Bson):
         produce - 5853ms
         total - 5854ms

binary log (Json):
         produce - 3230ms
         total - 3230ms

binary log (ProtoBuf):
         produce - 997ms
         total - 997ms

binary log (ProtoBuf, backgorund writer):
         produce - 609ms
         total - 1321ms
