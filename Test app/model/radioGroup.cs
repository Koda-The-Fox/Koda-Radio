using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Koda_Radio.model
{
    public class radioGroup
    {
        private string _name;
        private List<radioObject> _radios = new List<radioObject>();
        private string _fetchURL;
        private string _fetchQuery;

        public string Name { get { return _name; } set { _name = value; } }
        public List<radioObject> Radios { get { return _radios; } set { _radios = value; } }
        public string FetchURL { get => _fetchURL; set => _fetchURL = value; }
        public string FetchQuery { get => _fetchQuery; set => _fetchQuery = value; }

        public ListViewGroup GetListViewGroup()
        {
            ListViewGroup lvg = new ListViewGroup();
            lvg.Header = _name;
            foreach (var radio in _radios)
            {
                lvg.Items.Add(radio.GetListViewItem());
            }
            return lvg;
        }
    }
}
