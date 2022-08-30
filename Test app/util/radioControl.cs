using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_app.util
{
    public static class radioControl
    {


        public static void AddRadio()
        {

        }

        public static void RemoveRadio()
        {

        }



        public static void GetRadio()
        {
            JObject o1 = JObject.Parse(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));

            // read JSON directly from a file
            using (StreamReader file = File.OpenText(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o2 = (JObject)JToken.ReadFrom(reader);
                Console.WriteLine(o2.ToString());
            }

        }



        public static void AddGroup()
        {

        }

        public static void RemoveGroup()
        {

        }




    }
}
