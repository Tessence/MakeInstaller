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
    class StartStage : Stage
    {
        public StartStage(Maker maker, int progress) : base(maker, progress)
        {

        }
        public override string Title { get { return "开始打包"; } }
        public override bool Run()
        {
            base.Run();
            
            maker.outputStream = new FileStream(maker.outputFile, FileMode.Create, FileAccess.Write);
            maker.packerSize = maker.AddFileToStream(maker.packerFile, maker.outputStream);
            // write 512 empty byte for safe;
            for (int i = 0; i < Maker.PACK_START_SHIFT; ++i)
            {
                maker.buff[i] = 0;
            }

            maker.outputStream.Write(maker.buff, 0, Maker.PACK_START_SHIFT);
            maker.zipStream = new GZipStream(maker.outputStream, CompressionMode.Compress);
            maker.packer = new Packer(maker.zipStream);
            maker.packer.Pack(PackType.CONFIG);

            //config maybe changed after check.
            var serializer = new JavaScriptSerializer();
            maker.configContent = serializer.Serialize(maker.config);
            maker.packer.Pack(maker.configContent);
            return true;
        }
    }
}
