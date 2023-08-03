using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    class WriteRegTableStage : Stage
    {
        public WriteRegTableStage(Maker maker, int progress = 0) : base(maker, progress)
        {
        }

        public override string Title { get { return "写入注册表"; } }

        public override bool Run()
        {
            base.Run();
            //SOFTWARE在LocalMachine分支下
            foreach(var item in maker.variables)
            {
                OnMessage($" {item.Key} : {item.Value}");
            }
            var regValues = maker.config.RegValues;
            foreach(var reg in regValues)
            {
                WriteRegValue(reg);
                maker.installInfo.AddReg(reg);
            }
            return true;
        }

        private void WriteRegValue(RegValue val)
        {
            RegistryKey rootKey = null;
            switch(val.Type)
            {
                case "HKEY_LOCAL_MACHINE": rootKey = Registry.LocalMachine; break;
                case "HKEY_CURRENT_USER": rootKey = Registry.CurrentUser; break;
                case "HKEY_CLASSES_ROOT": rootKey = Registry.ClassesRoot;break;
                case "HKEY_USERS": rootKey = Registry.Users;break;
                case "HKEY_CURRENT_CONFIG": rootKey = Registry.CurrentConfig;break;
                case "HKEY_PERFORMANCE_DATA": rootKey = Registry.PerformanceData; break;
            }

            if (rootKey == null)
            {
                OnMessage(string.Format("Invalid Regist Type : {0}", val.Type), 0, false);
                return;
            }

            RegistryKey contanerKey = rootKey.OpenSubKey(val.Path, true);
            RegistryKey newKey = contanerKey.CreateSubKey(val.Name);
            var newKeyValues = val.Values;
            foreach(var newVal in newKeyValues)
            {
                string parsed_val = maker.ParseVarString(newVal.Value);
                long long_val;
                if(long.TryParse(parsed_val, out long_val))
                {
                    newKey.SetValue(newVal.Key, long_val, RegistryValueKind.DWord);
                }
                else
                {
                    newKey.SetValue(newVal.Key, parsed_val);
                }
            }
            newKey.Close();
        }
    }
}
