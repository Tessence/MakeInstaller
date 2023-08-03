using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MakeInstall
{
    class CreateFileStage : Stage
    {
        public CreateFileStage(Maker maker, int progress = 0) : base(maker, progress)
        {
        }

        public override string Title { get { return "创建文件"; } }

        public override bool Run()
        {
            base.Run();
            var packer = maker.packer;
            var buff = maker.buff;
            var zipStream = maker.zipStream;
            int progress = this.progress;
            long total_size = 0;
            while (true)
            {
                PackType ptype = (PackType)packer.UnPackByte();
                if (ptype != PackType.FILE_NAME)
                {
                    break;
                }
                var fileName = packer.UnPackString();
                maker.installInfo.AddFile(fileName);

                progress += 3;
                var writeFullPath = Path.Combine(maker.installFolder, fileName);
                var writeDirectory = Path.GetDirectoryName(fileName);
                CreateDir(maker.installFolder, writeDirectory);
                ptype = (PackType)packer.UnPackByte();
                if (ptype != PackType.FILE_DATA)
                {
                    break;
                }
                if (File.Exists(writeFullPath))
                {
                    File.Delete(writeFullPath);
                }
                FileStream fileStream = new FileStream(writeFullPath, FileMode.Create, FileAccess.Write);
                OnMessage("Create " + writeFullPath, progress, false);
                int fileLength = packer.UnPackInt();
                total_size += fileLength;
                while (fileLength > 0)
                {
                    int readSize = fileLength;
                    if (fileLength > Maker.STREAM_BUFF_SIZE)
                    {
                        readSize = Maker.STREAM_BUFF_SIZE % fileLength;
                    }
                    int size = zipStream.Read(buff, 0, readSize);
                    fileLength -= size;
                    fileStream.Write(buff, 0, size);
                }
                OnMessage("Done .", 0, false);
                fileStream.Close();
            }
            maker.SetVar("install_size", total_size/1024);
            return true;
        }

        private void CreateDir(string root, string dirs)
        {
            var dirInfo = dirs.Split('/');
            foreach (var dir in dirInfo)
            {
                root = Path.Combine(root, dir);
                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }
            }
        }

        public override void Finish()
        {
            base.Finish();
            maker.zipStream.Close();
        }
    }
}
