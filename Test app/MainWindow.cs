using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Koda_Radio.model;
using Koda_Radio.util;
using Koda_radio;

namespace Koda_Radio
{
    public partial class MainWindow : Form
    {
        ListViewItem nowPlaying = null;
         

        public MainWindow()
        {
            InitializeComponent();
            axWindowsMediaPlayer1.uiMode = "none";
            axWindowsMediaPlayer1.settings.volume = (int)numericUpDown1.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new RadioAddEdit().Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Actemium\Radio\";
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "radios.json");
            if (File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
                if (File.ReadAllText(filePath) == "")
                {
                    File.AppendAllText(filePath, "{}");
                }
            }

            JObject o1 = JObject.Parse(File.ReadAllText(filePath));
            Console.WriteLine(o1.ToString());

            //ListViewTestFill();
            jsonControl.GetAllRadioFromJson(listView1);


            /*
            List<radioObject> lro = jsonControl.GetAllRadioObjectsFromJson();
            List<radioGroup> lrg = jsonControl.GetAllRadioGroupsFromJson();
            */



            // read JSON directly from a file
            using (StreamReader file = File.OpenText(filePath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o2 = (JObject)JToken.ReadFrom(reader);
                Console.WriteLine(o2.ToString());
            }
        }



        private void ListViewTestFill()
        {
            listView1.Items.Clear();
            listView1.Groups.Clear();

            ListViewItem item1 = new ListViewItem() { Name = "lvitst1", Text = "test1"};
            item1.SubItems.Add(new ListViewItem.ListViewSubItem() { Name = "lvsitst1", Text = "url1" });

            ListViewItem item2 = new ListViewItem() { Name = "lvitst2", Text = "test2"};
            item2.SubItems.Add(new ListViewItem.ListViewSubItem() { Name = "lvsitst2", Text = "url2" });

            ListViewItem item3 = new ListViewItem() { Name = "lvitst3", Text = "test3"};
            item3.SubItems.Add(new ListViewItem.ListViewSubItem() { Name = "lvsitst3", Text = "url3" });


            ListViewGroup group1 = new ListViewGroup() { Name = "lvgtst1", Header = "group1", Items = {item1} };

            ListViewItem item4 = new ListViewItem() { Name = "lvitst4", Text = "test4" };
            item4.SubItems.Add(new ListViewItem.ListViewSubItem() { Name = "lvsitst4", Text = "url4" });
            item4.Group = group1;


            ListViewGroup group2 = new ListViewGroup() { Name = "lvgtst2", Header = "group2", Items = {item2} };

            ListViewItem item5 = new ListViewItem() { Name = "lvitst5", Text = "test5" };
            item5.SubItems.Add(new ListViewItem.ListViewSubItem() { Name = "lvsitst5", Text = "url5" });
            item5.Group = group2;


            //listView1.Items.Add(item1);
            //listView1.Items.Add(item2);
            listView1.Items.Add(item3);
            //listView1.Items.Add(item4);
            listView1.Groups.Add(group1);
            listView1.Groups.Add(group2);
            foreach (ListViewGroup lvg in listView1.Groups)
                foreach (ListViewItem lvi in lvg.Items)
                    listView1.Items.Add(lvi);
            //listView1.Items.Add(item5);
        }



        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ChangeRadioStation(object sender, EventArgs e, object target = null)
        {
            ListViewItem lvi = null;
            if (target == null)
                lvi = sender as ListViewItem;
            else
                lvi = target as ListViewItem;


            Console.WriteLine("Playing: " + lvi.SubItems[0].Text);
            Console.WriteLine("url: " + lvi.SubItems[1].Text);
            
            
            foreach (ListViewItem lvi2 in listView1.Items){
                if (lvi2.BackColor == Color.LightGreen)
                    lvi2.BackColor = Color.FromName("Window");
            }
            lvi.BackColor = Color.LightGreen;


            axWindowsMediaPlayer1.URL = lvi.SubItems[1].Text;
            axWindowsMediaPlayer1.Ctlcontrols.play();
            nowPlaying = lvi;
            this.Text = $"Playing {lvi.Text}";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.hendrikjansen.nl/henk/streaming.html");
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.volume = (int)numericUpDown1.Value;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            muteAudio();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void tsmiPresetVolumeSet_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            string volume = tsmi.Text.Replace("%","");
            updateVolume(int.Parse(volume));
        }

