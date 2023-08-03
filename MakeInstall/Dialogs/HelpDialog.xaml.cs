using System;
using System.Collections.Generic;
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
    /// HelpDialog.xaml 的交互逻辑
    /// </summary>
    public partial class HelpDialog : Window
    {
        public HelpDialog()
        {
            InitializeComponent();
            //StringBuilder content =new StringBuilder();
            //content.Append("{\n");
            //content.Append("	\"Name\" : \"Packer\",                      //安装时，在安装目录创建的文件夹名称\n");
            //content.Append("	\"InstallFolder\" : \"C:/Program Files/\",  //安装目录\n");
            //content.Append("	\"Source\" :\"Director/of/Files/to/pack\",  //需要打包的文件夹\n");
            //content.Append("	\"Start\":\"excute.exe\",                   //打包文件夹中的可执行文件\n");
            //content.Append("	\"Filter\": [ \".exe\", \".dll\" ],           //需要打包的文件类型\n");
            //content.Append("	\"DesktopIcon\" : true,                   //在\"桌面\"创建快捷方式\n");
            //content.Append("	\"StartScreen\" : true,                   //固定在“开始”屏幕\n");
            //content.Append("	\"OverrideExists\" : true,                //安装时覆盖已有文件\n");
            //content.Append("	\"Output\":\"installer.exe\"                //打包后的程序名称\n");
            //content.Append("}\n");
            //content.ToString()
            exampleBox.AppendText(Properties.Resources.pack_example);
        }
    }
}
