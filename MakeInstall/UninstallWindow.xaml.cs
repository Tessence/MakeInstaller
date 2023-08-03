using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MakeInstall
{
    /// <summary>
    /// UninstallWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UninstallWindow : MakerWindow
    {
        public UninstallWindow()
        {
            InitializeComponent();
            base_logTextBox = logTextBox;
            if (Maker.packedIcon != null)
            {
                this.Icon = Maker.packedIcon;
            }
            if (!string.IsNullOrEmpty(Maker.packedTitle))
            {
                this.Title = "卸载 " + Maker.packedTitle;
            }
        }

        protected override Maker CreateMaker()
        {
            return new Uninstaller(null);
        }

        private void uninstallBtn_Click(object sender, RoutedEventArgs e)
        {
            if (finished)
            {
                this.Close();
                return;
            }
            uninstallBtn.IsEnabled = false;
            cancelBtn.IsEnabled = false;
            Start(null);
        }

        bool finished = false;
        protected override void OnTaskFinished(string msg)
        {
            finished = true;
            uninstallBtn.Content = "完成卸载";
            uninstallBtn.IsEnabled = true;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        { 
            this.Close();
        }



        private void MakerWindow_Closed(object sender, EventArgs e)
        {
            DeleteItselfByCMD();
        }

        private void DeleteItselfByCMD()
        {
            var installPath = System.IO.Path.GetDirectoryName(maker.packerFile);
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", $"/C ping 1.1.1.1 -n 1 -w 1000 > Nul & rmdir /s /q \"{installPath}\"");
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            Process.Start(psi);
        }

    }
}
