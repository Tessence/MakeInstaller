using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall 
{
    class PackFileStage : Stage
    {
        public PackFileStage(Maker maker, int progress) : base(maker, progress)
        {
        }

        public override string Title { get { return "打包文件"; } }

        public override bool Run()
        {
            base.Run();
            bool allFile = false;
            foreach(var filter in maker.config.Filter)
            {
                if (filter.Equals(".*"))
                {
                    allFile = true;
                    break;
                }
            }
            GetSourceFileToPack(maker.sourcePath, maker.sourcePath, allFile, maker.zipStream);
            return true;
        }


        private int GetSourceFileToPack(string root, string folder, bool allFile, Stream stream)
        {
            int packByteCount = 0;
            PackConfig config = maker.config;
            Packer packer = maker.packer;
            var dirs = Directory.GetDirectories(folder);
            foreach (var dir in dirs)
            {
                packByteCount += GetSourceFileToPack(root, dir, allFile, stream);
            }
            var files = Directory.GetFiles(folder);
            foreach (var file in files)
            {
                if (!allFile && config.Filter != null && config.Filter.Count > 0)
                {

                    bool isNeed = false;
                    foreach (var filter in config.Filter)
                    {
                        if (file.EndsWith(filter, StringComparison.OrdinalIgnoreCase))
                        {
                            isNeed = true;
                            break;
                        }
                    }
                    if (!isNeed)
                    {
                        continue;
                    }
                }
                //if ()
                {
                    string relativeFileName = file.Substring(root.Length + 1);
                    maker.fileNameSet.Add(relativeFileName);

                    packByteCount += packer.Pack(PackType.FILE_NAME);
                    packByteCount += packer.Pack(relativeFileName);
                    packByteCount += packer.Pack(PackType.FILE_DATA);
                    FileInfo info = new FileInfo(file);
                    var len = info.Length;
                    packByteCount += packer.Pack((int)len);

                    maker.AddFileToStream(file, stream);
                }
            }

            return packByteCount;
        }
    }
}
