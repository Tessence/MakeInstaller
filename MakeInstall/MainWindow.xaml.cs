using MakeInstall.Dialogs;
using MakeInstall.License;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace MakeInstall
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MakerWindow, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public Visibility IsBaker
        {
            get
            {
                return _isBaked ? Visibility.Hidden : Visibility.Visible;
            }
        }
 
        public MainWindow()
        {
            InitializeComponent();

            base_logTextBox = logTextBox;
            base_progressBar = packProgressBar;
            base_progressMessage = progressMessage;
    }


        string configFile = string.Empty;
        private void selectConfigButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog= new OpenFileDialog();
            dialog.Filter = "*.json|*.json";
            var result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                packConfigTextBox.Text = dialog.FileName;
                //configFile = Path.Combine(packConfigTextBox.Text, "pack.json");
                configFile = packConfigTextBox.Text;
                if (!File.Exists(configFile))
                {
                    //packConfigTextBox.Text = "";
                    //configFile = string.Empty;
                    System.Windows.MessageBox.Show("该路径下不包含打包配置文件（pack.json）", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }         
         

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            //if (!maker.IsLoaded())
            //{
            //    return;
            //}                        
            Start(configFile);
        }


        private void exampleInfo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            HelpDialog dialog = new HelpDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void licenceInfo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            LicenseDialog dialog = new LicenseDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _isBaked = Maker.GetExeType() != ExeType.Packer;
            //if ()
            {
                PropertyChanged(this, new PropertyChangedEventArgs("IsBaker"));
            }
        }

        private void exploreButton_Click(object sender, RoutedEventArgs e)
        {
            if (maker.config == null)
            {
                return;
            }
            string outputFile = Path.Combine( maker.configPath, maker.config.Output);
            string outputPath = Path.GetDirectoryName(outputFile);
            System.Diagnostics.Process.Start("explorer.exe", outputPath);
        }

   

    }
}
