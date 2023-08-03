using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    public class Stage
    {
        public bool silent;
        public int progress;
        public virtual string Title { get { return "Base"; } }

        protected Maker maker;

        public Stage(Maker maker, int progress = 0)
        {
            this.maker = maker;
            this.progress = progress;
        }

      
        public virtual bool Run()
        {
            if (!silent)
            {
                maker.onMessage(Title, progress, false);                
            }
            return true;
        }

        public virtual void Finish()
        {

        }

        public virtual void Progress()
        {

        }

        public void OnMessage(string msg, int progress = 0, bool alert = false)
        {
            if (silent)
            {
                return;
            }
            maker.onMessage(msg, progress, alert);
        }

        public void OnPage(string pageName)
        {
            if(maker.onPage != null)
            {
                maker.onPage(pageName);
            }
        }

    }
}
