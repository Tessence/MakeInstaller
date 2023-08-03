using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    class ReplaceIconStage : Stage
    {
        public ReplaceIconStage(Maker maker, int progress = 0) : base(maker, progress)
        {
        }

        public override string Title { get { return "替换图标"; } }

        public override bool Run()
        {
            Process prc = new Process();
            ProcessStartInfo st = prc.StartInfo;
            var config = maker.config;
            string outputFile = maker.outputFile;
            string cache_colder = Path.Combine(Path.GetDirectoryName(outputFile), "cache");
            //System.AppDomain.CurrentDomain.BaseDirectory
            //Path.Combine(maker.configPath, config.Source, config.Icon);
            string iconFullPath = Path.Combine(cache_colder,"favicon.ico");
            iconFullPath = Path.GetFullPath(iconFullPath);
            maker.iconFullPath = iconFullPath;
            
            string temp_file =Path.Combine(cache_colder, "temp_file.exe");
            string selfFile = Process.GetCurrentProcess().MainModule.FileName;
            string scriptPath = Path.GetDirectoryName(selfFile);
            string command = $"{scriptPath}\\compile_res.bat {maker.startFile} {maker.packerFile} {temp_file}";
            OnMessage(command);
            //设置要启动的应用程序
            st.FileName = "cmd.exe";
            st.WorkingDirectory=cache_colder; 
            //st.FileName = "rep_icon.bat";
            st.Arguments = $"/C {command}";
            //是否使用操作系统shell启动
            st.UseShellExecute = false;
            // 接受来自调用程序的输入信息
            st.RedirectStandardInput = false;
            //输出信息
            st.RedirectStandardOutput = true;
            // 输出错误
            st.RedirectStandardError = true;
            //不显示程序窗口
            st.CreateNoWindow = true;
                         //启动程序
            prc.Start();

            //获取输出信息
              string strOuput = prc.StandardOutput.ReadToEnd();
            OnMessage(strOuput);
            //等待程序执行完退出进程
            prc.WaitForExit();
            prc.Close();
            maker.packerFile = temp_file;
            return true;
        }
    }
}
