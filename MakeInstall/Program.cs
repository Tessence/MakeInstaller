using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SimpleArgs;

namespace MakeInstall
{
    class Program
    {

        class LaunchArgs
        {
            [ArgumentParse("-m")]
            public string Mode="ui";


            [ArgumentParse("-c")]
            public string Config = null;
        }


 

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static int Main(string[] args)
        {
            LaunchArgs larg = null; 
            if (args!=null && args.Length>1)
            {
                larg = SimpleParse.Parse<LaunchArgs>(args);
            }
            if (larg != null)
            {
                //Console.Out.WriteLine(string.Format("{0}, {1}",larg.Config,larg.Mode));
                if("batch".Equals(larg.Mode))
                {
                    AttachConsole(ATTACH_PARENT_PROCESS);
                    Maker maker = new Maker(larg.Config);
                    maker.onMessage = OnMakerMessage;
                    maker.onWindowTitle = OnSetTitle;
                    maker.Run();

                    Console.Out.Close();
                    return 0;
                }
            }
            //FreeConsole();
            
            MakeInstall.App app = new MakeInstall.App();
            app.Init();
            app.Run();
            return 0;
        }

        private static void OnMakerMessage(string message, int progress, bool alert)
        {
            string tag = alert ? "[ERROR]" : "[INFO]";
            Console.Out.Write(tag);
            Console.Out.WriteLine(message);            
        }

        private static void OnSetTitle(string title)
        {
            
        }


        private const int ATTACH_PARENT_PROCESS = -1;
        [DllImport("Kernel32.dll", EntryPoint = "AttachConsole", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern void AttachConsole(int dwProcessId);
    }
}
