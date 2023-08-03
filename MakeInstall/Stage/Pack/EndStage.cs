using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    class EndStage : Stage
    {
        public EndStage(Maker maker, int progress) : base(maker, progress)
        {
        }
        public override string Title { get { return "写入签名"; } }
        public override bool Run()
        {
            base.Run();
            int byteCount = 0;
            //写入4k 空白到压缩流，防止解压时读取到标识符号。
            var buff = maker.buff;
            for (int i = 0; i < Maker.STREAM_BUFF_SIZE; ++i)
            {
                buff[i] = 0;
            }
            maker.zipStream.Write(buff, 0, Maker.STREAM_BUFF_SIZE);
            maker.zipStream.Flush();
            maker.zipStream.Close();

            FileStream appendStream = new FileStream(maker.outputFile, FileMode.Append, FileAccess.Write);
            appendStream.Seek(0, SeekOrigin.End);
            Packer packer = maker.packer;

            Packer.PackInt(buff, ref byteCount, maker.packerSize);
            appendStream.Write(buff, 0, byteCount);
            appendStream.Write(Maker.BAKED_FLAG, 0, Maker.BAKED_FLAG.Length);
            //打包类型
            buff[0] = (byte)ExeType.Packed;
            appendStream.Write(buff, 0, 1);

            appendStream.Flush();
            appendStream.Close();
            return true;
        }
    }
}
