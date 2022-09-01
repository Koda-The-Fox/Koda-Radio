using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Koda_Radio.model;
using static System.Windows.Forms.ListViewItem;
using Koda_radio.model;

namespace Koda_Radio.util
{
    public static class jsonControl
    {
        public static Settings GetSettings()
        {
            Settings settings = new Settings();




            return settings;
        }


        public static void GetAllRadioFromJson(ListView lv)
        {
            try
            {
                string strng = "";
                FileInfo fi = new FileInfo(Path.Combine(Environment.CurrentDirectory, @"Media\Documents\RadioList.json").ToString());
                //MessageBox.Show(fi.FullName);
                if (fi.Exists) // From file in deployment dir
                    strng = File.ReadAllText(fi.FullName);
                else // From resources
                    strng = Encoding.Default.GetString(Koda_radio.Properties.Resources.RadioList);


                List<object> listViewObj = RadioRecursion(strng);



                lv.Items.Clear();
                lv.Groups.Clear();

                foreach (object obj in listViewObj)
                {
                    bool lvi = false;
                    try
                    {
                        ListViewItem item = (ListViewItem)obj;
                        lvi = true;
                        lv.Items.Add(item);
                    }
                    catch (Exception ignore) { }
                    if (!lvi) 
                    {
                        try
                        {
                            ListViewGroup group = (ListViewGroup)obj;
                            lv.Groups.Add(group);
                        }catch (Exception ignore) { }
                    }
                }
                

                foreach (ListViewGroup lvg in lv.Groups)
                    foreach (ListViewItem lvi in lvg.Items)
                        lv.Items.Add(lvi);

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong when parsing the Json File.\n" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

        }


        private static List<Object> RadioRecursion(String strng, String objName = null)
        {
            List<Object> returnList = new List<Object>();
            JObject jObject = JObject.Parse(strng);

            foreach (KeyValuePair<string, JToken> child in jObject)
            {
                if (child.Value.ToString().ToLower() == "station")
                {
                    returnList.Add(createLVI(jObject, objName));
                    break;
                }
                else if (child.Value.ToString().ToLower() == "group")
                {
                    returnList.Add(createLVGI(jObject, objName));
                    break;
                }
                else
                {

                    returnList.AddRange(RadioRecursion(child.Value.ToString(), child.Key.ToString()));
                    continue;
                }
            }

            return returnList;
        }

        private static ListViewItem createLVI(JObject jo, string stationName)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = stationName;
            foreach (KeyValuePair<string, JToken> child in jo)
            {
                /*if (child.Key.ToLower() == "name")
                {
                    lvi.Text = child.Value.ToString();
                }
                else */if (child.Key.ToLower() == "url")
                {
                    lvi.SubItems.Add(new ListViewSubItem().Text = child.Value.ToString());
                }
            }
            return lvi;
        }

        private static ListViewGroup createLVGI(JObject jo, string groupName)
        {
            ListViewGroup lvg = new ListViewGroup();
            lvg.Header = groupName;
            foreach (KeyValuePair<string, JToken> child in jo)
            {
                if (child.Key.ToLower() != "type")
                {
                    lvg.Items.Add(createLVI(JObject.Parse(child.Value.ToString()), child.Key));
                }
            }
            return lvg;
        }



        private static List<radioGroup> handleRadioGroup(JObject jo)
        {
            List<radioGroup> LRG = new List<radioGroup>();

            foreach (KeyValuePair<string, JToken> child in jo)
            {
                JObject jo2 = JObject.Parse(child.Value.ToString());
                string type = "";
                foreach (KeyValuePair<string, JToken> child2 in jo2)
                {
                    radioGroup rg = new radioGroup();
                    if (child2.Value.ToString().ToLower() == "group" || type == "group")
                    {
                        if (type == "")
                        {
                            type = "group";
                            continue;
                        }
                        rg.Name = child2.Key.ToString();

                        JObject jo3 = JObject.Parse(child2.Value.ToString());
                        rg.Radios.Add(createRadio(jo3));
                        continue;
                    }
                    LRG.Add(rg);
                }
            }
            return LRG;
        }


        private static List<radioObject> handleRadioObject(JObject jo)
        {
            List<radioObject> LRO = new List<radioObject>();

            foreach (KeyValuePair<string, JToken> child in jo)
            {
                JObject jo2 = JObject.Parse(child.Value.ToString());
                string type = "";
                foreach (KeyValuePair<string, JToken> child2 in jo2)
                {
                    if (child2.Value.ToString().ToLower() == "station" || type == "station")
                    {
                        LRO.Add(createRadio(jo2, null));
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return LRO;
        }
        private static radioObject createRadio(JObject jo, radioGroup parent = null)
        {
            radioObject ro = new radioObject();
            foreach (KeyValuePair<string, JToken> child in jo)
            {
                if (child.Key.ToLower() == "name")
                {
                    ro.Name = child.Value.ToString();
                }
                else if (child.Key.ToLower() == "url")
                {
                    ro.Url = child.Value.ToString();
                }

                if (parent != null)
                    ro.Group = parent.Name;
            }
            return ro;
        }



        /*
         * Sources:
         * https://www.newtonsoft.com/json/help/html/ReadJson.htm // read data from Json file
         * https://stackoverflow.com/questions/3314140/how-to-read-embedded-resource-text-file // read from embedded file.
         */



    }
}
