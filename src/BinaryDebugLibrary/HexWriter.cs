using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryDebugLibrary
{
    public static class HexWriter
    {
        public static string ConvertBytesToHexString(byte[] bytes)
        {
            return String.Join(' ', bytes.Select(b => $"0x{b:X2}"));
        }
    }
}
