using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    interface MakerHolder
    { 
         

        void OnMakerMessage(string msg, int progress, bool alert);
        void OnSetTitle(string title);

        void OnPage(string page);
    }
}
