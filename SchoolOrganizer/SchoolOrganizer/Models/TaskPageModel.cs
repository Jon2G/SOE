using SchoolOrganizer.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SchoolOrganizer.Models
{
    class TaskPageModel
    {
        internal static async Task<FileImageSource> SaveImage(FileResult result)
        {
            await Task.Yield();
            DirectoryInfo directory = AppData.Instance.TaskImagesDirectory;
            if (!directory.Exists)
            {
                directory.Create();
            }
            FileInfo avatar = new FileInfo(Path.Combine(directory.FullName, result.FileName));
            using (FileStream stream = new FileStream(avatar.FullName, FileMode.OpenOrCreate))
            {
                using (var image = await result.OpenReadAsync())
                {
                    await image.CopyToAsync(stream);
                }
            }

            return GetImage(avatar);
        }
        internal static FileImageSource GetImage(FileInfo av)
        {
            DirectoryInfo directory = AppData.Instance.TaskImagesDirectory;
            if (!directory.Exists)
            {
                directory.Create();
                return null;
            }

            FileInfo avatar = new FileInfo(av.FullName);
            if (!avatar.Exists)
            {
                return null;
            }
            return (FileImageSource)FileImageSource.FromFile(avatar.FullName);
        }
    }
}