        private void timerVolumeTextUpdater_Tick(object sender, EventArgs e)
        {
            //updateVolume(axWindowsMediaPlayer1.settings.volume);
            int volume = axWindowsMediaPlayer1.settings.volume;
            numericUpDown1.Value = volume;
            tsmiCurVolRO.Text = "Current volume: " + volume.ToString() + "%";
            tsCurVolBox.Text = volume.ToString();


            // mute button updater
            muteToolStripMenuItem.Checked = axWindowsMediaPlayer1.settings.mute;

            if (axWindowsMediaPlayer1.settings.mute)
            {
                button4.BackColor = Color.Red;
                muteToolStripMenuItem.BackColor = Color.Red;
            }
            else
            {
                button4.BackColor = Color.FromName("Control");
                muteToolStripMenuItem.BackColor = Color.FromName("Control");
            }
        }

        private void updateVolume(int volume)
        {
            axWindowsMediaPlayer1.settings.volume = volume;
            timerVolumeTextUpdater_Tick(null, null);
        }

        private void muteAudio()
        {
            //toggle
            axWindowsMediaPlayer1.settings.mute = !axWindowsMediaPlayer1.settings.mute;

            if (axWindowsMediaPlayer1.settings.mute)
            {
                button4.BackColor = Color.Red;
                muteToolStripMenuItem.BackColor = Color.Red;
            }
            else
            {
                button4.BackColor = Color.FromName("Control");
                muteToolStripMenuItem.BackColor = Color.FromName("Control");
            }
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tsmiStationRoot_Click(object sender, EventArgs e)
        {
            

        }

        private void notifIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.Focus();
        }

        private void notifIconCMS_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("CMS Openened");
            tsmiStationRoot.DropDownItems.Clear();
            // Groups
            foreach (ListViewGroup lvg in listView1.Groups)
            {
                ToolStripMenuItem groupItem = new ToolStripMenuItem();
                groupItem.Text = lvg.Header;
                groupItem.Name = lvg.Header;

                // Item
                foreach (ListViewItem lvi in lvg.Items)
                {
                    ToolStripMenuItem tsmItem = new ToolStripMenuItem();
                    tsmItem.Text = lvi.Text;
                    tsmItem.Name = lvi.Name;
                    tsmItem.Click += (se, ev) => this.ChangeRadioStation(lvi, ev);
                    if (lvi == nowPlaying)
                        tsmItem.Checked = true;
                    groupItem.DropDownItems.Add(tsmItem);
                }
                tsmiStationRoot.DropDownItems.Add(groupItem);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView lv = sender as ListView;

            if (lv.SelectedItems.Count == 0)
            {
                return;
            }
            else if (lv.SelectedItems.Count > 1)
            {
                MessageBox.Show("Too many items selscted.");
                return;
            }

            ChangeRadioStation(lv.SelectedItems[0], new EventArgs());
        }

        private void tsmiAdd5Percent_Click(object sender, EventArgs e)
        {
            int vol = axWindowsMediaPlayer1.settings.volume;
            vol += 5;
            if (vol > 100)
                vol = 100;
            updateVolume(vol);
        }

        private void tsmiRem5Percent_Click(object sender, EventArgs e)
        {
            int vol = axWindowsMediaPlayer1.settings.volume;
            vol -= 5;
            if (vol < 0)
                vol = 0;
            updateVolume(vol);
        }

        private void tsCurVolBox_TextChanged(object sender, EventArgs e)
        {
            tsCurVolBox.BackColor = Color.FromName("Control");
            int vol = 0;
            if (!int.TryParse(tsCurVolBox.Text, out vol))
                tsCurVolBox.BackColor = Color.Red;

            updateVolume(vol);
        }

        private void button5_Click(object sender, EventArgs e)
        {

            jsonControl.GetAllRadioFromJson(listView1);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

            frmSettings frmStngs = new frmSettings();
            frmStngs.Show();
        }
    }
}
