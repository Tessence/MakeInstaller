using System;
using System.Collections.Generic;
using System.Text;

namespace MakeInstall
{
    public class PackConfig
	{
		public string Name { get; set; }
		public string Start { get; set; }

		public string Icon { get; set; }
		public string Uninstaller { get; set; }

		public string Source { get; set; }
		public List<string> Filter { get; set; }
		public string InstallFolder { get; set; }
		public bool DesktopIcon { get; set; }
		public bool StartScreen { get; set; }
		public bool OverrideExists { get; set; }

		public string Output { get; set; }

		public List<RegValue> RegValues { get; set; }
		public string AfterInstall { get; set; }

		public string AfterRemove { get; set; }
	}
}
