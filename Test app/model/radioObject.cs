using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static System.Windows.Forms.ListViewItem;

namespace Koda_Radio.model
{
    public class radioObject
    {
        private string _name;
        private string _url;
        private string _group;
        private string _fetchName;

        public string Name { get { return _name; } set { _name = value; } }  
        public string Url { get { return _url; } set { _url = value; } }
        public string Group { get { return _group;} set { _group = value; } }
        public string FetchName { get => _fetchName; set => _fetchName = value; }

        public ListViewItem GetListViewItem()
        {
            ListViewItem lvi = new ListViewItem
            {
                Text = _name
            };
            lvi.SubItems.Add(new ListViewSubItem().Text = _url);
            return lvi;
        }

        public bool lviEquals(ListViewItem lvi1)
        {
            bool result = false;
            ListViewItem lvi2 = GetListViewItem(); 
            if (lvi1 != null)
            {
                bool textValidator = lvi1.Text == lvi2.Text;
                bool subTextValidator =lvi2.SubItems[0].Text == lvi1.SubItems[0].Text;
                result = textValidator && subTextValidator;
            }
            return result;

        }

    }
}
