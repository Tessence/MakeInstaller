using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    class ClearRegStage : Stage
    {
        public ClearRegStage(Maker maker, int progress = 0) : base(maker, progress)
        {

        }

        public override string Title { get { return "清理注册表"; } }

        public override bool Run()
        {
            base.Run();
            //SOFTWARE在LocalMachine分支下
            if (maker.installInfo.RegValues == null)
            {
                return true;
            }
            var regValues = maker.installInfo.RegValues;
            foreach (var reg in regValues)
            {
                DeleteRegKey(reg);
            }
            return true;
        }

        private void DeleteRegKey(RegValue val)
        {
            RegistryKey rootKey = null;
            switch (val.Type)
            {
                case "HKEY_LOCAL_MACHINE": rootKey = Registry.LocalMachine; break;
                case "HKEY_CURRENT_USER": rootKey = Registry.CurrentUser; break;
                case "HKEY_CLASSES_ROOT": rootKey = Registry.ClassesRoot; break;
                case "HKEY_USERS": rootKey = Registry.Users; break;
                case "HKEY_CURRENT_CONFIG": rootKey = Registry.CurrentConfig; break;
                case "HKEY_PERFORMANCE_DATA": rootKey = Registry.PerformanceData; break;
            }

            if (rootKey == null)
            {
                OnMessage(string.Format("Invalid Regist Type : {0}", val.Type), 0, false);
                return;
            }

            RegistryKey contanerKey = rootKey.OpenSubKey(val.Path, true);
            if (contanerKey != null)
            {
                contanerKey.DeleteSubKey(val.Name);
                OnMessage(string.Format("删除注册表项 {0}\\{1}\\{2}.",val.Type, val.Path, val.Name));
            }            
        }
    }
 
}
