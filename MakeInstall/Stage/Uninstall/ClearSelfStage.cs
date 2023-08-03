using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    class ClearSelfStage : Stage
    {
        public ClearSelfStage(Maker maker, int progress = 0) : base(maker, progress)
        {
        }

        public override string Title { get { return "清理文件夹"; } }

        public override bool Run()
        {
            return base.Run();


        }

        /// <summary>
        /// 删除程序自身（本文地址：http://www.cnblogs.com/Interkey/p/DeleteItself.html）
        /// </summary>
   
    }
}
