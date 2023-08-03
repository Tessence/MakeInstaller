using MakeInstall.License;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace MakeInstall.Dialogs
{
    /// <summary>
    /// LicenseDialog.xaml 的交互逻辑
    /// </summary>
    public partial class LicenseDialog : Window
    {        
        public LicenseDialog()
        {
            InitializeComponent();             
            //Uri privateKey = new Uri("/Key/rsa_private_key.pem");
            //var info = Application.GetResourceStream(privateKey);
            //StreamReader keyReader = new StreamReader(info.Stream);
            //var privateKeyContent = keyReader.ReadToEnd();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowLicenseInfo();
        }

        private void ShowLicenseInfo()
        {
            //            LicenseHelper.CreateLicense("Key/rsa_public_key.pem", "license.txt");            
            localMachineID.Text = LicenseHelper.GetLocalUniqueID();
            currentAppName.Text = LicenseHelper.APP_NAME;
            var lic = LicenseHelper.LoadLicense(Properties.Resources.rsa_private_key);
            if (lic != null)
            {
                appName.Text = lic.AppName;
                expireTime.Text = lic.ExpireTime;
                machineId.Text = lic.MachineID;
            }
            if (LicenseHelper.Licensed(OnLicenseInfo))
            {
                licenseBox.Header = "授权信息";
                licenseBox.Foreground = new SolidColorBrush(Color.FromArgb(255, 10, 10, 10));
            }
            else
            {
                licenseBox.Header = "授权错误";
                licenseBox.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 10, 10));
            }
        }


        private  string defaultLicensePath = "license.txt";
        private void OnLicenseInfo(string msg, bool error)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "license.txt|*.txt";
            var result = dialog.ShowDialog();
            defaultLicensePath = System.IO.Path.GetFullPath(defaultLicensePath);
            if (result.HasValue && result.Value)
            {
                var content = File.ReadAllText(dialog.FileName);
                if (File.Exists(defaultLicensePath))
                {
                    var confirm = MessageBox.Show("授权文件" + defaultLicensePath + " 已存在\n添加OK覆盖", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (confirm != MessageBoxResult.OK)
                    {
                        return;
                    }
                }
                File.WriteAllText(defaultLicensePath, content);
                LicenseHelper.LoadLicense(Properties.Resources.rsa_private_key, true);

                ShowLicenseInfo();
            }
        }

        private void emailConsult_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //< a target = "_blank" href = "http://mail.qq.com/cgi-bin/qm_share?t=qm_mailme&email=8ZaenZiXlLGXnomckJid35KenA" style = "text-decoration:none;" > 邮件咨询 </ a >
            //< a target = "_blank" href = "http://mail.qq.com/cgi-bin/qm_share?t=qm_mailme&email=8YKYnJ6ehLGAgN_Snpw" style = "text-decoration:none;" > 给我写信 </ a >
                 Process.Start("http://mail.qq.com/cgi-bin/qm_share?t=qm_mailme&email=8YKYnJ6ehLGAgN_Snpw");
        }

        private void orgSiteBtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //https://www.simoou.com/#/tool/makeinstall
            Process.Start("https://codethings.cc");
        }
    }
}
