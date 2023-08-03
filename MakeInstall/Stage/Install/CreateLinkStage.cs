using IWshRuntimeLibrary;
using System;
using System.IO;

namespace MakeInstall
{
    class CreateLinkStage : Stage
    {
        public CreateLinkStage(Maker maker, int progress = 0) : base(maker, progress)
        {
        }

        public override string Title { get { return "创建快捷方式"; } }

        public override bool Run()
        {
            if (maker.createDesktopLink == null || maker.createDesktopLink.Value == false)
            {
                return true;
            }
            base.Run();
            string linkName = maker.config.Name;
            CreateShortcutOnDesktop(linkName, TargetPath);
            return true;
        }
         
        public string TargetPath
        {
            get
            {
                var config = maker.config;
                string targetPath = Path.Combine(maker.installFolder, config.Start);
                targetPath = targetPath.Replace("/", "\\");
                return targetPath;
            }
        }

        private void CreateShortcutOnDesktop(string shortcutName, string targetPath,
             string description = null, string iconLocation = null)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);//获取桌面文件夹路径
            CreateShortcut(desktop, shortcutName, targetPath, description, iconLocation);
        }


        /// <summary>
        /// 创建快捷方式
        /// </summary>
        /// <param name="directory">快捷方式所处的文件夹</param>
        /// <param name="shortcutName">快捷方式名称</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="description">描述</param>
        /// <param name="iconLocation">图标路径，格式为"可执行文件或DLL路径, 图标编号"，
        /// 例如System.Environment.SystemDirectory + "\\" + "shell32.dll, 165"</param>
        /// <remarks></remarks>
        public void CreateShortcut(string directory, string shortcutName, string targetPath,
            string description = null, string iconLocation = null)
        {
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            string shortcutPath = Path.Combine(directory, string.Format("{0}.lnk", shortcutName));
            maker.installInfo.AddFile(shortcutPath);

            OnMessage(string.Format("Create lnk : {0} -> {1}", shortcutPath, targetPath), 0, false);
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);//创建快捷方式对象
            shortcut.TargetPath = targetPath;//指定目标路径
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);//设置起始位置
            shortcut.WindowStyle = 1;//设置运行方式，默认为常规窗口
            shortcut.Description = description;//设置备注
            shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation;//设置图标路径
            shortcut.Save();//保存快捷方式
        }
    }
}
