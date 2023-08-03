using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace MakeInstall
{
    /// <summary>
    /// InstallWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InstallWindow : MakerWindow, MakerHolder
    {
        public InstallWindow()
        {
            InitializeComponent();
            base_logTextBox = logTextBox;
            if (Maker.packedIcon != null)
            {
                this.Icon = Maker.packedIcon;
            }
            if (!string.IsNullOrEmpty(Maker.packedTitle))
            {
                this.Title = "安装 " + Maker.packedTitle;
            }
        }

        protected override Maker CreateMaker()
        {
            return new Installer(null);
        }

        protected override void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            base.OnWindowLoaded(sender, e);
            installPathText.Text = maker.installFolder;
            fixInStartScreen.IsChecked = maker.config.StartScreen;
            createLinkOnDesktop.IsChecked = maker.config.DesktopIcon;
        }

        bool finished = false;
        protected override void OnTaskFinished(string msg)
        {
            //base.OnTaskFinished(msg);
            finished = true;
            installBtn.Content = "完成";
            installBtn.IsEnabled = true;
        }

        private void selectInstallPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "选择安装路径";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                installPathText.Text = dialog.SelectedPath;
            }
            
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void installBtn_Click(object sender, RoutedEventArgs e)
        {
            if (finished)
            {
                this.Close();
                return;
            }

            maker.installFolder = installPathText.Text;
            maker.createDesktopLink = createLinkOnDesktop.IsChecked;
            maker.fixInStartScreen = fixInStartScreen.IsChecked;

            if (Directory.Exists(maker.installFolder))
            {
                var files = Directory.GetFiles(maker.installFolder);
                var dirs = Directory.GetDirectories(maker.installFolder);
                if ((files != null && files.Length > 0) || (dirs != null && dirs.Length > 0))
                {
                    System.Windows.Forms.MessageBox.Show(
                        "安装文件夹包含文件/文件夹,可能会覆盖文件", "失败",MessageBoxButtons.OK , MessageBoxIcon.Error);
                    
                    return;
                }
            }
            installBtn.IsEnabled = false;
            cancelBtn.IsEnabled = false;
            Start(null);
        }
    }
}
