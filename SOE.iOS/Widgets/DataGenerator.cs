using Binding;
using Foundation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Xamarin.Forms;

namespace SOE.iOS.Widgets
{
    [Preserve]
    public abstract class DataGenerator
    {
        protected const string GroupId = "group.com.soe.soe-app";
        protected abstract string FileName { get; }
        protected abstract object GenerateData();
        protected string SerializateData(object data)
        {
            if (data is null)
            {
                return null;
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(data, Formatting.Indented);
        }
        protected void WriteToFile(string jsonData)
        {
            if (jsonData is null)
            {
                jsonData = string.Empty;
            }
            NSUrl url = NSFileManager.DefaultManager.GetContainerUrl(GroupId);
            if(url is null)
            {
                Console.WriteLine("NULL CONTAINER URL");
                return;
            }
            url = url.Append(FileName, false);
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
                    writer.Write(jsonData);
                }
            }
            Console.WriteLine("====DONE====");
            WidgetCenterProxy proxy = new WidgetCenterProxy();
            proxy.ReloadAllTimeLines();
            proxy.GetCurrentConfigurationsWithCompletion((widgets) => {

                foreach (WidgetInfoProxy widget in widgets)
                {
                    
                    Console.WriteLine(widget.Kind + " " + widget.Family);

                }
                Console.WriteLine("After widgets " + widgets.Count);
            });
        }
        public void GenerateAndRefresh()
        {
            try
            {
                WriteToFile(SerializateData(GenerateData()));

            }catch(Exception ex)
            {
                Kit.Log.Logger?.Error(ex, "GenerateAndRefresh");
            }
        }
    }
}

