using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    class RemoveLinkStage : Stage
    {
        public RemoveLinkStage(Maker maker, int progress = 0) : base(maker, progress)
        {
        }
    }
}
