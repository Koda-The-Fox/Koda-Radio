﻿using Newtonsoft.Json;
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
using System.Net;
using System.Reflection;

namespace Koda_Radio
{
    public partial class MainWindow : Form
    {
        public static List<object> listObj = new List<object>();
        radioObject nowPlayingStation = null;
        radioGroup nowPlayingGroup = null;
        //ListViewItem nowPlaying = null;
         

        public MainWindow()
        {
            InitializeComponent();
            axWindowsMediaPlayer1.uiMode = "none";
            axWindowsMediaPlayer1.settings.volume = (int)numericUpDown1.Value;

            lblVersion.Text = String.Format("v.{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());


            #region Test Track viewer

            string station = "radio-veronica-top-1000-allertijden";

            string query = "%7B%0A%20%20getStations(profile%3A%20%22radio-brand-web%22%2C%20slug%3A%20%22stations-radio-veronica%22)%20%7B%0A%20%20%20%20items%20%7B%0A%20%20%20%20%20%20title%0A%20%20%20%20%20%20slug%0A%20%20%20%20%20%20playouts(profile%3A%20%22%22%2C%20limit%3A%201)%20%7B%0A%20%20%20%20%20%20%20%20broadcastDate%0A%20%20%20%20%20%20%20%20track%20%7B%0A%20%20%20%20%20%20%20%20%20%20id%0A%20%20%20%20%20%20%20%20%20%20title%0A%20%20%20%20%20%20%20%20%20%20artistName%0A%20%20%20%20%20%20%20%20%20%20isrc%0A%20%20%20%20%20%20%20%20%20%20images%20%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20type%0A%20%20%20%20%20%20%20%20%20%20%20%20uri%0A%20%20%20%20%20%20%20%20%20%20%20%20__typename%0A%20%20%20%20%20%20%20%20%20%20%7D%0A%20%20%20%20%20%20%20%20%20%20__typename%0A%20%20%20%20%20%20%20%20%7D%0A%20%20%20%20%20%20%20%20__typename%0A%20%20%20%20%20%20%7D%0A%20%20%20%20%20%20__typename%0A%20%20%20%20%7D%0A%20%20%20%20__typename%0A%20%20%7D%0A%7D&variables=%7B%7D";
            string query2 = "{ getStations(profile: \"radio - brand - web\", slug: \"stations - radio - veronica\") { items { title slug playouts(profile: \"\", limit: 1) { broadcastDate track { id title artistName isrc images { type uri __typename } __typename } __typename } __typename } __typename } }";
            string query3 = "{ getStation(profile: \"radio-brand-web\" slug: \"radio-veronica-top-1000-allertijden\") { title playouts(profile: \"\", limit: 10) { broadcastDate track { id title artistName isrc images { type uri __typename } __typename } __typename } __typename } }";
            string query4 = $"{{ getStation(profile: \"radio-brand-web\" slug: \"{station}\") {{ title playouts(profile: \"\", limit: 5) {{ broadcastDate track {{ id title artistName isrc images {{ type uri __typename }} __typename }} __typename }} __typename }} }}";
            var urlAddress = $@"https://graph.talparad.io/?query={query4}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);

            //request.Headers.Add(HttpRequestHeader.Accept, "application/json")
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
            //request.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            request.Headers.Add("x-api-key", "da2-vak2kxayzzh7lgexfqvefckdby");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "cross-site");

            request.Accept = "*/*";
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Referer = "https://www.radioveronica.nl/"; // Required 
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.CharacterSet == null)
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                string data = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
                JObject o1 = JObject.Parse(data);
                Console.WriteLine(data.ToString());
            }
            #endregion
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
                if (string.IsNullOrEmpty(File.ReadAllText(filePath)))
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
            nowPlayingGroup = null;
            nowPlayingStation = null;

            pictureBox1.Visible = false;
            pictureBox1.ImageLocation = "";

            ListViewItem lvi = null;
            if (target == null)
                lvi = sender as ListViewItem;
            else
                lvi = target as ListViewItem;


