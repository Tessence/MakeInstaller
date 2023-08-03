using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    class StatisticStage : Stage
    {
        public StatisticStage(Maker maker, int progress) : base(maker, progress)
        {
        }
        public override string Title { get { return "统计信息"; } }
    }
}
