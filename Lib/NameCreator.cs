using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lib
{
    public class NameCreator
    {
        private string defName;
        private SortedSet<string> names; 
        public NameCreator(IEnumerable<string> names, string defName)
        {
            this.defName = defName;
            this.names = new SortedSet<string>();
            foreach (string name in names)
                this.names.Add(name);

        }
        private int getnumbersRight(string str)
        {
            Regex regex = new Regex("[\\d]+$");
            string match = regex.Match(str).Value;
            if (string.IsNullOrEmpty(match)) return 1;
            return int.Parse(match) + 1;
        }
        private string getNameWONumber(string str)
        {
            Regex regex = new Regex("^[\\D]+");
            string match = regex.Match(str).Value;
            if (string.IsNullOrEmpty(match)) return "";
            return match;
        }
        public string GetName(string name, bool addGenerated = true)
        {
            if (string.IsNullOrEmpty(name)) name = defName;
            while (names.Contains(name))
                name = getNameWONumber(name) + getnumbersRight(name);
            if (addGenerated) names.Add(name);
            return name;
        }

    }
}
