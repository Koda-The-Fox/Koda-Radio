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
            //List<object> listObj = new List<object>();
            try
            {
                string strng = "";
                FileInfo fi = new FileInfo(Path.Combine(Environment.CurrentDirectory, @"Media\Documents\RadioList.json").ToString());
                //MessageBox.Show(fi.FullName);
                if (fi.Exists) // From file in deployment dir
                    strng = File.ReadAllText(fi.FullName);
                else // From resources
                    strng = Encoding.Default.GetString(Koda_radio.Properties.Resources.RadioList);


                MainWindow.listObj = RadioRecursion(strng);



                lv.Items.Clear();
                lv.Groups.Clear();

                foreach (object obj in MainWindow.listObj)
                {
                    bool lvi = false;
                    try
                    {
                        radioObject item = (radioObject)obj;
                        lvi = true;
                        lv.Items.Add(item.GetListViewItem());
                    }
                    catch { }
                    if (!lvi) 
                    {
                        try
                        {
                            radioGroup group = (radioGroup)obj;
                            lv.Groups.Add(group.GetListViewGroup());
                        }catch { }
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

            if (objName != null ? objName.ToLower() != "fetch" : true)
                foreach (KeyValuePair<string, JToken> child in jObject)
                {
                    if (child.Value.ToString().ToLower() == "station")
                    {
                        //returnList.Add(createLVI(jObject, objName));
                        returnList.Add(createRadioObject(jObject, objName));
                        break;
                    }
                    else if (child.Value.ToString().ToLower() == "group")
                    {
                        //returnList.Add(createLVGI(jObject, objName));
                        returnList.Add(createRadioGroup(jObject, objName));
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

        private static Object FetchRecursion(String joStrng, radioObject raObj = null, radioGroup raGrp = null)
        {
            List<Object> returnList = new List<Object>();
            JObject jo = JObject.Parse(joStrng);

            foreach (KeyValuePair<string, JToken> child in jo)
            {
                if (child.Key.ToString().ToLower() == "url")
                {
                    raGrp.FetchURL = child.Value.ToString();
                }
                else if (child.Key.ToString().ToLower() == "query")
                {
                    raGrp.FetchQuery = child.Value.ToString();
                }
                else if (child.Key.ToString().ToLower() == "station")
                {
                    raObj.FetchName = child.Value.ToString();
                }
                else
                {
                    if (raGrp != null)
                    {
                        raGrp = (radioGroup)FetchRecursion(child.Value.ToString(), raObj, raGrp);
                    }
                    else
                    {
                        raObj = (radioObject)FetchRecursion(child.Value.ToString(), raObj, raGrp);
                    }
                    continue;
                }
            }
            if (raGrp != null)
            {
                return raGrp;
            }
            else 
            {
                return raObj;
            }
        }


        private static ListViewItem createLVI(JObject jo, string stationName)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = stationName;
            foreach (KeyValuePair<string, JToken> child in jo)
            {
                if (child.Key.ToLower() == "url")
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
                else if (child.Key.ToLower() == "fetch")
                {

                }
            }
            return lvg;
        }

        private static radioObject createRadioObject(JObject jo, string stationName)
        {
            radioObject raObj = new radioObject();
            raObj.Name = stationName;
            foreach (KeyValuePair<string, JToken> child in jo)
            {
                if (child.Key.ToLower() == "url")
                {
                    raObj.Url = child.Value.ToString();
                }
                else if (child.Key.ToLower() == "fetch")
                {
                    raObj = (radioObject)FetchRecursion(child.Value.ToString(), raObj);
                }
            }
            return raObj;
        }

        private static radioGroup createRadioGroup(JObject jo, string groupName)
        {
            radioGroup raGrp = new radioGroup();
            raGrp.Name = groupName;
            foreach (KeyValuePair<string, JToken> child in jo)
            {
                if (child.Key.ToLower() == "fetch")
                {
                    raGrp = (radioGroup)FetchRecursion(child.Value.ToString(), null, raGrp);
                }
                else if (child.Key.ToLower() != "type")
                {
                    radioObject raObj = createRadioObject(JObject.Parse(child.Value.ToString()), child.Key);
                    raObj.Group = groupName;
                    raGrp.Radios.Add(raObj);
                }
            }
            return raGrp;
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
