using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    class InstallFinishStage : Stage
    {
        public InstallFinishStage(Maker maker, int progress = 0) : base(maker, progress)
        {
        }

        public override string Title { get { return "完成"; } }

        public override bool Run()
        {
            return base.Run();
        }
    }
}
