using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revanced_Builder
{
    public class CompatiblePackage
    {
        public string name { get; set; }
        public List<string> versions { get; set; }
    }

    public class Option
    {
        public string key { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public bool required { get; set; }
        public List<string> choices { get; set; }
    }
    public class Patch
    {
        public string name { get; set; }
        public string description { get; set; }
        public string version { get; set; }
        public bool excluded { get; set; }
        public bool deprecated { get; set; }
        public List<Option> options { get; set; }
        public List<string> dependencies { get; set; }
        public List<CompatiblePackage> compatiblePackages { get; set; }

    }
}
