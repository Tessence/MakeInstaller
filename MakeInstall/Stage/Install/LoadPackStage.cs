using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MakeInstall
{
    class LoadPackStage : Stage
    {
        public LoadPackStage(Maker maker, int progress = 0) : base(maker, progress)
        {

        }

        public override string Title { get { return "加载安装包"; } }

        public override bool Run()
        {
            base.Run();
            
            maker.selfFileInfo = new FileInfo(maker.packerFile);
            int packEnd = (int)maker.selfFileInfo.Length - Maker.FLAG_SIZE - 4 - 1;// last byte is exe type
            FileStream stream = new FileStream(maker.packerFile, FileMode.Open, FileAccess.Read);
            stream.Seek(packEnd, SeekOrigin.Begin);
            stream.Read(maker.buff, 0, 4);
            int offset = 0;
            var packerSize = Packer.UnPackInt(maker.buff, ref offset);
            maker.outputStream = stream;
            maker.packerSize = packerSize;
            
            return true;

        }
    }
}
