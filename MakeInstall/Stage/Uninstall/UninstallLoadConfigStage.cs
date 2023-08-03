using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MakeInstall
{
    class UninstallLoadConfigStage : Stage
    {
        public UninstallLoadConfigStage(Maker maker, int progress = 0) : base(maker, progress)
        {

        }

        public override string Title { get { return "加载卸载配置"; } }

        public override bool Run()
        {
            
            maker.selfFileInfo = new FileInfo(maker.packerFile);
            int packEnd = (int)maker.selfFileInfo.Length - Maker.FLAG_SIZE - 4 - 1;
            FileStream stream = new FileStream(maker.packerFile, FileMode.Open, FileAccess.Read);

            stream.Seek(packEnd, SeekOrigin.Begin);
            stream.Read(maker.buff, 0, 4);
            int offset = 0;
            var packerSize = Packer.UnPackInt(maker.buff, ref offset);

            int packStart = packerSize + Maker.PACK_START_SHIFT;
            stream.Seek(packStart, SeekOrigin.Begin);

            
            maker.packer = new Packer(stream);
            var packer = maker.packer;
            var configType = (PackType)packer.UnPackByte();

            if (configType != PackType.CONFIG)
            {
                OnMessage("config segment wrong.", 0, true);
                return false;
            }

            string configContent = maker.packer.UnPackString();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            
            InstallInfo config = serializer.Deserialize<InstallInfo>(configContent);            
            maker.installInfo = config;

            return true;
        }
    }
}
