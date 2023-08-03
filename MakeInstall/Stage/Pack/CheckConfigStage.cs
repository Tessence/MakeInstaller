using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MakeInstall
{
    class CheckConfigStage : Stage
    {
        public CheckConfigStage(Maker maker, int progress = 0) : base(maker, progress)
        {
        }

        public override string Title { get { return "检查打包配置"; } }

        public override bool Run()
        {
            var config = maker.config;
            if(config == null)
            {
                var serializer = new JavaScriptSerializer();
                config = serializer.Deserialize<PackConfig>(maker.configContent);
                maker.config = config;
            }

            OnMessage("检查打包源文件路径");
            if (config.Source[1]== ':')
            {
                maker.sourcePath = Path.GetFullPath(config.Source);
            }
            else
            {
                OnMessage("使用相对配置文件的路径 : " + config.Source);
                maker.sourcePath = Path.Combine(maker.configPath, config.Source);
                OnMessage("绝对路径为 : " + maker.sourcePath);
            }

            if(!Directory.Exists(maker.sourcePath))
            {
                OnMessage("路径 : " + maker.sourcePath +" 不存在");
                return false;
            }
            
            maker.startFile = Path.Combine(maker.sourcePath, config.Start);
            if(!File.Exists(maker.startFile))
            {
                OnMessage("配置的Start文件 : " + maker.sourcePath + " 不存在");
                return false;
            }

            OnMessage("检查输出文件路径");
            OnMessage("相对配置文件路径为 : " + config.Output);
            maker.outputFile = Path.Combine(maker.configPath, config.Output);
            var outputPath = Path.GetDirectoryName(maker.outputFile);
            if(!Directory.Exists(outputPath))
            {
                OnMessage("路径 : " + outputPath +" 不存在");
                return false;
            }

            var uninstallerName = maker.config.Uninstaller;
            OnMessage("卸载入口配置 : " + uninstallerName);
            if (string.IsNullOrEmpty(uninstallerName) || string.IsNullOrWhiteSpace(uninstallerName))
            {
                uninstallerName = "uninstaller.exe";
                OnMessage("使用默认名称 : " + uninstallerName);

                maker.config.Uninstaller = uninstallerName;
            }
            

            if (config.Filter == null || config.Filter.Count < 1)
            {
                OnMessage("文件后缀过滤未配置，将打包所有类型文件.");
            }

            return true;
        }
    }
}
