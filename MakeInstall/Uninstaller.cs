using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    class Uninstaller : Installer
    {
        public Uninstaller(string configFile) : base(configFile)
        {

        }

        public override void Load()
        {
         
            stages = new List<Stage>();
            stages.Add(new UninstallLoadConfigStage(this, 5));
            stages.Add(new ClearRegStage(this, 15));
            stages.Add(new DeleteFileStage(this, 90));
            stages.Add(new InstallFinishStage(this, 100));
             
        }
    }
}
