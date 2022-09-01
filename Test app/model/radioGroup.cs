using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koda_Radio.model
{
    public class radioGroup
    {
        private string name;
        private List<radioObject> radios;

        public string Name { get { return name; } set { name = value; } }
        public List<radioObject> Radios { get { return radios; } set { radios = value; } }
    }
}
