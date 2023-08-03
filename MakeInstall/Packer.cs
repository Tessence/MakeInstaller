using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    public enum PackType : byte
    {
        STRING = 0,
        FILE_NAME = 1,
        FILE_DATA = 2,
        CONFIG = 3,
        ARCHIVE = 4,
    }

    public class Packer
    {
        Stream stream;
        const int PACK_BUFF_SIZE = 2048;
        byte[] buff = new byte[PACK_BUFF_SIZE];

        public Packer(Stream writer)
        {
            this.stream = writer;
        }

        public int Pack(int val)
        {
            int offset = 0;
            PackInt(buff, ref offset, val);
            stream.Write(buff, 0, offset);
            return offset;
        }


        public static void PackInt(byte [] buff, ref int offset, int val)
        {           
            buff[offset] = (byte)(val & 0xff); val = val >> 8; offset++;
            buff[offset] = (byte)(val & 0xff); val = val >> 8; offset++;
            buff[offset] = (byte)(val & 0xff); val = val >> 8; offset++;
            buff[offset] = (byte)(val & 0xff); offset++;             
        }

        public static int UnPackInt(byte[] buff, ref int offset)
        {
            byte v1 = buff[offset++];
            byte v2 = buff[offset++];
            byte v3 = buff[offset++];
            byte v4 = buff[offset++];
            int val = v1 | v2 << 8 | v3 << 16 | v4 << 24;
            return val;
        }

        public int Pack(string val)
        {
            int cnt = 0;
            cnt += Pack(PackType.STRING);
            int len = Encoding.UTF8.GetBytes(val, 0, val.Length, buff, 4);
            cnt += Pack(len);
            cnt += len;
            stream.Write(buff, 4, len);
            return cnt;
        }

        public static byte[] GetPackBytes(string val)
        {
            var bytes = Encoding.UTF8.GetBytes(val);
            return bytes;
        }

        public int Pack(byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
            return bytes.Length;
        }


        public int Pack(PackType ty)
        {
            buff[0] = (byte)ty;
            stream.Write(buff, 0, 1);
            return 1;
        }

        public int Pack(ExeType ty)
        {
            buff[0] = (byte)ty;
            stream.Write(buff, 0, 1);
            return 1;
        }

        public static string UnPackString(byte[] buff, ref int offset, int len)
        {
            var chars = Encoding.UTF8.GetChars(buff, offset, len);
            string val = new string(chars);
            //                string val = new string((sbyte*)buff, offset, len, Encoding.UTF8);
            return val;            
        }

        public byte UnPackByte()
        {
            stream.Read(buff, 0, 1);
            byte val = buff[0];
            return val;
        }

        public int UnPackInt()
        {
            int val;
            stream.Read(buff, 0, 4);
            int offset = 0;
            val = UnPackInt(buff, ref offset);
            return val;
        }

        public string UnPackString()
        {
            PackType str = (PackType)UnPackByte();
            if(str != PackType.STRING)
            {
                return string.Empty;
            }
            int strLen = UnPackInt();
            var buff = this.buff;
            if (strLen > PACK_BUFF_SIZE)
            {
                buff = new byte[strLen];
            }
            stream.Read(buff, 0, strLen);
            int offset = 0;
            string val = UnPackString(buff, ref offset, strLen);
            return val;
        }
    }
}
