﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeInstall
{
    public class RegValue
    {
        public string Type { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public Dictionary<string,string> Values { get; set; }
    }
}
