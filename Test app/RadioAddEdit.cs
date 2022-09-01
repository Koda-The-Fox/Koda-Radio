using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Koda_Radio.model;
using Koda_Radio.util;

namespace Koda_Radio
{
    public partial class RadioAddEdit : Form
    {
        public RadioAddEdit()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                // @TODO Make a function that adds the radio to file
                radioObject ro = new radioObject();
                try
                {
                    ro.Name = textBox1.Text;
                    ro.Url = textBox2.Text;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                radioGroup rg = new radioGroup();
                // @TODO Make a function that adds the radio to the existing group or make a new group and add the radio to it
            }

        }

        private void RadioAddEdit_Load(object sender, EventArgs e)
        {

        }
    }
}
