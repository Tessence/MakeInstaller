using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall 
{
    class SeekToPackStartStage : Stage
    {
        public SeekToPackStartStage(Maker maker, int progress = 0) : base(maker, progress)
        {
        }

        public override bool Run()
        {
            int packStart = maker.packerSize + Maker.PACK_START_SHIFT;
            maker.outputStream.Seek(packStart, SeekOrigin.Begin);            
            maker.zipStream = new GZipStream(maker.outputStream, CompressionMode.Decompress);
            maker.packer = new Packer(maker.zipStream);
            maker.installInfo = new InstallInfo();
            return true;
        }
    }
}
