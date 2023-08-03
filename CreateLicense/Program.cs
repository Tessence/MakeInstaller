using MakeInstall.License;
using SimpleArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateLicense
{
    class Program
    {
        class LaunchArgs
        {
            [ArgumentParse("-t")]
            public string type;

            [ArgumentParse("-i")]
            public string uid;

            [ArgumentParse("-l")]
            public string license;

            [ArgumentParse("-p")]
            public string pubKey;

            [ArgumentParse("-d")]
            public int days;

        }

        static void Main(string[] args)
        {
            var largs = SimpleArgs.SimpleParse.Parse<LaunchArgs>(args);
            CreateLicense(largs.type, largs.uid, largs.pubKey, largs.license, largs.days);
        }

        public static string LicToString(LicenseInfo li)
        {
            return string.Format("Name:{0};Expire:{1};Machine:{2};FC23FA", li.AppName, li.ExpireTime, li.MachineID);
        }

        public static void CreateLicense(string app_name, string uid, string publicKey, string licenseFile, int days = 3)
        {
            var now = DateTime.Now;
            var expire = now.AddDays(days);
            var expire_str = expire.ToString("yyyy-MM-dd");
            string publicContent = File.ReadAllText(publicKey);
            RSA rsa = new RSA(publicContent, true);
            LicenseInfo license = new MakeInstall.License.LicenseInfo();
            license.AppName = app_name;
            license.ExpireTime = expire_str;
            license.MachineID = uid;

            var info = LicToString(license);

            var license_pub = rsa.Encode(info);

            int len = license_pub.Length;
            int step = 25;
            int index = 0;
            StringBuilder sb = new StringBuilder("-------- license --------\n");
            while (index < len)
            {
                if (step + index > len)
                {
                    step = len - index;
                }
                sb.Append(license_pub.Substring(index, step));
                sb.Append("\n");
                index += step;
            }
            sb.Append("-------- license --------");            
            File.WriteAllText( $"{licenseFile}-{expire_str}.txt", sb.ToString());
        }
    }
}
