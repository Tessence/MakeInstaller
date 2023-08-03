using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    class FinishStage : Stage
    {
        public FinishStage(Maker maker, int progress = 0) : base(maker, progress)
        {
        }
        public override string Title { get { return "已完成"; } }
    }
}
