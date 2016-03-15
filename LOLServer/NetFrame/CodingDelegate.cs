using System;
using System.Collections.Generic;
using System.Text;

namespace NetFrame {
    public delegate byte[] LengthEncode(byte[] value);
    public delegate byte[] LengthDecode(ref List<byte> value);
    public delegate byte[] MsgEncode(object value);
    public delegate object MsgDecode(byte[] value);
}
