using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MakeInstall 
{
    class LoadConfigStage : Stage
    {
        public LoadConfigStage(Maker maker,int progress) : base(maker,progress)
        {
        }
        public override string Title { get { return "加载配置"; } }
        public override bool Run()
        {
            base.Run();
            if (string.IsNullOrEmpty(maker.configFile))
            {
                OnMessage("请先选择打包配置文件", 0, true);
                return false;
            }
            if (!File.Exists(maker.configFile))
            {
                OnMessage("打包配置文件不存在。", 0, true);
                return false;
            }

            maker.configPath = Path.GetDirectoryName(maker.configFile);
            maker.configContent = File.ReadAllText(maker.configFile);

            //var serializer = new JavaScriptSerializer();
            //maker.config = serializer.Deserialize<PackConfig>(maker.configContent);
            //maker.sourcePath = Path.Combine(maker.configPath, maker.config.Source);

            //maker.startFile = Path.Combine(maker.configPath, maker.config.Source, maker.config.Start);
            //maker.outputFile = Path.Combine(maker.configPath, maker.config.Output);

            //maker.sourcePath = Path.GetFullPath(maker.sourcePath);
            //maker.startFile = Path.GetFullPath(maker.startFile);

            return true;
        }
    }
}
