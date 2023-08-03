using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MakeInstall
{
    class PackIconStage : Stage
    {
        public PackIconStage(Maker maker, int progress = 0) : base(maker, progress)
        {
        }
        
        public override string Title { get { return "打包窗体图标"; } }

        public override bool Run()
        {
            string outputFile = Path.Combine( Path.GetDirectoryName(maker.packerFile),"iconed_packer.exe");
            
            //FileStream stream = new FileStream(maker.packerFile, FileMode.Open, FileAccess.Read);
            FileStream output = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            maker.AddFileToStream(maker.packerFile, output);

            var buff = maker.buff;
            int offset = 0;
            var nameBytes = Packer.GetPackBytes(maker.config.Name);
            output.Write(nameBytes, 0, nameBytes.Length);
            int iconSize = maker.AddFileToStream(maker.iconFullPath, output);
            Packer.PackInt(buff, ref offset, nameBytes.Length);
            Packer.PackInt(buff, ref offset, iconSize);
            output.Write(buff, 0, offset);
            output.Write(Maker.ICONED_FLAG, 0, Maker.FLAG_SIZE);
            //ImageSource img;
            output.Close();
            maker.packerFile = outputFile;
            return true;
        }
    }
}
