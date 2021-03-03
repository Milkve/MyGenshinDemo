using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Utils
{
    public static class PointerUtil
    {
        public unsafe static int ReadInt(byte[] data, int n)
        {
            fixed (byte* p = data)
            {
                int* pvalue = (int*)(p + n * sizeof(int));
                return *pvalue;
            }
        }
        public  unsafe static void WriteInt(byte[] data, int n, int value)
        {
            fixed (byte* p = data)
            {
                int* pvalue = (int*)(p + n * sizeof(int));
                *pvalue = value;
            }
        }

        public unsafe static KeyValuePair<short, int> ReadKShortVInt(byte[] data, int n)
        {

            fixed (byte* p = data)
            {
                short* ptype = (short*)(p + n * (sizeof(short) + sizeof(int)));
                //int* pvalue = (int*)(ptype + sizeof(short));
                int* pvalue = (int*)(p + n * (sizeof(short) + sizeof(int)) + sizeof(short));
                return new KeyValuePair<short, int>(*ptype, *pvalue);
            }
        }

        public unsafe static void WriteKShortVInt(byte[] data, int n, short type, int value)
        {
            fixed (byte* p = data)
            {
                short* ptype = (short*)(p + n * (sizeof(short) + sizeof(int)));
                *ptype = type;
                //int* pvalue = (int*)(ptype + sizeof(short));
                int* pvalue = (int*)(p + n * (sizeof(short) + sizeof(int)) + sizeof(short));
                *pvalue = value;
            }

        }
    }
}
