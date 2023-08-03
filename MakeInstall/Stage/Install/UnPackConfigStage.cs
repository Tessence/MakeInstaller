using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MakeInstall
{
    class UnPackConfigStage : Stage
    {
        public UnPackConfigStage(Maker maker, int progress = 0) : base(maker, progress)
        {
        }

        public override string Title { get { return "解析配置"; } }

        public override bool Run()
        {
            base.Run();

            PackType configType = (PackType)maker.packer.UnPackByte();
            if (configType != PackType.CONFIG)
            {
                OnMessage("config segment wrong.", 0, true);
                return false;
            }
            string configContent = maker.packer.UnPackString();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            PackConfig config = serializer.Deserialize<PackConfig>(configContent);

            if(string.IsNullOrEmpty(maker.installFolder))
            {
                string installFolder = Path.Combine(config.InstallFolder, config.Name);
                if (!Directory.Exists(installFolder))
                {
                    Directory.CreateDirectory(installFolder);
                }
                maker.installFolder = installFolder;
            }

            maker.config = config;
            maker.configContent = configContent;
            if (maker.onWindowTitle != null)
            {
                maker.onWindowTitle(config.Name);
            }
            maker.SetVar("install_path",maker.installFolder);
            maker.SetVar("uninstaller_file", config.Uninstaller);

            maker.installInfo.InstallPath = maker.installFolder;
            return true;
        }
    }
}
