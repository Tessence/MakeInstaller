using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    class DeleteFileStage : Stage
    {
        public DeleteFileStage(Maker maker, int progress = 0) : base(maker, progress)
        {

        }

        public override string Title { get { return "删除已安装文件"; } }

        public override bool Run()
        {
            if (maker.installInfo == null)
            {
                return false;
            }
            var info = maker.installInfo;
            var installPath = info.InstallPath;
            foreach (var file in info.Files)
            {
                var fullPath = Path.Combine(installPath, file);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    OnMessage("已删除文件 : " + fullPath);
                }
                else
                {
                    OnMessage("未找到文件 : " + fullPath);
                }
            }
            return true;
        }
    }
}
