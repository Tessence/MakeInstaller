using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    class Installer : Maker
    {
        public Installer(string configFile) : base(configFile)
        {
        }

        public override void Load()
        {
            prepareStages = new List<Stage>();
            prepareStages.Add(new LoadPackStage(this, 0));
            prepareStages.Add(new SeekToPackStartStage(this, 0));
            prepareStages.Add(new UnPackConfigStage(this, 0));
            foreach(var stage in prepareStages)
            {
                stage.silent = true;
            }

            stages = new List<Stage>();
            //stages.Add(new CheckConfigStage(this, 12));
            stages.Add(new SeekToPackStartStage(this, 2));
            stages.Add(new UnPackConfigStage(this, 8));
            stages.Add(new CreateFileStage(this, 15));
            stages.Add(new CreateLinkStage(this, 75));
            stages.Add(new CreateStartScreenStage(this, 80));
            stages.Add(new WriteRegTableStage(this, 85));
            stages.Add(new CreateUninstallStage(this, 90));
            stages.Add(new InstallFinishStage(this, 100));
        }

   
    }
}
