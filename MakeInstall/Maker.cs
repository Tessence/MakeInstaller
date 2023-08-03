using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MakeInstall
{

    public delegate void MakerMessage(string message, int progress, bool alert);
    public delegate void OnSingleMessage(string message);

    public enum ExeType : byte
    {
        None = 0,
        Packer = 1,
        Packed = 2,
        Uninstaller = 3,
    }

    public class Maker
    {
        public const int STREAM_BUFF_SIZE = 4096;
        public const int PACK_START_SHIFT = 512;
        public const int FLAG_SIZE = 64;

        public string configFile;
        public string configPath;
        public string sourcePath;
        public string startFile;
        public string outputFile;
        public string packerFile;
        public string installFolder;
        public string iconFullPath;
        public bool? createDesktopLink;
        public bool? fixInStartScreen;

        public InstallInfo installInfo;
        public PackConfig config;
        public FileStream outputStream;
        public GZipStream zipStream;
        public Packer packer = null;
        public FileInfo selfFileInfo = null;
        public int packerSize = 0;
        public string configContent;

        public HashSet<string> fileNameSet = new HashSet<string>();
        

        public MakerMessage onMessage;
        public byte[] buff = new byte[STREAM_BUFF_SIZE];
        public OnSingleMessage onWindowTitle;
        public OnSingleMessage onPage;
        public static  byte[] BAKED_FLAG = new byte[FLAG_SIZE] { 146, 82, 187, 91, 183, 134, 82, 46, 26, 139, 1, 46, 134, 124, 124, 225, 0, 216, 230, 247, 148, 244, 171, 128, 206, 194, 77, 251, 65, 158, 4, 16, 97, 70, 44, 111, 120, 147, 117, 189, 101, 185, 229, 30, 162, 236, 3, 162, 140, 124, 16, 229, 248, 97, 190, 25, 47, 136, 210, 191, 115, 171, 10, 127 };
        public static byte[]  UNINSTALL_FLAG = new byte[FLAG_SIZE] { 16, 182, 187, 91, 183, 134, 82, 45, 216, 109, 1, 46, 134, 124, 124, 225, 0, 216, 230, 247, 148, 244, 171, 128, 206, 194, 77, 251, 65, 158, 4, 16, 97, 70, 44, 111, 120, 147, 117, 189, 101, 185, 229, 30, 162, 236, 3, 162, 140, 124, 16, 229, 148, 97, 190, 25, 47, 132, 210, 191, 152, 171, 10, 123 };
        public static byte[]  ICONED_FLAG = new byte[FLAG_SIZE] { 16, 162, 87, 91, 113, 34, 82, 45, 21, 19, 51, 46, 134, 124, 124, 225, 0, 216, 230, 247, 148, 244, 171, 128, 206, 194, 77, 251, 65, 158, 4, 16, 97, 70, 44, 111, 120, 147, 117, 189, 101, 185, 229, 30, 12, 26, 33, 162, 140, 124, 16, 29, 18, 197, 190, 25, 47, 232, 243, 11, 52, 179, 90, 23 };
        delegate int OnAddFileToStream(string fileName, Stream stream);
        public Dictionary<string, object> variables = new Dictionary<string, object>();

        private static ExeType checkedType = 0; 
        public static ImageSource packedIcon = null;
        public static string packedTitle = string.Empty;

        protected List<Stage> stages;
        protected List<Stage> prepareStages ;

        public Maker(string configFile)
        {
            this.configFile = configFile;

            packerFile = Process.GetCurrentProcess().MainModule.FileName;
            //packerFile = "F:/PerforceWorkspaces/tqz_MakeInstall/client/MakeInstall/simoou-installer.exe";
            //packerFile = "F:/PerforceWork/tqz_EASONTANG-NB0_Scripts_3124/client/MakeInstall/pack/simoou-installer.exe";
            //packerFile = "C:/Program Files/simoou/uninstaller.exe";
            selfFileInfo = new FileInfo(packerFile);
        }

        public virtual void Load()
        {
            stages = new List<Stage>();
            stages.Add(new CheckLicenseStage(this, 2));
            stages.Add(new LoadConfigStage(this, 5));
            stages.Add(new CheckConfigStage(this, 10));
            stages.Add(new ReplaceIconStage(this, 12));
            stages.Add(new PackIconStage(this, 23));
            stages.Add(new StartStage(this, 35));
            stages.Add(new StatisticStage(this, 55));
            stages.Add(new PackFileStage(this, 75));
            stages.Add(new EndStage(this, 90));
            stages.Add(new FinishStage(this, 100));            
        }

        public  void Prepare()
        {
            if (prepareStages == null) return;

            foreach (var stage in prepareStages)
            {
                bool result = stage.Run();
                if (!result)
                {
                    onMessage(stage.Title + " Failed", 0, false);
                    break;
                }
                stage.Finish();
            }
        }

        public  void Run()
        {
            foreach (var stage in stages)
            {
                bool result = stage.Run();
                if (!result)
                {
                    onMessage(stage.Title + " Failed", 0, false);
                    break;
                }
                stage.Finish();
            }
        }

        public virtual void Unload()
        {

        }

        public static ExeType GetExeType()
        {
            if(checkedType > 0)
            {
                return checkedType;
            }
            //packerFile = "F:/PerforceWorkspaces/tqz_MakeInstall/client/MakeInstall/simoou-installer.exe";
            //packerFile = "C:/Program Files/simoou/uninstaller.exe";

            var packerFile = Process.GetCurrentProcess().MainModule.FileName;
            FileStream selfStream = new FileStream(packerFile, FileMode.Open, FileAccess.Read);
            const int sizeShift = 4;
            var selfFileInfo = new FileInfo(packerFile);
            long flagShift = selfFileInfo.Length - FLAG_SIZE - 1;// last byte is ExeType
            flagShift = flagShift - sizeShift;// packerSize is at the front of flag;
            
            byte[] buff = new byte[FLAG_SIZE + 5];
            selfStream.Seek(flagShift, SeekOrigin.Begin);
            selfStream.Read(buff, 0, FLAG_SIZE + 5);
            int offset = 0;

      

            ExeType exeType = (ExeType)buff[FLAG_SIZE + sizeShift];
            byte[] flagBytes = null;
            switch(exeType)
            {
                case ExeType.Packed:
                    flagBytes = BAKED_FLAG;break;
                case ExeType.Uninstaller:
                    flagBytes = UNINSTALL_FLAG;break;
            }

            checkedType =  ExeType.Packer;

            
            if (flagBytes != null)
            {                
                
                if (CompareBytes(BAKED_FLAG, buff, sizeShift))
                {
                    checkedType = ExeType.Packed;
                }

                if (checkedType == ExeType.Packer)
                {
                    if(CompareBytes(UNINSTALL_FLAG,buff,sizeShift))
                    {
                        checkedType = ExeType.Uninstaller;
                    }                    
                }
            }

            // unpack icon 
            if (checkedType == ExeType.Packed || checkedType == ExeType.Uninstaller)
            {
                int packerSize = Packer.UnPackInt(buff, ref offset);
                if (packerSize > 0)
                {
                    int packInfoSize = ICONED_FLAG.Length + 8;
                    byte[] iconInfoBuff = new byte[packInfoSize];
                    selfStream.Seek(packerSize - packInfoSize, SeekOrigin.Begin);
                    selfStream.Read(iconInfoBuff, 0, packInfoSize);
                    if (CompareBytes(ICONED_FLAG, iconInfoBuff, 8))
                    {
                        offset = 0;
                        int titleDataSize = Packer.UnPackInt(iconInfoBuff, ref offset);
                        int iconDataSize = Packer.UnPackInt(iconInfoBuff, ref offset);

                        byte[] titleDataBuff = new byte[titleDataSize];
                        selfStream.Seek(packerSize - packInfoSize - iconDataSize - titleDataSize, SeekOrigin.Begin);
                        selfStream.Read(titleDataBuff, 0, titleDataSize);
                        offset = 0;
                        packedTitle = Packer.UnPackString(titleDataBuff, ref offset, titleDataSize);

                        selfStream.Seek(packerSize - packInfoSize - iconDataSize, SeekOrigin.Begin);

                        MemoryStream ms = new MemoryStream();
                        int iconDataBuffSize = 512;
                        byte[] iconDataBuff = new byte[iconDataBuffSize];
                        while (iconDataSize > 0)
                        {
                            selfStream.Read(iconDataBuff, 0, iconDataBuffSize);
                            ms.Write(iconDataBuff, 0, iconDataBuffSize);
                            iconDataSize -= iconDataBuffSize;
                        }
                        
                        BitmapImage img = new BitmapImage();
                        img.BeginInit();
                        img.CacheOption = BitmapCacheOption.OnLoad; // here
                        img.StreamSource = ms;
                        img.EndInit();
                        packedIcon = img;
                    }
                }
            }
            selfStream.Close();

            return checkedType;
        }

        private static bool CompareBytes(byte[] flag, byte[] data,int dataShift)
        {
            for (int i = 0; i < FLAG_SIZE; ++i)
            {
                if (flag[i] != data[i + dataShift])
                {
                    return false;
                    
                }
            }
            return true;
        }

         

 

        public void SetVar(string name, object val)
        {
            variables["${" + name + "}"] = val;
        }

        public string ParseVarString(string str)
        {            
            Regex reg = new Regex("\\$\\{([a-z_]*)\\}");
            var matches = reg.Matches(str);
            foreach(var mt in matches)
            {
                var var_name = mt.ToString();
                object val;
                variables.TryGetValue(var_name, out val);
                if (val != null)
                {
                    str = str.Replace(var_name, val.ToString());
                }
            }
            return str;
        }

 
  
              
 


  

        public int AddFileToStream(string file, Stream stream)
        {
            if (File.Exists(file))
            {
                onMessage("pack file : " + file, 0, false);

                FileStream srcStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                int totalSize = 0;
                while (true)
                {
                    int size = srcStream.Read(buff, 0, STREAM_BUFF_SIZE);
                    if (size < 1)
                    {
                        break;
                    }
                    totalSize += size;
                    stream.Write(buff, 0, size);
                }
                stream.Flush();
                srcStream.Close();
                return totalSize;
            }
            return 0;
        }
    }
}
