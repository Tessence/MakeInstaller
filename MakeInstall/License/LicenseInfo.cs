using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall.License
{
    public class LicenseInfo
    {
        public string AppName { get; set; }
        public string ExpireTime { get; set; }
        public string MachineID { get; set; }

        public static LicenseInfo FromString(string license)
        {
            if (string.IsNullOrEmpty(license))
            {
                return null;
            }
            LicenseInfo info = new LicenseInfo();
            var infos = license.Split(';');
            info.AppName = infos[0].Split(':')[1];
            info.ExpireTime = infos[1].Split(':')[1];
            info.MachineID = infos[2].Split(':')[1];
            return info;
        }
    }
}
