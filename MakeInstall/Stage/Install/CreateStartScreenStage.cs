using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    class CreateStartScreenStage : CreateLinkStage
    {
        public CreateStartScreenStage(Maker maker, int progress = 0) : base(maker, progress)
        {
        }

        public override string Title { get { return "固定在开始屏幕"; } }

        public override bool Run()
        {
            if (maker.fixInStartScreen == null || maker.fixInStartScreen.Value == false)
            {
                return true;
            }
            if (!silent)
            {
                maker.onMessage(Title, progress, false);
            }
            CreateShortcutOnStartMenu(maker.config.Name, TargetPath);
            return true;
        }

        private void CreateShortcutOnStartMenu(string shortcutName, string targetPath,
       string description = null, string iconLocation = null)
        {
            string shortcatPath = Environment.GetFolderPath( Environment.SpecialFolder.CommonStartMenu);//获取开始菜单文件夹路径 "Programs")
            shortcatPath = Path.Combine(shortcatPath, "Programs");
            CreateShortcut(shortcatPath, shortcutName, targetPath, description, iconLocation);
        }
    }
}
