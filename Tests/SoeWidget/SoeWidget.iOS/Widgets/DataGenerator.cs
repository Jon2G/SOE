using Binding;
using Foundation;
using Newtonsoft.Json;
using SoeWidget.iOS.Widgets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

[assembly:Dependency(typeof(DataGenerator))]
namespace SoeWidget.iOS.Widgets
{
    public class DataGenerator:ITestDataGenerator
    {
        public partial class Values
        {
            [JsonProperty("data")]
            public Dictionary<string, Datum> Data { get; set; }
        }

        public partial class Datum
        {
            [JsonProperty("value")]
            public string Value { get; set; }

            [JsonProperty("delta")]
            public string Delta { get; set; }
        }

        public const string GroupId = "group.com.soe.soe-app";
        public const string WidgetFileData = "timeline.json";
        

        public DataGenerator()
        {
        }

        public void GenerateAndLog()
        {
            NSUrl url = NSFileManager.DefaultManager.GetContainerUrl(GroupId);
            if(url is null)
            {
                Console.WriteLine("NULL CONTAINER URL");
                return;
            }
            url = url.Append(WidgetFileData, false);
            Console.WriteLine(url.Path);
            if (File.Exists(url.Path))
            {
                Console.WriteLine("File already exists");
                Console.WriteLine(File.ReadAllText(url.Path));
            }
            else
            {
                Console.WriteLine("File does not exists");
            }

            using(FileStream fileStream=new FileStream(url.Path, FileMode.OpenOrCreate))
            {
                fileStream.Position = 0;
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    WriteTestData(writer);
                }
            }
            Console.WriteLine("====DONE====");

            var proxy = new WidgetCenterProxy();
            proxy.ReloadAllTimeLines();
            proxy.GetCurrentConfigurationsWithCompletion((widgets) => {

                foreach (WidgetInfoProxy widget in widgets)
                {
                    
                    Console.WriteLine(widget.Kind + " " + widget.Family);

                }
                Console.WriteLine("After widgets " + widgets.Count);
            }
            );



        }

        private void WriteTestData(StreamWriter writer)
        {
            // Go go fake business logic!
            Values v = new Values
            {
                Data = new Dictionary<string, Datum> {
                    { "2021-08-12",  new Datum { Value = "50.34", Delta = "-1.68"} },
                    { "2022-08-12",  new Datum { Value = "51.99", Delta = "-0.03"} },
                    { "2023-08-12",  new Datum { Value = "51.56", Delta = "-0.46"} },
                }
            };
            writer.Write(JsonConvert.SerializeObject(v));
        }
    }
}

