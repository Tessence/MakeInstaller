using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    public class InstallInfo
    {

        public InstallInfo()
        {
            Files = new List<string>();
            RegValues = new List<RegValue>();
        }

        public string InstallPath { get; set; }
        public List<string> Files { get; set; }
        public List<RegValue> RegValues { get; set; }

        public void AddFile(string file)
        {
            Files.Add(file);
        }

        public void AddReg(RegValue reg)
        {
            RegValues.Add(reg);
        }
    }
}
