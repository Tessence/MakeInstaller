using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MakeInstall.License
{
    class LicenseHelper
    {
        public const string APP_NAME = "MAKE_INSTALL";
        private static string GetCpuID()
        {
            try
            {
                string cpuInfo = "";//cpu序列号
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
                moc = null;
                mc = null;
                return cpuInfo;
            }
            catch
            {
                return "unknow";
            }

            finally
            { }
        }

        /// <summary>
        /// 获取网卡地址
        /// </summary>
        /// <returns>网卡地址</returns>
        private static string GetMacAddressNew()
        {
            const int MIN_MAC_ADDR_LENGTH = 12;
            string macAddress = string.Empty;
            long maxSpeed = -1;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                string tempMac = nic.GetPhysicalAddress().ToString();
                if (nic.Speed > maxSpeed &&
                    !string.IsNullOrEmpty(tempMac) &&
                    tempMac.Length >= MIN_MAC_ADDR_LENGTH)
                {
                    maxSpeed = nic.Speed;
                    macAddress = tempMac;
                }
            }

            return macAddress;
        }

        private static string GetUserName()
        {
            try
            {
                string un = "";
                un = Environment.UserName;
                return un;
            }
            catch
            {
                return "unknow";
            }
            finally
            { }
        }

        private static string localUniqueId = string.Empty;
        public static string GetLocalUniqueID()
        {
            if(!string.IsNullOrEmpty(localUniqueId))
            {
                return localUniqueId;
            }
            string id = APP_NAME;
            id = GetCpuID();
            id += GetMacAddressNew();
            
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(id);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                localUniqueId = sb.ToString();
                return localUniqueId;
            } 
        }


        private static string LicenseName
        {
            get
            {
                return "license.txt";
            }
        }

        private static string license_path = null;
        public static string LicensePath
        {
            get
            {                
                if(!string.IsNullOrEmpty(license_path))
                {
                    return license_path;
                }
                var currentFile = Process.GetCurrentProcess().MainModule.FileName;
                var curentPath = Path.GetDirectoryName(currentFile);
                license_path = Path.Combine(curentPath, LicenseName);
                if (!File.Exists(license_path))
                {
                    Environment.SpecialFolder[] folders = new Environment.SpecialFolder[] {
                        Environment.SpecialFolder.MyDocuments,
                        Environment.SpecialFolder.UserProfile,
                        Environment.SpecialFolder.Desktop,
                    };
                    foreach(var fd in folders)
                    {
                        var path = Path.Combine(Environment.GetFolderPath(fd), LicenseName);
                        if (File.Exists(path))
                        {
                            license_path = path;
                            break;
                        }
                    }
                    license_path = null;
                }

                return license_path;
            }
        }

        public static LicenseInfo LoadLicense(string privateContent, bool force=false)
        {
            if (license != null && !force)
            {
                return license;
            }
            license = null;
            RSA rsa = new RSA(privateContent, true);
            var licensePath = LicensePath;
            if (string.IsNullOrEmpty(licensePath))
            {
                return license;
            }
            var licenseContent = File.ReadAllText(licensePath);

            licenseContent= licenseContent.Replace("--------", "");
            licenseContent = licenseContent.Replace(" ", "");
            licenseContent = licenseContent.Replace("\n", "");
            licenseContent = licenseContent.Replace("license", "");

            var licenseInfo = rsa.DecodeOrNull(licenseContent);
            license = LicenseInfo.FromString(licenseInfo);
            return license; 
        }


       

        private static LicenseInfo license = null;

        public static bool Licensed (Action<string, bool> msg)
        {
            license = LoadLicense(Properties.Resources.rsa_private_key);
            if (license == null)
            {
                //MessageBox.Show("没有找到有效的授权文件", "授权警告", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                msg("没有找到有效的授权文件", true);
                return false;
            }

            if(!license.AppName.Equals(APP_NAME))
            {
                //MessageBox.Show("授权文件不匹配", "授权警告", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                msg("授权文件不匹配", true);
                return false;
            }

            string localId = GetLocalUniqueID();
            if (!license.MachineID.Equals(localId))
            {
                //MessageBox.Show("授权文件不匹配", "授权警告", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                msg("不是当前电脑的授权文件(CPU序列号).", true);
                return false;
            }

            DateTime expireTime = DateTime.MinValue;
            DateTime.TryParse(license.ExpireTime, out expireTime);
            if(expireTime< DateTime.Now)
            {
               // MessageBox.Show("授权已过期", "授权警告", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                msg("授权已过期", true);
                return false;
            }
            msg("已授权", false);
            return true;
        }
    }
}
