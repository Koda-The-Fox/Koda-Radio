using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_app.model
{
    public class radioObject
    {
        private string name;
        private string url;
        private string group;

        public string Name { get { return name; } set { name = value; } }  
        public string Url { get { return url; } set { url = value; } }
        public string Group { get { return group;} set { group = value; } }

    }
}
