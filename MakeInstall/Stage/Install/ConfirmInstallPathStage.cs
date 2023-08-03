using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    class ConfirmInstallPathStage : Stage
    {
        public ConfirmInstallPathStage(Maker maker, int progress = 0) : base(maker, progress)
        {

        }

        public override string Title { get { return "确认安装路径"; } }

        public override bool Run()
        {

            return true;
        }
    }
}
