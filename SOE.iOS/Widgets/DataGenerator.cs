using Binding;
using Foundation;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SOE.iOS.Widgets
{
    [Preserve]
    public abstract class DataGenerator
    {
        protected const string GroupId = "group.com.soe.soe-app";
        protected abstract string FileName { get; }
        protected abstract Task<IEnumerable> GenerateData();
        protected string SerializateData(object data)
        {
            if (data is null)
            {
                return String.Empty;
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(data, Formatting.Indented);
        }
        protected void WriteToFile(string jsonData)
        {
            if (jsonData is null)
            {
                return;
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
            }
            else
            {
                Console.WriteLine("File does not exists");
            }

            using(FileStream fileStream=new FileStream(url.Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                fileStream.Position = 0;
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    fileStream.SetLength(0); // discard the contents of the file by setting the length to 0
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
        public async Task GenerateAndRefresh()
        {
            await Task.Yield();
            try
            {
                IEnumerable data = await GenerateData();
                if(data!=null)
                    WriteToFile(SerializateData(data));

            }catch(Exception ex)
            {
                Kit.Log.Logger?.Error(ex, "GenerateAndRefresh");
            }
        }
    }
}

