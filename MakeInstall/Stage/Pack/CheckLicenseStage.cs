using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MakeInstall
{
    class CheckLicenseStage : Stage
    {
        public CheckLicenseStage(Maker maker, int progress = 0) : base(maker, progress)
        {
        }

        public override string Title { get { return "检查授权信息"; } }

        public override bool Run()
        {
            base.Run();
            var file = License.LicenseHelper.LicensePath;
            if(string.IsNullOrEmpty(file))
            {
                OnMessage("未找到授权文件", 0, false);
                return false;
            }
            OnMessage("授权文件 : " + file, 0, false);
            return License.LicenseHelper.Licensed(OnLicenseMessage);
        }

        public void OnLicenseMessage(string message, bool error)
        {
            if(error)
            {
                ////MessageBox.Show(message, "授权错误", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                OnMessage("授权错误 : " + message, 0, true);
            }
            else
            {
                OnMessage("授权验证 : " + message, 0, false);
            }
        }
    }
}
