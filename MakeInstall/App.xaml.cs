using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MakeInstall
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {


        public void Init()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;
            var exeType = Maker.GetExeType();
            string startUri = "MainWindow.xaml";
            switch (exeType)
            {
                case ExeType.Packed: startUri = "InstallWindow.xaml"; break;
                case ExeType.Uninstaller: startUri = "UninstallWindow.xaml"; break;
            }

 
            this.StartupUri = new System.Uri(startUri, System.UriKind.Relative); 
            System.Uri resourceLocater = new System.Uri("/MakeInstall;component/app.xaml", System.UriKind.Relative);
            System.Windows.Application.LoadComponent(this, resourceLocater);
        }
    }
}