            //Console.WriteLine("Playing: " + lvi.SubItems[0].Text);
            //Console.WriteLine("url: " + lvi.SubItems[1].Text);
            
            
            foreach (ListViewItem lvi2 in listView1.Items){
                if (lvi2.BackColor == Color.LightGreen)
                    lvi2.BackColor = Color.FromName("Window");
            }
            lvi.BackColor = Color.LightGreen;


            axWindowsMediaPlayer1.URL = lvi.SubItems[1].Text;
            axWindowsMediaPlayer1.Ctlcontrols.play();
            //nowPlaying = lvi;
            

            foreach (object obj in listObj)
            {
                try
                {
                    radioObject raObj = (radioObject)obj;
                    if (raObj.lviEquals(lvi))
                    {
                        nowPlayingStation = raObj;
                        break;
                    }
                }
                catch
                {
                    radioGroup raGrp = (radioGroup)obj;
                    foreach (radioObject raObj in raGrp.Radios)
                    {
                        if (raObj.lviEquals(lvi))
                        {
                            nowPlayingGroup = raGrp;
                            nowPlayingStation = raObj;
                            break;
                        }
                    }
                }
            }

            UpdateSngList();

            this.Text = $"Playing: {lvi.Text}";


            lblStationTitle.Text = lvi.Text;
            notifIcon.Text = $"Playing: {lvi.Text}";
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

        private bool CloseOverride = false;
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseOverride = true;
            this.Close();
        }

        private void tsmiStationRoot_Click(object sender, EventArgs e)
        {
            

        }

