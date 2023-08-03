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
    class CreateUninstallStage : Stage
    {
        public CreateUninstallStage(Maker maker, int progress = 0) : base(maker, progress)
        {
        }

        public override string Title { get { return "创建卸载程序"; } }

        public override bool Run()
        {
            base.Run();
            var makerConfig = maker.config;
            var uninstallerName = makerConfig.Uninstaller;
            OnMessage("卸载入口配置 : " + uninstallerName);
            //if (string.IsNullOrEmpty(uninstallerName) || string.IsNullOrWhiteSpace(uninstallerName))
            //{
            //    uninstallerName = "uninstaller.exe";
            //    OnMessage("使用默认名称 : " + uninstallerName);
            //    maker.config.Uninstaller = uninstallerName;
            //}

            string uninstaller = Path.Combine(maker.installFolder, makerConfig.Uninstaller);
            OnMessage("创建卸载程序" + uninstaller, 0, false);

            FileStream pstream = new FileStream(maker.packerFile, FileMode.Open, FileAccess.Read);
            FileStream ustream = new FileStream(uninstaller, FileMode.Create, FileAccess.Write);
            var packerLen = maker.packerSize;

            var buff = maker.buff;
            var buffLen = buff.Length;
            var len = packerLen;
            while (len > 0)
            {
                if (len < buffLen)
                {
                    buffLen = len;
                }
                int readLen = pstream.Read(buff, 0, buffLen);
                ustream.Write(buff, 0, readLen);
                len -= readLen;
            }
            pstream.Close();
            for (int i = 0; i < Maker.PACK_START_SHIFT; ++i)
            {
                maker.buff[i] = 0;
            }
            ustream.Write(maker.buff, 0, Maker.PACK_START_SHIFT);

            Packer packer = new Packer(ustream);

            var info = maker.installInfo;
            
            packer.Pack(PackType.CONFIG);
            OnMessage("写入已安装配置");
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var installContent = serializer.Serialize(info);
            packer.Pack(installContent);

            packer.Pack(maker.packerSize);
            packer.Pack(Maker.UNINSTALL_FLAG);
            packer.Pack(ExeType.Uninstaller);
            OnMessage("写入" + uninstaller + "完成", 0, false);
            ustream.Close();

            return true;
        }

    }
}