        private void notifIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            tsmiChangeState.Text = "Hide";
            Show();
            Activate(); // send to front
            WindowState = FormWindowState.Normal;
        }

        private void notifIconCMS_Opened(object sender, EventArgs e)
        {
            //Console.WriteLine("CMS Openened");

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
                    if (nowPlayingStation != null)
                        if (lvi == nowPlayingStation.GetListViewItem())
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
            new frmSettings().Show();
        }

        private void lblAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new AboutKodaCreations().Show();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (chbxHideOnClose.Checked && !CloseOverride)
            {
                notifIcon.BalloonTipTitle = "Koda Radio was hidden to the tool bar.";
                notifIcon.BalloonTipText = "Disable the setting 'Hide On Close' to prevent this from happening.";
                notifIcon.ShowBalloonTip(500);
                e.Cancel = true;
                tsmiChangeState.Text = "Show";
                Hide();
            }
        }

        private void notifIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            Show();
        }


        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifIcon.Visible = false;
        }

        private void tsmiChangeState_Click(object sender, EventArgs e)
        {
            if (tsmiChangeState.Text == "Hide")
            {
                tsmiChangeState.Text = "Show";
                this.Hide();
            }
            else
            {
                tsmiChangeState.Text = "Hide";
                this.Show();
            }
        }

        private void tmrSongUpdtr_Tick(object sender, EventArgs e)
        {
            UpdateSngList();
        }

        private void UpdateSngList()
        {
            if (nowPlayingStation != null)
                if (nowPlayingStation.FetchName != null)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                        nowPlayingGroup.FetchURL+
                        nowPlayingGroup.FetchQuery.Replace("@station@", nowPlayingStation.FetchName).Replace("\\\"", "%22").Replace(":", "%3A")/*.Replace("{{", "%7B").Replace("}}", "%7D")*/);

                    //request.Headers.Add(HttpRequestHeader.Accept, "application/json")
                    request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
                    //request.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    request.Headers.Add("x-api-key", "da2-vak2kxayzzh7lgexfqvefckdby");
                    request.Headers.Add("Sec-Fetch-Dest", "empty");
                    request.Headers.Add("Sec-Fetch-Mode", "cors");
                    request.Headers.Add("Sec-Fetch-Site", "cross-site");

                    request.Accept = "*/*";
                    request.ContentType = "application/json";
                    request.Method = "GET";
                    request.Referer = "https://www.radioveronica.nl/"; // Required 
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream receiveStream = response.GetResponseStream();
                        StreamReader readStream = null;
                        if (response.CharacterSet == null)
                            readStream = new StreamReader(receiveStream);
                        else
                            readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                        string data = readStream.ReadToEnd();
                        response.Close();
                        readStream.Close();

                        // TODO: Find a proper way to travel trough the/a Json.

                        JToken t1 = JToken.Parse(data);
                        JObject o1 = JObject.Parse(data);

                        dynamic dynJson = JsonConvert.DeserializeObject(data);
                        JToken songList = null;
                        try
                        {
                            songList = dynJson["data"]["getStation"]["playouts"];
                        }
                        catch { }
                        if (songList != null)
                        {
                            IEnumerable<JToken> list = songList.Children();
                            try
                            {
                                JToken song = list.First();
                                try
                                {
                                    //lblSong1Timestamp.Text = DateTime.Parse(song["broadcastDate"].ToString()).ToString("HH:mm:ss");
                                }
                                catch { }
                                try
                                {
                                    lblSong1Title.Text = song["track"]["artistName"].ToString() + " - " + song["track"]["title"].ToString();
                                }
                                catch { lblSong1Title.Text = ""; }
                                try
                                {
                                    JToken images = song["track"]["images"].First;
                                    if (images != null)
                                    {
                                        pictureBox1.ImageLocation = images["uri"].ToString();
                                        pictureBox1.Visible = chbxShowAlbum.Checked;
                                    }
                                }
                                catch
                                {
                                    pictureBox1.Visible = false;
                                    pictureBox1.ImageLocation = "";
                                }
                            }
                            catch { }
                            try
                            {
                                JToken song = list.Skip(1).First();// second song
                                try
                                {
                                    lblSong2Timestamp.Text = DateTime.Parse(song["broadcastDate"].ToString()).ToString("HH:mm:ss");
                                }
                                catch { }
                                try
                                {
                                    lblSong2Title.Text = song["track"]["artistName"].ToString() + " - " + song["track"]["title"].ToString();
                                }
                                catch { lblSong2Title.Text = ""; }
                                try
                                {
                                    JToken images = song["track"]["images"].First;
                                    if (images != null)
                                    {
                                        pictureBox1.ImageLocation = images["uri"].ToString();
                                        pictureBox1.Visible = chbxShowAlbum.Checked;
                                        toolTip1.SetToolTip(pictureBox1, pictureBox1.ImageLocation);
                                    }
                                }
                                catch
                                {
                                    pictureBox1.Visible = false;
                                    pictureBox1.ImageLocation = "";
                                    toolTip1.SetToolTip(pictureBox1, pictureBox1.ImageLocation);
                                }
                            }
                            catch { }
                            /*
                            foreach (JToken song in list)
                            {
                                Console.WriteLine(song);
                                try
                                {
                                    lblSong1Timestamp.Text = DateTime.Parse(song["broadcastDate"].ToString()).ToString("HH:mm:ss");
                                } catch { }
                                try
                                {
                                    lblSong1Title.Text = song["track"]["artistName"].ToString() + " - " + song["track"]["title"].ToString();
                                } catch { }
                                try
                                {
                                    pictureBox1.ImageLocation = song["track"]["images"]["uri"].ToString();
                                    pictureBox1.Visible = true;
                                } catch
                                {
                                    pictureBox1.Visible = false;
                                    pictureBox1.ImageLocation = "";
                                }
                            }
                            */

                        }


                        Console.WriteLine(data);
                    }
                }
        }

        private void chbxShowAlbum_CheckedChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(pictureBox1.ImageLocation))
                pictureBox1.Visible = chbxShowAlbum.Checked;
        }


        private void lblSong1Title_TextChanged(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            if (!string.IsNullOrEmpty(lblSong1Title.Text))
            {
                lblSong1Timestamp.Text = "Currently playing";
            }
            toolTip1.SetToolTip(lbl, lbl.Text);
        }

        private void lblSongTitle_TextChanged(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            toolTip1.SetToolTip(lbl, lbl.Text);
        }
    }
}
